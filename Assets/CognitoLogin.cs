using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Doozy.Runtime.UIManager.Components;
using System.Security.Cryptography;
using System;


public class CognitoLogin : MonoBehaviour
{
    EventSystem system;

    [Header("Input Fields")]
    public TMP_InputField mailInput;
    public TMP_InputField passwordInput;

    [Header("Buttons")]
    public Button SubmitButton;
    public Button registerButton;
    public UIButton backButton;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject userPanel;
    public GameObject horizontalLayoutGroup;

    [Header("Login Button Doozy")]
    public UIButton userButton;
    public TMP_Text loginButtonText;

    [Header("AWS Cognito")]
    [SerializeField] private string region = "ap-southeast-2";
    [SerializeField] private string clientId = "5p8jmsjfbflsmvlkuvlm2tf5sj";

    [Header("DynamoDB Settings")]
    [SerializeField] private string identityPoolId = "ap-southeast-2:cb73616f-0288-49a6-a0d2-a8ed98486edc";
    [SerializeField] private string userPoolId = "ap-southeast-2_peDwwenxf";
    [SerializeField] private string dynamoTableName = "Aetheric_PlayerData";

    private string accessToken;
    private string idToken;
    private string identityId;
    private string awsAccessKey;
    private string awsSecretKey;
    private string awsSessionToken;

    [Header("UI Text")]
    public TMP_Text loginText;

    [Header("UserInfoToggle Ref")]
    public UserInfoToggle userInfoToggle;

    private bool isLoggedIn = false;

