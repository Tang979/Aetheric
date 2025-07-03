using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Doozy.Runtime.UIManager.Components;

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
    public string region = "ap-southeast-2";
    public string clientId = "1iv1l3avi8ah3gn2b3ua03i81v";

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
            // ✅ Người dùng đã đăng nhập trước đó
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
        else
            Debug.LogWarning("⚠️ backButton chưa được gán!");

        if (mailInput != null)
            mailInput.Select();

        // ✅ Tự động xử lý khi đã đăng nhập trước đó
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

            // Gán lại data cho GameManager nếu cần
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerData.username = PlayerPrefs.GetString("LoggedUsername", "");
                GameManager.Instance.PlayerData.email = PlayerPrefs.GetString("LoggedEmail", "");
                GameManager.Instance.PlayerData.phone = PlayerPrefs.GetString("LoggedPhone", "");
            }

            // ⚠️ Gọi SetUserButtonToUserPanel để set behavior mới cho user button
            if (userInfoToggle != null)
            {
                userInfoToggle.SetUserButtonToUserPanel();
            }
            else
            {
                Debug.LogWarning("⚠️ userInfoToggle chưa được gán!");
            }
        }
        else
        {
            // Nếu chưa login, thì ẩn HorizontalLayout và hiện loginPanel nếu muốn
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
        else
        {
            Debug.LogWarning("⚠️ loginPanel đang bị ẩn hoặc null!");
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

                string username = email.Split('@')[0];
                string phone = "Chưa cập nhật";

                PlayerPrefs.SetString("LoggedUsername", username);
                PlayerPrefs.SetString("LoggedEmail", email);
                PlayerPrefs.SetString("LoggedPhone", phone);
                PlayerPrefs.SetInt("IsLoggedIn", 1);
                PlayerPrefs.Save();

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerData.username = username;
                    GameManager.Instance.PlayerData.email = email;
                    GameManager.Instance.PlayerData.phone = phone;
                    GameManager.Instance.SavePlayerData();
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
                Debug.Log("Phản hồi: " + responseText);
            }
        }
        else
        {
            Debug.LogError("❌ Lỗi đăng nhập: " + request.downloadHandler.text);
        }
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
        Debug.Log("🔙 Quay lại HorizontalLayout từ loginPanel");

        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (horizontalLayoutGroup != null)
            horizontalLayoutGroup.SetActive(true);
    }
}
