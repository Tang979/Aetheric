using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class SimpleLambdaAPI : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiGatewayUrl = "https://YOUR_API_ID.execute-api.ap-southeast-2.amazonaws.com/prod/save-game-data";
    
    [System.Serializable]
    public class GameDataPayload
    {
        public string userId;
        public string gameData;
    }
    
    [System.Serializable]
    public class APIResponse
    {
        public bool success;
        public string message;
        public string error;
    }
    
    // Lưu dữ liệu game qua Lambda API
    public IEnumerator SaveGameDataToCloud(string userId, string gameData)
    {
        Debug.Log("🔄 Bắt đầu lưu dữ liệu qua Lambda API...");
        
        // Tạo payload
        GameDataPayload payload = new GameDataPayload
        {
            userId = userId,
            gameData = gameData
        };
        
        string jsonPayload = JsonUtility.ToJson(payload);
        Debug.Log("📤 Payload: " + jsonPayload);
        
        UnityWebRequest request = new UnityWebRequest(apiGatewayUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 10;
        
        Debug.Log("🔄 Đang gửi request đến Lambda...");
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Lưu dữ liệu thành công qua Lambda!");
            Debug.Log("📥 Response: " + request.downloadHandler.text);
            
            try
            {
                APIResponse response = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    Debug.Log("✅ " + response.message);
                }
                else
                {
                    Debug.LogError("❌ API Error: " + response.error);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("⚠️ Không parse được response JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("❌ Lỗi lưu dữ liệu: " + request.downloadHandler.text);
        }
        
        request.Dispose();
    }
    
    // Tải dữ liệu game từ Lambda API
    public IEnumerator LoadGameDataFromCloud(string userId)
    {
        Debug.Log("🔄 Bắt đầu tải dữ liệu từ Lambda API...");
        
        string loadUrl = apiGatewayUrl.Replace("save-game-data", "load-game-data");
        string requestUrl = $"{loadUrl}?userId={userId}";
        
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        request.timeout = 10;
        
        Debug.Log("🔄 Đang tải dữ liệu từ Lambda...");
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Tải dữ liệu thành công từ Lambda!");
            Debug.Log("📥 Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Lỗi tải dữ liệu: " + request.downloadHandler.text);
        }
        
        request.Dispose();
    }
    
    // Test methods
    public void TestSaveData()
    {
        string testUserId = "test@example.com";
        string testGameData = "{\"level\":5,\"score\":1000,\"username\":\"testuser\"}";
        
        StartCoroutine(SaveGameDataToCloud(testUserId, testGameData));
    }
    
    public void TestLoadData()
    {
        string testUserId = "test@example.com";
        StartCoroutine(LoadGameDataFromCloud(testUserId));
    }
}