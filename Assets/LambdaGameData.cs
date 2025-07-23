using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class LambdaGameData : MonoBehaviour
{
    [Header("Mobile Game API Settings")]
    private string apiBaseUrl = "https://5scl4ut173.execute-api.ap-southeast-2.amazonaws.com";
    private string saveEndpoint = "/save-data";
    private string loadEndpoint = "/load-data";

    [System.Serializable]
    public class GameDataRequest
    {
        public string userId;
        public string gameData;
    }
    
    [System.Serializable]
    public class DirectGameDataRequest
    {
        public string userId;
        public PlayerData gameData; // Direct object
    }
    
    [System.Serializable]
    public class LambdaResponse
    {
        public bool success;
        public string message;
        public string error;
        public string userId;
    }
    
    [System.Serializable]
    public class LoadDataResponse
    {
        public bool success;
        public string message;
        public string userId;
        public string lastUpdated;
        // Không include gameData vì JsonUtility không parse được nested object
    }

    public IEnumerator SaveGameDataViaLambda(string userId, string gameData)
    {
        string jsonPayload = JsonUtility.ToJson(new GameDataRequest { userId = userId, gameData = gameData });

        UnityWebRequest request = new UnityWebRequest(apiBaseUrl + saveEndpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Lưu dữ liệu thành công qua Lambda!");
            Debug.Log("📄 Response: " + request.downloadHandler.text);
            
            try
            {
                LambdaResponse response = JsonUtility.FromJson<LambdaResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    Debug.Log($"✅ {response.message}");
                    Debug.Log($"👤 Saved for user: {response.userId}");
                }
                else
                {
                    Debug.LogError($"❌ Lambda Error: {response.error}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Parse response error: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"❌ Failed to save: {request.responseCode} - {request.error}");
            Debug.LogError($"📄 Response: {request.downloadHandler.text}");
        }
        
        request.Dispose();
    }
    
    // Lưu dữ liệu dạng JSON object trực tiếp
    public IEnumerator SaveDirectGameData(string userId, PlayerData playerData)
    {
        string jsonPayload = JsonUtility.ToJson(new DirectGameDataRequest { userId = userId, gameData = playerData });
        
        Debug.Log("📤 Direct JSON payload: " + jsonPayload);

        UnityWebRequest request = new UnityWebRequest(apiBaseUrl + saveEndpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Lưu dữ liệu JSON object thành công!");
            Debug.Log("📄 Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"❌ Failed to save direct: {request.responseCode} - {request.error}");
            Debug.LogError($"📄 Response: {request.downloadHandler.text}");
        }
        
        request.Dispose();
    }
    
    // Load dữ liệu game - xử lý player mới và cũ
    public IEnumerator LoadGameDataFromServer(string userId)
    {
        Debug.Log($"📥 Đang kiểm tra dữ liệu cho user: {userId}");

        string fullUrl = apiBaseUrl + loadEndpoint + "?userId=" + userId;
        UnityWebRequest request = UnityWebRequest.Get(fullUrl);
        request.timeout = 10;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;

            if (responseText.Contains("\"success\":true"))
            {
                Debug.Log("👤 PLAYER CŨ - Tìm thấy dữ liệu trên server");

                // Extract gameData từ response
                int gameDataStart = responseText.IndexOf("\"gameData\":") + 11;
                int gameDataEnd = responseText.IndexOf(",\"lastUpdated\"");

                if (gameDataStart > 10 && gameDataEnd > gameDataStart)
                {
                    string gameDataJson = responseText.Substring(gameDataStart, gameDataEnd - gameDataStart);

                    // Parse và ghi đè lên file local
                    PlayerData serverData = JsonUtility.FromJson<PlayerData>(gameDataJson);
                    if (serverData != null && GameManager.Instance != null)
                    {
                        GameManager.Instance.SetPlayerData(serverData);
                        GameManager.Instance.SavePlayerData(); // Ghi vào playerdata.json

                        Debug.Log($"✅ Đã cập nhật dữ liệu local từ server: {serverData.username}");
                    }
                }
            }
            else
            {
                Debug.Log("🆕 PLAYER MỚI - Không tìm thấy dữ liệu trên server");
                yield return HandleNewPlayer(userId);
            }
        }
        else if (request.responseCode == 404)
        {
            Debug.Log("🆕 PLAYER MỚI - Server trả về 404");
            yield return HandleNewPlayer(userId);
        }
        else
        {
            Debug.LogError($"❌ Lỗi kết nối: {request.responseCode}");
            yield return HandleNewPlayer(userId); // Fallback
        }

        request.Dispose();
    }
    
    // Xử lý player mới - đẩy dữ liệu hiện tại lên server
    private IEnumerator HandleNewPlayer(string userId)
    {
        Debug.Log("🆕 Xử lý player mới...");
        
        if (GameManager.Instance != null)
        {
            PlayerData currentData = GameManager.Instance.PlayerData;
            
            // Đảm bảo có thông tin user
            currentData.email = userId;
            currentData.username = userId.Split('@')[0];
            
            Debug.Log($"📤 Đẩy dữ liệu mới lên server cho: {currentData.username}");
            
            // Lưu lên server
            yield return StartCoroutine(SaveDirectGameData(userId, currentData));
            
            // Lưu local
            GameManager.Instance.SavePlayerData();
            
            Debug.Log("✅ Hoàn thành setup player mới");
        }
    }
    
    // Test method cho direct JSON save
    public void TestSaveDirectData()
    {
        if (GameManager.Instance != null)
        {
            string email = PlayerPrefs.GetString("LoggedEmail", "test@example.com");
            StartCoroutine(SaveDirectGameData(email, GameManager.Instance.PlayerData));
        }
        else
        {
            Debug.LogWarning("⚠️ Không có GameManager!");
        }
    }
    
    // Test network đơn giản
    public void TestSimpleHTTP()
    {
        StartCoroutine(SimpleHTTPTest());
    }
    
    private IEnumerator SimpleHTTPTest()
    {
        Debug.Log("🌐 Test kết nối đơn giản...");
        
        // Test với Google trước
        UnityWebRequest googleTest = UnityWebRequest.Get("https://www.google.com");
        googleTest.timeout = 5;
        
        yield return googleTest.SendWebRequest();
        
        if (googleTest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Kết nối Google thành công - Network OK");
            
            // Test API Gateway
            string testUrl = apiBaseUrl + "/load-data?userId=test";
            Debug.Log("🔄 Test API Gateway: " + testUrl);
            
            UnityWebRequest apiTest = UnityWebRequest.Get(testUrl);
            apiTest.timeout = 10;
            
            yield return apiTest.SendWebRequest();
            
            Debug.Log($"📝 API Test Result: {apiTest.result}");
            Debug.Log($"📝 API Response Code: {apiTest.responseCode}");
            Debug.Log($"📝 API Response: {apiTest.downloadHandler.text}");
            
            apiTest.Dispose();
        }
        else
        {
            Debug.LogError("❌ Không kết nối được Google - Network issue");
        }
        
        googleTest.Dispose();
    }
}