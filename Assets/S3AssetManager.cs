using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class S3AssetManager : MonoBehaviour
{
    public static S3AssetManager Instance;
    
    [Header("S3 Settings")]
    [SerializeField] private string s3BaseUrl = "https://aetheric-game-assets.s3.ap-southeast-2.amazonaws.com";
    [SerializeField] private int maxCacheSizeMB = 100;
    
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();
    
    [Header("Cache Settings")]
    [SerializeField] private bool autoCacheCleanup = true;
    [SerializeField] private bool cleanupUnusedEvents = true; // Xóa assets của các event không còn active
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Kiểm tra cache size và xóa nếu cần
            if (autoCacheCleanup)
            {
                StartCoroutine(CleanupOldCache());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #region Sprite Loading
    
    public void LoadSprite(string spritePath, System.Action<Sprite> callback)
    {
        StartCoroutine(LoadSpriteCoroutine(spritePath, callback));
    }
    
    private IEnumerator LoadSpriteCoroutine(string spritePath, System.Action<Sprite> callback)
    {
        // Check memory cache first
        if (spriteCache.TryGetValue(spritePath, out Sprite cachedSprite))
        {
            callback?.Invoke(cachedSprite);
            yield break;
        }
        
        // Check disk cache
        string cacheKey = "sprite_" + spritePath.GetHashCode();
        if (PlayerPrefs.HasKey(cacheKey))
        {
            string cachedPath = PlayerPrefs.GetString(cacheKey);
            if (File.Exists(cachedPath))
            {
                yield return StartCoroutine(LoadSpriteFromDisk(cachedPath, (loadedSprite) => {
                    if (loadedSprite != null)
                    {
                        spriteCache[spritePath] = loadedSprite;
                        callback?.Invoke(loadedSprite);
                    }
                    else
                    {
                        StartCoroutine(DownloadSprite(spritePath, callback));
                    }
                }));
                yield break;
            }
        }
        
        // Download from S3
        yield return StartCoroutine(DownloadSprite(spritePath, callback));
    }
    
    private IEnumerator DownloadSprite(string spritePath, System.Action<Sprite> callback)
    {
        string url = s3BaseUrl + spritePath;
        Debug.Log($"🔄 Downloading sprite: {url}");
        
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.timeout = 10;
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            
            // Create sprite
            Sprite sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                Vector2.one * 0.5f
            );
            
            // Cache in memory
            spriteCache[spritePath] = sprite;
            
            // Cache to disk
            StartCoroutine(CacheTextureToDisk(texture, spritePath));
            
            callback?.Invoke(sprite);
            Debug.Log($"✅ Sprite downloaded: {spritePath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Failed to download sprite: {request.error}");
            callback?.Invoke(null);
        }
        
        request.Dispose();
    }
    
    private IEnumerator LoadSpriteFromDisk(string path, System.Action<Sprite> callback)
    {
        Debug.Log($"📂 Loading sprite from cache: {path}");
        
        try
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            
            Sprite sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                Vector2.one * 0.5f
            );
            
            callback?.Invoke(sprite);
            Debug.Log("✅ Cached sprite loaded successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to load cached sprite: {e.Message}");
            callback?.Invoke(null);
        }
        
        yield return null;
    }
    
    private IEnumerator CacheTextureToDisk(Texture2D texture, string spritePath)
    {
        try
        {
            string filename = "sprite_" + spritePath.GetHashCode() + ".png";
            string path = Path.Combine(Application.persistentDataPath, filename);
            
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngData);
            
            string cacheKey = "sprite_" + spritePath.GetHashCode();
            PlayerPrefs.SetString(cacheKey, path);
            PlayerPrefs.Save();
            
            Debug.Log($"💾 Sprite cached to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to cache texture: {e.Message}");
        }
        
        yield return null;
    }
    
    #endregion
    
    #region Audio Loading
    
    public void LoadAudio(string audioPath, System.Action<AudioClip> callback)
    {
        StartCoroutine(LoadAudioCoroutine(audioPath, callback));
    }
    
    private IEnumerator LoadAudioCoroutine(string audioPath, System.Action<AudioClip> callback)
    {
        // Check memory cache first
        if (audioCache.TryGetValue(audioPath, out AudioClip cachedClip))
        {
            callback?.Invoke(cachedClip);
            yield break;
        }
        
        // Check disk cache
        string cacheKey = "audio_" + audioPath.GetHashCode();
        if (PlayerPrefs.HasKey(cacheKey))
        {
            string cachedPath = PlayerPrefs.GetString(cacheKey);
            if (File.Exists(cachedPath))
            {
                // Update timestamp
                PlayerPrefs.SetString(cacheKey + "_timestamp", DateTime.Now.ToString("o"));
                PlayerPrefs.Save();
                
                // Load from disk cache
                // Note: Unity doesn't support loading AudioClip from file directly
                // We'll still need to download it, but we'll update the timestamp
            }
        }
        
        // Download from S3
        string url = s3BaseUrl + audioPath;
        Debug.Log($"🔄 Downloading audio: {url}");
        
        AudioType audioType = audioPath.EndsWith(".mp3") ? AudioType.MPEG : 
                             audioPath.EndsWith(".ogg") ? AudioType.OGGVORBIS : 
                             audioPath.EndsWith(".wav") ? AudioType.WAV : AudioType.UNKNOWN;
        
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
        request.timeout = 30; // Audio có thể lớn hơn
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            
            // Cache in memory
            audioCache[audioPath] = clip;
            
            // Cache to disk (just save the timestamp for tracking)
            string filename = "audio_" + audioPath.GetHashCode() + ".bytes";
            string path = Path.Combine(Application.persistentDataPath, filename);
            
            try
            {
                // Create empty file as a marker
                File.WriteAllText(path, "audio_cache");
                
                PlayerPrefs.SetString(cacheKey, path);
                PlayerPrefs.SetString(cacheKey + "_timestamp", DateTime.Now.ToString("o"));
                PlayerPrefs.Save();
                
                Debug.Log($"💾 Audio cache reference saved: {path}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"⚠️ Failed to create audio cache reference: {e.Message}");
            }
            
            callback?.Invoke(clip);
            Debug.Log($"✅ Audio downloaded: {audioPath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Failed to download audio: {request.error}");
            callback?.Invoke(null);
        }
        
        request.Dispose();
    }
    
    #endregion
    
    #region Cache Management
    
    public void ClearMemoryCache()
    {
        spriteCache.Clear();
        audioCache.Clear();
        System.GC.Collect();
        Debug.Log("🧹 Memory cache cleared");
    }
    
    public void ClearDiskCache()
    {
        StartCoroutine(ClearDiskCacheCoroutine());
    }
    
    private IEnumerator ClearDiskCacheCoroutine()
    {
        string[] cacheFiles = Directory.GetFiles(Application.persistentDataPath, "sprite_*");
        foreach (string file in cacheFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to delete cache file: {e.Message}");
            }
            yield return null;
        }
        
        // Xóa các file audio cache
        string[] audioFiles = Directory.GetFiles(Application.persistentDataPath, "audio_*");
        foreach (string file in audioFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to delete audio cache file: {e.Message}");
            }
            yield return null;
        }
        
        int totalFiles = cacheFiles.Length + audioFiles.Length;
        Debug.Log($"🧹 Disk cache cleared: {totalFiles} files deleted");
    }
    
    // Kiểm tra và xóa cache cũ
    private IEnumerator CleanupOldCache()
    {
        Debug.Log("🔍 Checking cache size and age...");
        
        // Xóa cache của các event không còn active
        if (cleanupUnusedEvents && EventManager.Instance != null)
        {
            string currentEvent = EventManager.Instance.GetCurrentEvent();
            yield return StartCoroutine(CleanupUnusedEventAssets(currentEvent));
            
            // Nếu đã xóa cache của các event không còn active, thì không cần kiểm tra kích thước nữa
            yield break;
        }
        
        // Kiểm tra kích thước cache
        long totalSize = 0;
        int filesDeleted = 0;
        
        // Lấy danh sách tất cả các file cache
        List<CacheFileInfo> cacheFiles = new List<CacheFileInfo>();
        
        // Lấy tất cả các file cache
        string[] allCacheFiles = Directory.GetFiles(Application.persistentDataPath, "*.*");
        foreach (string file in allCacheFiles)
        {
            // Chỉ xử lý các file cache
            if (!file.Contains("sprite_") && !file.Contains("audio_") && !file.Contains("bg_"))
                continue;
                
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                string cacheKey = "";
                
                // Xác định loại cache
                if (file.Contains("sprite_"))
                    cacheKey = "sprite_" + Path.GetFileNameWithoutExtension(file).Replace("sprite_", "");
                else if (file.Contains("audio_"))
                    cacheKey = "audio_" + Path.GetFileNameWithoutExtension(file).Replace("audio_", "");
                else if (file.Contains("bg_"))
                    cacheKey = "bg_cache_" + Path.GetFileNameWithoutExtension(file).Replace("bg_", "");
                
                string timestampStr = PlayerPrefs.GetString(cacheKey + "_timestamp", "");
                DateTime timestamp = string.IsNullOrEmpty(timestampStr) ? 
                    fileInfo.CreationTime : DateTime.Parse(timestampStr);
                    
                cacheFiles.Add(new CacheFileInfo {
                    Path = file,
                    Size = fileInfo.Length,
                    LastAccessed = timestamp,
                    CacheKey = cacheKey
                });
                
                totalSize += fileInfo.Length;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"⚠️ Error processing cache file {file}: {e.Message}");
            }
            
            yield return null;
        }
        
        float totalSizeMB = totalSize / (1024f * 1024f);
        Debug.Log($"💾 Total cache size: {totalSizeMB:F2} MB ({cacheFiles.Count} files)");
        
        // Sắp xếp theo thời gian truy cập (cũ nhất lên đầu)
        cacheFiles.Sort((a, b) => a.LastAccessed.CompareTo(b.LastAccessed));
        
        // Xóa cache cũ, cache của event không còn active, hoặc khi vượt quá kích thước
        foreach (var file in cacheFiles)
        {
            // Kiểm tra file có thuộc về event nào không
            bool isEventAsset = false;
            string fileEvent = "";
            
            // Các event đã biết
            string[] knownEvents = new string[] { "winter", "halloween", "christmas", "summer" };
            string currentEvent = EventManager.Instance != null ? EventManager.Instance.GetCurrentEvent() : "none";
            
            foreach (string eventType in knownEvents)
            {
                if (file.Path.Contains(eventType) && eventType != currentEvent)
                {
                    isEventAsset = true;
                    fileEvent = eventType;
                    break;
                }
            }
            
            // Kiểm tra tuổi của file
            TimeSpan age = DateTime.Now - file.LastAccessed;
            bool isTooOld = age.TotalDays > 30; // 30 ngày là mặc định
            
            // Kiểm tra kích thước cache
            bool needToFreeSpace = totalSizeMB > maxCacheSizeMB;
            
            // Xóa file nếu:
            // 1. Thuộc về event không còn active
            // 2. Hoặc quá cũ
            // 3. Hoặc cần giải phóng bộ nhớ
            if (isEventAsset || isTooOld || needToFreeSpace)
            {
                try
                {
                    File.Delete(file.Path);
                    totalSize -= file.Size;
                    totalSizeMB = totalSize / (1024f * 1024f);
                    filesDeleted++;
                    
                    // Xóa PlayerPrefs entry
                    if (!string.IsNullOrEmpty(file.CacheKey))
                    {
                        PlayerPrefs.DeleteKey(file.CacheKey);
                        PlayerPrefs.DeleteKey(file.CacheKey + "_timestamp");
                    }
                    
                    string reason = isEventAsset ? $"unused event asset ({fileEvent})" :
                               isTooOld ? $"too old ({age.TotalDays:F1} days)" : 
                               "cache size limit";
                    Debug.Log($"🗑️ Deleted cache file: {Path.GetFileName(file.Path)} ({reason})");
                    
                    // Nếu đã giảm xuống dưới ngưỡng, dừng xóa
                    if (totalSizeMB <= maxCacheSizeMB * 0.8f && !isTooOld)
                        break;
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ Failed to delete cache file {file.Path}: {e.Message}");
                }
            }
            
            yield return null;
        }
        
        if (filesDeleted > 0)
        {
            PlayerPrefs.Save();
            Debug.Log($"🧹 Cache cleanup complete: {filesDeleted} files deleted, {totalSizeMB:F2} MB remaining");
        }
        else
        {
            Debug.Log("✅ Cache is clean, no files deleted");
        }
    }
    
    // Class để lưu thông tin về file cache
    private class CacheFileInfo
    {
        public string Path;
        public long Size;
        public DateTime LastAccessed;
        public string CacheKey;
    }
    
    // Xóa cache của các event không còn active
    private IEnumerator CleanupUnusedEventAssets(string currentEvent)
    {
        Debug.Log($"🗑️ Checking for unused event assets (current event: {currentEvent})");
        
        // Danh sách các event
        string[] knownEvents = new string[] { "winter", "halloween", "christmas", "summer" };
        
        // Danh sách các pattern cần xóa cho mỗi event
        Dictionary<string, List<string>> eventPatterns = new Dictionary<string, List<string>>();
        
        // Tạo patterns cho mỗi event
        foreach (string eventType in knownEvents)
        {
            if (eventType == currentEvent) continue; // Bỏ qua event hiện tại
            
            List<string> patterns = new List<string>
            {
                $"fire_{eventType}",
                $"ice_{eventType}",
                $"poison_{eventType}",
                $"rock_{eventType}",
                $"electric_{eventType}",
                $"{eventType}_main",
                $"{eventType}_theme",
                $"{eventType}_background"
            };
            
            eventPatterns[eventType] = patterns;
        }
        
        // Lấy tất cả các file cache
        string[] allFiles = Directory.GetFiles(Application.persistentDataPath);
        int totalDeleted = 0;
        
        // Kiểm tra và xóa các file không còn dùng
        foreach (string file in allFiles)
        {
            // Bỏ qua các file không phải cache
            if (!file.Contains("sprite_") && !file.Contains("audio_") && !file.Contains("bg_"))
                continue;
                
            // Kiểm tra file có thuộc event nào không
            foreach (var eventEntry in eventPatterns)
            {
                string eventType = eventEntry.Key;
                List<string> patterns = eventEntry.Value;
                
                bool shouldDelete = false;
                foreach (string pattern in patterns)
                {
                    // Kiểm tra cả tên file và hash code
                    if (file.Contains(pattern) || file.Contains(pattern.GetHashCode().ToString()))
                    {
                        shouldDelete = true;
                        break;
                    }
                }
                
                if (shouldDelete)
                {
                    try
                    {
                        // Xóa file
                        File.Delete(file);
                        totalDeleted++;
                        Debug.Log($"🗑️ Deleted unused event asset: {Path.GetFileName(file)} (event: {eventType})");
                        
                        // Xóa PlayerPrefs entry
                        string cacheKey = "";
                        if (file.Contains("sprite_"))
                            cacheKey = "sprite_" + Path.GetFileNameWithoutExtension(file).Replace("sprite_", "");
                        else if (file.Contains("audio_"))
                            cacheKey = "audio_" + Path.GetFileNameWithoutExtension(file).Replace("audio_", "");
                        else if (file.Contains("bg_"))
                            cacheKey = "bg_cache_" + Path.GetFileNameWithoutExtension(file).Replace("bg_", "");
                            
                        if (!string.IsNullOrEmpty(cacheKey))
                        {
                            PlayerPrefs.DeleteKey(cacheKey);
                            PlayerPrefs.DeleteKey(cacheKey + "_timestamp");
                        }
                        
                        break; // Thoát khỏi vòng lặp eventPatterns sau khi xóa
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"❌ Failed to delete file {file}: {e.Message}");
                    }
                    
                    break;
                }
            }
            
            yield return null;
        }
        
        if (totalDeleted > 0)
        {
            PlayerPrefs.Save(); // Lưu các thay đổi trong PlayerPrefs
            Debug.Log($"🧹 Total cleaned up: {totalDeleted} files for inactive events");
        }
        else
        {
            Debug.Log("✅ No unused event assets found");
        }
    }
    
    [ContextMenu("Show Cache Files")]
    public void ShowCacheFiles()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath);
        Debug.Log($"Found {files.Length} cache files:");
        foreach (string file in files)
        {
            Debug.Log(Path.GetFileName(file));
        }
    }
    
    [ContextMenu("Force Cleanup Unused Events")]
    public void ForceCleanupUnusedEvents()
    {
        if (EventManager.Instance != null)
        {
            string currentEvent = EventManager.Instance.GetCurrentEvent();
            StartCoroutine(CleanupUnusedEventAssets(currentEvent));
        }
        else
        {
            Debug.LogWarning("⚠️ EventManager not found");
        }
    }
    
    #endregion
    
    #region Helper Methods
    
    public string GetFullUrl(string path)
    {
        return s3BaseUrl + path;
    }
    
    #endregion
}