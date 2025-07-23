using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DynamicBackgroundManager : MonoBehaviour
{
    [Header("Background Components")]
    public Image backgroundImage;
    
    [Header("Default Background")]
    public Sprite defaultBackground;
    
    [Header("S3 Settings")]
    [SerializeField] private string s3BaseUrl = "https://aetheric-game-assets.s3.ap-southeast-2.amazonaws.com";
    
    [Header("Background Paths")]
    [SerializeField] private string winterBackgroundPath = "/backgrounds/winter_main_menu.jpg";
    [SerializeField] private string halloweenBackgroundPath = "/backgrounds/halloween_main_menu.jpg";
    [SerializeField] private string christmasBackgroundPath = "/backgrounds/christmas_main_menu.jpg";
    [SerializeField] private string summerBackgroundPath = "/backgrounds/summer_main_menu.jpg";
    
    [Header("Transition Settings")]
    [SerializeField] private bool useTransition = true;
    [SerializeField] private float transitionSpeed = 2f;
    
    private void Start()
    {
        if (backgroundImage == null)
        {
            Debug.LogError("❌ Background Image not assigned!");
            return;
        }
        
        // Đảm bảo có default background
        if (defaultBackground == null && backgroundImage.sprite != null)
        {
            defaultBackground = backgroundImage.sprite;
        }
        
        // Kiểm tra event và load background phù hợp
        LoadEventBackground();
    }
    
    public void LoadEventBackground()
    {
        if (EventManager.Instance == null)
        {
            Debug.LogWarning("⚠️ EventManager not found, using default background");
            SetDefaultBackground();
            return;
        }
        
        string eventType = EventManager.Instance.GetCurrentEvent();
        
        switch (eventType)
        {
            case "winter":
                StartCoroutine(LoadBackgroundFromS3(winterBackgroundPath));
                break;
            case "halloween":
                StartCoroutine(LoadBackgroundFromS3(halloweenBackgroundPath));
                break;
            case "christmas":
                StartCoroutine(LoadBackgroundFromS3(christmasBackgroundPath));
                break;
            case "summer":
                StartCoroutine(LoadBackgroundFromS3(summerBackgroundPath));
                break;
            default:
                SetDefaultBackground();
                break;
        }
    }
    
    private void SetDefaultBackground()
    {
        if (backgroundImage != null && defaultBackground != null)
        {
            if (useTransition)
                StartCoroutine(TransitionToSprite(defaultBackground));
            else
                backgroundImage.sprite = defaultBackground;
                
            Debug.Log("🎮 Using default background");
        }
    }
    
    private IEnumerator LoadBackgroundFromS3(string backgroundPath)
    {
        Debug.Log($"🔄 Loading background from: {s3BaseUrl}{backgroundPath}");
        
        string fullUrl = s3BaseUrl + backgroundPath;
        
        // Kiểm tra cache
        string cacheKey = "bg_cache_" + backgroundPath.GetHashCode();
        if (PlayerPrefs.HasKey(cacheKey))
        {
            string cachedPath = PlayerPrefs.GetString(cacheKey);
            if (System.IO.File.Exists(cachedPath))
            {
                yield return StartCoroutine(LoadFromCache(cachedPath));
                yield break;
            }
        }
        
        // Download từ S3
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullUrl);
        request.timeout = 10;
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            
            // Tạo sprite từ texture
            Sprite newSprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                Vector2.one * 0.5f
            );
            
            // Cache locally
            StartCoroutine(CacheTextureToFile(texture, backgroundPath, cacheKey));
            
            // Apply sprite
            if (backgroundImage != null)
            {
                if (useTransition)
                    StartCoroutine(TransitionToSprite(newSprite));
                else
                    backgroundImage.sprite = newSprite;
                    
                Debug.Log($"✅ Background loaded successfully!");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Failed to load background: {request.error}");
            SetDefaultBackground();
        }
        
        request.Dispose();
    }
    
    private IEnumerator TransitionToSprite(Sprite newSprite)
    {
        // Fade out
        float alpha = 1f;
        Color originalColor = backgroundImage.color;
        
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * transitionSpeed;
            backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // Change sprite
        backgroundImage.sprite = newSprite;
        
        // Fade in
        while (alpha < 1)
        {
            alpha += Time.deltaTime * transitionSpeed;
            backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // Ensure full opacity
        backgroundImage.color = originalColor;
    }
    
    private IEnumerator CacheTextureToFile(Texture2D texture, string backgroundPath, string cacheKey)
    {
        try
        {
            string filename = "bg_" + backgroundPath.GetHashCode() + ".png";
            string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
            
            byte[] pngData = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, pngData);
            
            PlayerPrefs.SetString(cacheKey, path);
            
            // Lưu thông tin về thời gian cache
            PlayerPrefs.SetString(cacheKey + "_timestamp", System.DateTime.Now.ToString("o"));
            PlayerPrefs.Save();
            
            Debug.Log($"💾 Background cached to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to cache texture: {e.Message}");
        }
        
        yield return null;
    }
    
    private IEnumerator LoadFromCache(string path)
    {
        Debug.Log($"📂 Loading background from cache: {path}");
        
        try
        {
            byte[] fileData = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            
            Sprite cachedSprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                Vector2.one * 0.5f
            );
            
            if (backgroundImage != null)
            {
                if (useTransition)
                    StartCoroutine(TransitionToSprite(cachedSprite));
                else
                    backgroundImage.sprite = cachedSprite;
                    
                Debug.Log("✅ Cached background loaded successfully!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to load cached background: {e.Message}");
            SetDefaultBackground();
        }
        
        yield return null;
    }
    
    // Method để test từ inspector
    [ContextMenu("Force Reload Background")]
    public void ForceReloadBackground()
    {
        LoadEventBackground();
    }
    
    // Xóa cache của background hiện tại
    [ContextMenu("Clear Current Background Cache")]
    public void ClearCurrentBackgroundCache()
    {
        if (EventManager.Instance == null) return;
        
        string eventType = EventManager.Instance.GetCurrentEvent();
        string backgroundPath = "";
        
        switch (eventType)
        {
            case "winter":
                backgroundPath = winterBackgroundPath;
                break;
            case "halloween":
                backgroundPath = halloweenBackgroundPath;
                break;
            case "christmas":
                backgroundPath = christmasBackgroundPath;
                break;
            case "summer":
                backgroundPath = summerBackgroundPath;
                break;
            default:
                Debug.Log("⚠️ No active event to clear cache for");
                return;
        }
        
        string cacheKey = "bg_cache_" + backgroundPath.GetHashCode();
        if (PlayerPrefs.HasKey(cacheKey))
        {
            string cachedPath = PlayerPrefs.GetString(cacheKey);
            if (System.IO.File.Exists(cachedPath))
            {
                try
                {
                    System.IO.File.Delete(cachedPath);
                    Debug.Log($"🗑️ Deleted cached background: {cachedPath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Failed to delete cache file: {e.Message}");
                }
            }
            
            PlayerPrefs.DeleteKey(cacheKey);
            PlayerPrefs.DeleteKey(cacheKey + "_timestamp");
            PlayerPrefs.Save();
        }
        
        // Reload background
        LoadEventBackground();
    }
    
    // Xóa tất cả cache
    [ContextMenu("Clear All Background Cache")]
    public void ClearAllBackgroundCache()
    {
        string[] paths = new string[] {
            winterBackgroundPath,
            halloweenBackgroundPath,
            christmasBackgroundPath,
            summerBackgroundPath
        };
        
        foreach (string path in paths)
        {
            string cacheKey = "bg_cache_" + path.GetHashCode();
            if (PlayerPrefs.HasKey(cacheKey))
            {
                string cachedPath = PlayerPrefs.GetString(cacheKey);
                if (System.IO.File.Exists(cachedPath))
                {
                    try
                    {
                        System.IO.File.Delete(cachedPath);
                        Debug.Log($"🗑️ Deleted cached background: {cachedPath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"❌ Failed to delete cache file: {e.Message}");
                    }
                }
                
                PlayerPrefs.DeleteKey(cacheKey);
                PlayerPrefs.DeleteKey(cacheKey + "_timestamp");
            }
        }
        
        PlayerPrefs.Save();
        Debug.Log("🧹 All background cache cleared!");
        
        // Reload current background
        LoadEventBackground();
    }
}