    void Awake()
    {
        system = EventSystem.current;

        if (PlayerPrefs.GetInt("IsLoggedIn", 0) == 1)
        {
            string savedUsername = PlayerPrefs.GetString("LoggedUsername", "");
            isLoggedIn = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerData.username = savedUsername;
                GameManager.Instance.PlayerData.email = PlayerPrefs.GetString("LoggedEmail", "");
                GameManager.Instance.PlayerData.phone = PlayerPrefs.GetString("LoggedPhone", "");
            }

            if (loginButtonText != null)
                loginButtonText.text = savedUsername;

            if (loginText != null)
                loginText.text = savedUsername;

            if (userInfoToggle != null)
                userInfoToggle.SetUserButtonToUserPanel();
        }
        else
        {
            isLoggedIn = false;

            if (loginButtonText != null)
                loginButtonText.text = "Login";

            if (loginText != null)
                loginText.text = "";
        }
    }

    void Start()
    {
        system = EventSystem.current;

        if (SubmitButton != null)
            SubmitButton.onClick.AddListener(OnLoginClicked);

        if (registerButton != null)
            registerButton.onClick.AddListener(ShowRegisterPanel);

        if (userButton != null)
            userButton.onClickEvent.AddListener(OnUserButtonClicked);

        if (backButton != null)
            backButton.onClickEvent.AddListener(OnBackToHorizontalLayout);

        if (mailInput != null)
            mailInput.Select();

        if (PlayerPrefs.GetInt("IsLoggedIn", 0) == 1)
        {
            string savedUsername = PlayerPrefs.GetString("LoggedUsername", "User");
            isLoggedIn = true;

            if (horizontalLayoutGroup != null)
                horizontalLayoutGroup.SetActive(true);

            if (loginButtonText != null)
                loginButtonText.text = savedUsername;

            if (loginText != null)
                loginText.text = savedUsername;

            if (loginPanel != null)
                loginPanel.SetActive(false);

            if (userPanel != null)
                userPanel.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerData.username = PlayerPrefs.GetString("LoggedUsername", "");
                GameManager.Instance.PlayerData.email = PlayerPrefs.GetString("LoggedEmail", "");
                GameManager.Instance.PlayerData.phone = PlayerPrefs.GetString("LoggedPhone", "");
            }

            if (userInfoToggle != null)
                userInfoToggle.SetUserButtonToUserPanel();
        }
        else
        {
            if (horizontalLayoutGroup != null)
                horizontalLayoutGroup.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            system.currentSelectedGameObject?.GetComponent<Selectable>()?.FindSelectableOnUp()?.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            system.currentSelectedGameObject?.GetComponent<Selectable>()?.FindSelectableOnDown()?.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnLoginClicked();
        }
    }

    void ShowRegisterPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(true);
    }

    void OnLoginClicked()
    {
        if (loginPanel != null && loginPanel.activeInHierarchy)
        {
            StartCoroutine(TryLogin());
        }
    }

    IEnumerator TryLogin()
    {
        string email = mailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ Vui lòng nhập Email và Password");
            yield break;
        }

        string url = $"https://cognito-idp.{region}.amazonaws.com/";

        string jsonPayload = $@"
        {{
            ""AuthParameters"": {{
                ""USERNAME"": ""{email}"",
                ""PASSWORD"": ""{password}""
            }},
            ""AuthFlow"": ""USER_PASSWORD_AUTH"",
            ""ClientId"": ""{clientId}""
        }}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-amz-json-1.1");
        request.SetRequestHeader("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;

            if (responseText.Contains("AuthenticationResult"))
            {
                Debug.Log("✅ Đăng nhập thành công!");

                var authResult = JsonUtility.FromJson<AuthResponse>(responseText);
                accessToken = authResult.AuthenticationResult.AccessToken;
                idToken = authResult.AuthenticationResult.IdToken;

                string username = email.Split('@')[0];
                string phone = "Chưa cập nhật";

                PlayerPrefs.SetString("LoggedUsername", username);
                PlayerPrefs.SetString("LoggedEmail", email);
                PlayerPrefs.SetString("LoggedPhone", phone);
                PlayerPrefs.SetInt("IsLoggedIn", 1);
                Debug.Log(PlayerPrefs.GetString("LoggedEmail", ""));
                PlayerPrefs.Save();

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerData.username = username;
                    GameManager.Instance.PlayerData.email = email;
                    GameManager.Instance.PlayerData.phone = phone;
                    GameManager.Instance.SavePlayerData();

                    // Lưu dữ liệu lên cloud
                    string gameDataJson = JsonUtility.ToJson(GameManager.Instance.PlayerData);
                    Debug.Log("📦 Dữ liệu game: " + gameDataJson);
                    StartCoroutine(SaveGameDataToCloud(email, gameDataJson));
                }

                if (loginButtonText != null)
                    loginButtonText.text = username;

                if (loginText != null)
                    loginText.text = username;

                if (userInfoToggle != null)
                    userInfoToggle.SetUserButtonToUserPanel();

                if (horizontalLayoutGroup != null)
                    horizontalLayoutGroup.SetActive(true);

                if (loginPanel != null)
                    loginPanel.SetActive(false);

                if (userPanel != null)
                    userPanel.SetActive(false);

                isLoggedIn = true;
            }
            else
            {
                Debug.LogError("❌ Sai thông tin đăng nhập!");
            }
        }
        else
        {
            Debug.LogError("❌ Lỗi đăng nhập: " + request.downloadHandler.text);
        }

        request.Dispose();
    }

    void OnUserButtonClicked()
    {
        if (isLoggedIn)
        {
            if (horizontalLayoutGroup != null)
                horizontalLayoutGroup.SetActive(false);

            if (loginPanel != null)
                loginPanel.SetActive(false);

            if (userPanel != null)
                userPanel.SetActive(true);
        }
        else
        {
            if (loginPanel != null)
                loginPanel.SetActive(true);

            if (horizontalLayoutGroup != null)
                horizontalLayoutGroup.SetActive(false);

            if (userPanel != null)
                userPanel.SetActive(false);
        }
    }

    void OnBackToHorizontalLayout()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (horizontalLayoutGroup != null)
            horizontalLayoutGroup.SetActive(true);
    }

    // Classes để parse JSON response
    [System.Serializable]
    public class AuthResponse
    {
        public AuthenticationResult AuthenticationResult;
    }

    [System.Serializable]
    public class AuthenticationResult
    {
        public string AccessToken;
        public string IdToken;
    }

    [System.Serializable]
    public class GetIdResponse
    {
        public string IdentityId;
    }

    [System.Serializable]
    public class CredentialsResponse
    {
        public Credentials Credentials;
    }

    [System.Serializable]
    public class Credentials
    {
        public string AccessKeyId;
        public string SecretKey;
        public string SessionToken;
    }

    // Lưu dữ liệu game lên DynamoDB với IdToken
    private IEnumerator SaveGameDataToCloud(string userId, string gameData)
    {
        Debug.Log("🔄 Bắt đầu lưu dữ liệu lên cloud...");
        Debug.Log($"📦 UserId: {userId}");
        Debug.Log($"📦 GameData: {gameData}");
        Debug.Log($"📦 IdToken có: {!string.IsNullOrEmpty(idToken)}");

        // TẠMTHỜI BỎ QUA AWS CALLS ĐỂ TEST
        Debug.Log("⚠️ Tạm thời bỏ qua AWS calls để test...");

        yield return new WaitForSeconds(1f);

        Debug.Log("✅ Giả lập lưu dữ liệu thành công!");

        /* AWS CALLS - SẼ ENABLE LẠI SAU
        if (string.IsNullOrEmpty(idToken))
        {
            Debug.LogError("❌ Không có IdToken!");
            yield break;
        }

        // Bước 1: Lấy Identity ID
        yield return StartCoroutine(GetIdentityId());
        if (string.IsNullOrEmpty(identityId))
        {
            Debug.LogError("❌ Không lấy được Identity ID!");
            yield break;
        }

        // Bước 2: Lấy temporary credentials
        yield return StartCoroutine(GetTemporaryCredentials());
        if (string.IsNullOrEmpty(awsAccessKey))
        {
            Debug.LogError("❌ Không lấy được AWS credentials!");
            yield break;
        }

        // Bước 3: Lưu dữ liệu lên DynamoDB
        yield return StartCoroutine(PutItemToDynamoDB(userId, gameData));
        */
    }

    // Bước 1: Lấy Identity ID từ Cognito Identity Pool
    public IEnumerator GetIdentityId()
    {
        if (string.IsNullOrEmpty(idToken))
        {
            Debug.LogError("❌ Không có IdToken!");
            yield break;
        }

        Debug.Log("🔄 Bắt đầu lấy Identity ID...");
        string url = $"https://cognito-identity.{region}.amazonaws.com/";
        string loginkey = $"cognito-idp.{region}.amazonaws.com/{userPoolId}";

        // Tạo payload JSON sạch hơn
        string payload = $@"{{
        ""IdentityPoolId"":""{identityPoolId}"",
        ""Logins"":{{
        ""{loginkey}"":""{idToken}""
        }}
        }}";

        Debug.Log("📤 URL: " + url);
        Debug.Log("📤 Payload: " + payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-amz-json-1.1");
        request.SetRequestHeader("X-Amz-Target", "AWSCognitoIdentityService.GetId");
        request.timeout = 15;

        Debug.Log("🔄 Đang gửi request...");

        // float startTime = Time.time;
        yield return request.SendWebRequest();
        float endTime = Time.time;

        // Debug.Log($"⏱️ Request mất {endTime - startTime:F2} giây");
        Debug.Log($"🔍 Request result: {request.result}");
        Debug.Log($"🔍 Response code: {request.responseCode}");
        Debug.Log($"🔍 Response text: {request.downloadHandler.text}");

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var response = JsonUtility.FromJson<GetIdResponse>(request.downloadHandler.text);
                identityId = response.IdentityId;
                Debug.Log("✅ Đã lấy Identity ID: " + identityId);
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ Lỗi parse JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError($"❌ Lỗi lấy Identity ID: {request.error}");
            Debug.LogError($"❌ Response: {request.downloadHandler.text}");
        }

        request.Dispose();
    }

    // Bước 2: Lấy temporary credentials
    private IEnumerator GetTemporaryCredentials()
    {
        Debug.Log("🔑 Bắt đầu lấy temporary credentials...");

        string url = $"https://cognito-identity.{region}.amazonaws.com/";
        string loginKey = $"cognito-idp.{region}.amazonaws.com/{userPoolId}";

        string payload = $@"{{
""IdentityId"":""{identityId}"",
""Logins"":{{
""{loginKey}"":""{idToken}""
}}
}}";

        Debug.Log("📤 Credentials payload: " + payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-amz-json-1.1");
        request.SetRequestHeader("X-Amz-Target", "AWSCognitoIdentityService.GetCredentialsForIdentity");
        request.timeout = 15;

        Debug.Log("🔄 Đang gửi credentials request...");

        float startTime = Time.time;
        yield return request.SendWebRequest();
        float endTime = Time.time;

        Debug.Log($"⏱️ Credentials request mất {endTime - startTime:F2} giây");
        Debug.Log($"🔍 Credentials result: {request.result}");

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                Debug.Log("📄 Credentials response: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<CredentialsResponse>(request.downloadHandler.text);
                awsAccessKey = response.Credentials.AccessKeyId;
                awsSecretKey = response.Credentials.SecretKey;
                awsSessionToken = response.Credentials.SessionToken;
                Debug.Log("✅ Đã lấy temporary credentials thành công");
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ Lỗi parse credentials JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError($"❌ Lỗi lấy credentials: {request.error}");
            Debug.LogError($"❌ Credentials response: {request.downloadHandler.text}");
        }

        request.Dispose();
    }

    // Bước 3: Lưu dữ liệu lên DynamoDB với AWS Signature V4
    private IEnumerator PutItemToDynamoDB(string userId, string gameData)
    {
        Debug.Log("💾 Lưu dữ liệu lên DynamoDB...");

        string host = $"dynamodb.{region}.amazonaws.com";
        string url = $"https://{host}/";

        string payload = $@"
        {{
            ""TableName"": ""{dynamoTableName}"",
            ""Item"": {{
                ""userId"": {{""S"": ""{userId}""}},
                ""gameData"": {{""S"": ""{gameData}""}},
                ""lastUpdated"": {{""S"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}""}}
            }}
        }}";

        // Tạo AWS Signature V4
        string dateTime = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
        string date = DateTime.UtcNow.ToString("yyyyMMdd");

        // Tạo canonical request
        string canonicalHeaders = $"host:{host}\nx-amz-date:{dateTime}\nx-amz-security-token:{awsSessionToken}\nx-amz-target:DynamoDB_20120810.PutItem\n";
        string signedHeaders = "host;x-amz-date;x-amz-security-token;x-amz-target";

        string payloadHash = ComputeSHA256Hash(payload);
        string canonicalRequest = $"POST\n/\n\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

        // Tạo string to sign
        string credentialScope = $"{date}/{region}/dynamodb/aws4_request";
        string stringToSign = $"AWS4-HMAC-SHA256\n{dateTime}\n{credentialScope}\n{ComputeSHA256Hash(canonicalRequest)}";

        // Tạo signing key
        byte[] signingKey = GetSigningKey(awsSecretKey, date, region, "dynamodb");
        string signature = ComputeHMACSHA256(stringToSign, signingKey);

        // Tạo authorization header
        string authHeader = $"AWS4-HMAC-SHA256 Credential={awsAccessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/x-amz-json-1.0");
        request.SetRequestHeader("X-Amz-Target", "DynamoDB_20120810.PutItem");
        request.SetRequestHeader("Host", host);
        request.SetRequestHeader("X-Amz-Date", dateTime);
        request.SetRequestHeader("X-Amz-Security-Token", awsSessionToken);
        request.SetRequestHeader("Authorization", authHeader);
        request.timeout = 15;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Đã lưu dữ liệu lên DynamoDB thành công!");
            Debug.Log("📄 Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Lỗi lưu dữ liệu: " + request.downloadHandler.text);
        }

        request.Dispose();
    }

    // Helper methods cho AWS Signature V4
    private string ComputeSHA256Hash(string input)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    private byte[] GetSigningKey(string secretKey, string date, string region, string service)
    {
        byte[] kDate = ComputeHMACSHA256Bytes("AWS4" + secretKey, date);
        byte[] kRegion = ComputeHMACSHA256Bytes(kDate, region);
        byte[] kService = ComputeHMACSHA256Bytes(kRegion, service);
        return ComputeHMACSHA256Bytes(kService, "aws4_request");
    }

    private string ComputeHMACSHA256(string data, byte[] key)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA256(key))
        {
            byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    private byte[] ComputeHMACSHA256Bytes(string data, string key)
    {
        return ComputeHMACSHA256Bytes(Encoding.UTF8.GetBytes(key), data);
    }

    private byte[] ComputeHMACSHA256Bytes(byte[] key, string data)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA256(key))
        {
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }
    }
}