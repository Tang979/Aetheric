using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class EventConfig
{
    public string currentEvent = "none";
    public string eventStartDate = "";
    public string eventEndDate = "";
    public string eventMessage = "";
    public bool forceUpdate = false;
    public string[] eventAssets = new string[0];
}

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Event Control")]
    public EventControlMode controlMode = EventControlMode.Hybrid;
    
    [Header("Manual Override")]
    [Tooltip("Chỉ áp dụng khi controlMode = Manual")]
    public string manualEvent = "none";
    
    [Header("Development Settings")]
    public bool developmentMode = true;
    public string testEvent = "winter";
    
    [Header("Server Config")]
    [SerializeField] private string configUrl = "https://aetheric-game-assets.s3.ap-southeast-2.amazonaws.com/config/events.json";
    [SerializeField] private float configRefreshTime = 3600f; // 1 giờ
    
    [Header("Cache Settings")]
    [SerializeField] private bool cleanupExpiredEventAssets = true;
    [SerializeField] private bool immediateCleanupWhenEventEnds = true; // Xóa ngay khi event kết thúc
    
    private EventConfig serverConfig;
    private DateTime lastConfigCheck;
    
    public enum EventControlMode
    {
        Manual,     // Từ inspector
        Server,     // Từ S3 config
        TimeBased,  // Theo thời gian
        Hybrid      // Server + time fallback
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            lastConfigCheck = DateTime.MinValue;
            serverConfig = new EventConfig();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Lưu event trước đó
        string previousEvent = PlayerPrefs.GetString("LastActiveEvent", "none");
        
        // Load config mới
        StartCoroutine(LoadEventConfig());
        
        // Kiểm tra và xóa cache cũ
        if (cleanupExpiredEventAssets)
        {
            StartCoroutine(CleanupExpiredEventAssets(previousEvent));
        }
    }
    
    public string GetCurrentEvent()
    {
        // Development mode override
        if (developmentMode)
            return testEvent;
            
        // Kiểm tra theo control mode
        string eventType;
        switch (controlMode)
        {
            case EventControlMode.Manual:
                eventType = manualEvent;
                break;
                
            case EventControlMode.Server:
                eventType = GetServerEvent();
                break;
                
            case EventControlMode.TimeBased:
                eventType = GetTimeBasedEvent();
                break;
                
            case EventControlMode.Hybrid:
                string serverEvent = GetServerEvent();
                eventType = string.IsNullOrEmpty(serverEvent) || serverEvent == "none" 
                    ? GetTimeBasedEvent() 
                    : serverEvent;
                break;
                
            default:
                eventType = "none";
                break;
        }
        
        // Lưu thông tin về event hiện tại
        if (eventType != "none")
        {
            SaveCurrentEventInfo(eventType);
        }
        
        return eventType;
    }
    
    private string GetServerEvent()
    {
        // Refresh config nếu quá cũ
        if ((DateTime.Now - lastConfigCheck).TotalSeconds > configRefreshTime)
        {
            StartCoroutine(LoadEventConfig());
        }
        
        return serverConfig?.currentEvent ?? "none";
    }
    
    private string GetTimeBasedEvent()
    {
        DateTime now = DateTime.Now;
        
        // Christmas Event: 15/12 - 15/1
        if ((now.Month == 12 && now.Day >= 15) || (now.Month == 1 && now.Day <= 15))
            return "christmas";
            
        // Halloween Event: 15/10 - 5/11
        if ((now.Month == 10 && now.Day >= 15) || (now.Month == 11 && now.Day <= 5))
            return "halloween";
            
        // Summer Event: 15/6 - 15/8
        if ((now.Month >= 6 && now.Month <= 8) && 
            (now.Month != 6 || now.Day >= 15) && 
            (now.Month != 8 || now.Day <= 15))
            return "summer";
            
        // Winter Event: 15/1 - 15/3
        if ((now.Month >= 1 && now.Month <= 3) && 
            (now.Month != 1 || now.Day >= 15) && 
            (now.Month != 3 || now.Day <= 15))
            return "winter";
            
        return "none";
    }
    
    public IEnumerator LoadEventConfig()
    {
        Debug.Log("🔄 Checking for event updates...");
        
        UnityWebRequest request = UnityWebRequest.Get(configUrl);
        request.timeout = 10;
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                serverConfig = JsonUtility.FromJson<EventConfig>(request.downloadHandler.text);
                lastConfigCheck = DateTime.Now;
                
                Debug.Log($"✅ Event config loaded: {serverConfig.currentEvent}");
                
                if (!string.IsNullOrEmpty(serverConfig.eventMessage))
                    Debug.Log($"📢 Event message: {serverConfig.eventMessage}");
                    
                // Cache config locally
                PlayerPrefs.SetString("LastEventConfig", request.downloadHandler.text);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Error parsing event config: {e.Message}");
                LoadCachedConfig();
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Failed to load event config: {request.error}");
            LoadCachedConfig();
        }
        
        request.Dispose();
    }
    
    private void LoadCachedConfig()
    {
        string cachedConfig = PlayerPrefs.GetString("LastEventConfig", "");
        if (!string.IsNullOrEmpty(cachedConfig))
        {
            try
            {
                serverConfig = JsonUtility.FromJson<EventConfig>(cachedConfig);
                Debug.Log("📋 Using cached event config");
            }
            catch
            {
                serverConfig = new EventConfig();
                Debug.LogWarning("⚠️ Failed to load cached config");
            }
        }
    }
    
    public bool IsEventActive(string eventName)
    {
        return GetCurrentEvent() == eventName;
    }
    
    public string[] GetEventAssets()
    {
        return serverConfig?.eventAssets ?? new string[0];
    }
    
    // Debug methods
    [ContextMenu("Force Reload Config")]
    public void ForceReloadConfig()
    {
        StartCoroutine(LoadEventConfig());
    }
    
    [ContextMenu("Activate Winter Event")]
    public void ActivateWinterEvent()
    {
        developmentMode = true;
        testEvent = "winter";
        Debug.Log("❄️ Winter event activated (Development mode)");
    }
    
    [ContextMenu("Activate Halloween Event")]
    public void ActivateHalloweenEvent()
    {
        developmentMode = true;
        testEvent = "halloween";
        Debug.Log("🎃 Halloween event activated (Development mode)");
    }
    
    [ContextMenu("Activate Christmas Event")]
    public void ActivateChristmasEvent()
    {
        developmentMode = true;
        testEvent = "christmas";
        Debug.Log("🎄 Christmas event activated (Development mode)");
    }
    
    [ContextMenu("Deactivate All Events")]
    public void DeactivateEvents()
    {
        developmentMode = true;
        testEvent = "none";
        Debug.Log("🚫 All events deactivated (Development mode)");
    }
    
    [ContextMenu("Switch to Production Mode")]
    public void SwitchToProductionMode()
    {
        developmentMode = false;
        Debug.Log("🚀 Switched to production mode");
    }
    
    #region Cache Management
    
    // Lưu thông tin về event hiện tại
    private void SaveCurrentEventInfo(string eventType)
    {
        // Lưu event hiện tại
        PlayerPrefs.SetString("LastActiveEvent", eventType);
        
        // Lưu thời gian bắt đầu event
        if (PlayerPrefs.GetString("Event_" + eventType + "_StartTime", "") == "")
        {
            PlayerPrefs.SetString("Event_" + eventType + "_StartTime", System.DateTime.Now.ToString("o"));
        }
        
        // Lưu thời gian cuối cùng sử dụng
        PlayerPrefs.SetString("Event_" + eventType + "_LastUsed", System.DateTime.Now.ToString("o"));
        PlayerPrefs.Save();
    }
    
    // Kiểm tra và xóa cache của các event đã hết hạn
    private IEnumerator CleanupExpiredEventAssets(string previousEvent)
    {
        Debug.Log("🧹 Checking for expired event assets...");
        
        // Lấy event hiện tại
        string currentEvent = GetCurrentEvent();
        SaveCurrentEventInfo(currentEvent);
        
        // Danh sách các event đã biết
        string[] knownEvents = new string[] { "winter", "halloween", "christmas", "summer" };
        
        // Nếu event đã thay đổi, xóa cache của event cũ
        if (immediateCleanupWhenEventEnds && previousEvent != "none" && previousEvent != currentEvent)
        {
            Debug.Log($"🗑️ Event changed from {previousEvent} to {currentEvent} - cleaning up previous event assets");
            yield return StartCoroutine(CleanupEventAssets(previousEvent));
        }
        
        // Xóa cache của các event khác
        foreach (string eventType in knownEvents)
        {
            // Bỏ qua event hiện tại
            if (eventType == currentEvent) continue;
            
            // Nếu đã xóa event trước đó rồi thì bỏ qua
            if (eventType == previousEvent && immediateCleanupWhenEventEnds) continue;
            
            // Xóa cache của event này
            Debug.Log($"🗑️ Cleaning up assets for inactive event: {eventType}");
            yield return StartCoroutine(CleanupEventAssets(eventType));
            
            yield return null;
        }
    }
    
    // Xóa cache của một event cụ thể
    private IEnumerator CleanupEventAssets(string eventType)
    {
        // Xóa background cache
        string[] backgroundPaths = new string[] {
            "/backgrounds/" + eventType + "_main_menu.jpg",
            "/backgrounds/" + eventType + "_background.jpg"
        };
        
        foreach (string path in backgroundPaths)
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
                        Debug.Log($"🗑️ Deleted cached file: {cachedPath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"❌ Failed to delete cache file: {e.Message}");
                    }
                }
                
                PlayerPrefs.DeleteKey(cacheKey);
            }
            
            yield return null;
        }
        
        // Xóa tower skin cache
        string[] towerTypes = new string[] { "fire", "ice", "poison", "rock", "electric" };
        foreach (string tower in towerTypes)
        {
            string skinPath = $"/sprites/tower_skins/{tower}_{eventType}.png";
            string cacheKey = "sprite_" + skinPath.GetHashCode();
            
            if (PlayerPrefs.HasKey(cacheKey))
            {
                string cachedPath = PlayerPrefs.GetString(cacheKey);
                if (System.IO.File.Exists(cachedPath))
                {
                    try
                    {
                        System.IO.File.Delete(cachedPath);
                        Debug.Log($"🗑️ Deleted cached tower skin: {cachedPath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"❌ Failed to delete cache file: {e.Message}");
                    }
                }
                
                PlayerPrefs.DeleteKey(cacheKey);
            }
            
            yield return null;
        }
        
        // Xóa audio cache
        string[] audioPaths = new string[] {
            $"/audio/music/{eventType}_theme.mp3",
            $"/audio/sfx/{eventType}_effect.mp3"
        };
        
        foreach (string path in audioPaths)
        {
            string cacheKey = "audio_" + path.GetHashCode();
            if (PlayerPrefs.HasKey(cacheKey))
            {
                string cachedPath = PlayerPrefs.GetString(cacheKey);
                if (System.IO.File.Exists(cachedPath))
                {
                    try
                    {
                        System.IO.File.Delete(cachedPath);
                        Debug.Log($"🗑️ Deleted cached audio: {cachedPath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"❌ Failed to delete cache file: {e.Message}");
                    }
                }
                
                PlayerPrefs.DeleteKey(cacheKey);
            }
            
            yield return null;
        }
        
        // Xóa thông tin event
        PlayerPrefs.DeleteKey("Event_" + eventType + "_StartTime");
        PlayerPrefs.DeleteKey("Event_" + eventType + "_LastUsed");
        PlayerPrefs.Save();
        
        Debug.Log($"✅ Cleanup completed for event: {eventType}");
    }
    
    [ContextMenu("Force Cleanup All Event Assets")]
    public void ForceCleanupAllEventAssets()
    {
        StartCoroutine(CleanupAllEventAssets());
    }
    
    private IEnumerator CleanupAllEventAssets()
    {
        string[] events = new string[] { "winter", "halloween", "christmas", "summer" };
        
        foreach (string eventType in events)
        {
            yield return StartCoroutine(CleanupEventAssets(eventType));
        }
        
        Debug.Log("🧹 All event assets cleaned up!");
    }
    
    [ContextMenu("Test Event Change")]
    public void TestEventChange()
    {
        if (developmentMode)
        {
            string previousEvent = PlayerPrefs.GetString("LastActiveEvent", "none");
            string newEvent = previousEvent == "winter" ? "halloween" : 
                             previousEvent == "halloween" ? "christmas" : 
                             previousEvent == "christmas" ? "summer" : "winter";
            
            Debug.Log($"🔄 Testing event change from {previousEvent} to {newEvent}");
            
            // Lưu event cũ
            string oldEvent = testEvent;
            
            // Set event mới
            testEvent = newEvent;
            
            // Giả lập event change
            StartCoroutine(CleanupExpiredEventAssets(oldEvent));
            
            // Lưu event mới
            PlayerPrefs.SetString("LastActiveEvent", newEvent);
            PlayerPrefs.Save();
            
            Debug.Log("💯 Event changed, check console for cleanup logs");
        }
        else
        {
            Debug.LogWarning("⚠️ Enable developmentMode to test event changes");
        }
    }
    
    #endregion
}