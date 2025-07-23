using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Doozy.Runtime.UIManager.Components;
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

                    // Load dữ liệu cũ trước, sau đó mới save
                    LambdaGameData lambdaAPI = FindFirstObjectByType<LambdaGameData>();
                    // string gameDataJson = JsonUtility.ToJson(GameManager.Instance.PlayerData);
                    // StartCoroutine(SaveGameDataViaLambda(email, gameDataJson));
                    if (lambdaAPI != null)
                    {
                        StartCoroutine(LoadThenSaveGameData(lambdaAPI, email));
                    }
                }

                if (loginButtonText != null)
                    loginButtonText.text = username;

                if (loginText != null)
                    loginText.text = username;

                if (userInfoToggle != null)
                    userInfoToggle.SetUserButtonToUserPanel();
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

    // Load data trước, sau đó save (cho returning players)
    private IEnumerator LoadThenSaveGameData(LambdaGameData lambdaAPI, string email)
    {
        Debug.Log("🔄 Kiểm tra dữ liệu cũ của player...");
        yield return StartCoroutine(lambdaAPI.LoadGameDataFromServer(email));
        yield return new WaitForSeconds(1f);

        if (GameManager.Instance != null)
        {
            string gameDataJson = JsonUtility.ToJson(GameManager.Instance.PlayerData);
            Debug.Log("📦 Lưu dữ liệu sau khi merge: " + gameDataJson);
            yield return StartCoroutine(lambdaAPI.SaveGameDataViaLambda(email, gameDataJson));
        }

        // Tắt panel sau khi đã hoàn thành
        if (loginPanel != null)
            loginPanel.SetActive(false);
        if (userPanel != null)
            userPanel.SetActive(false);
        if (horizontalLayoutGroup != null)
            horizontalLayoutGroup.SetActive(true);
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
            {
                loginPanel.SetActive(true);
            }

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
}