using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Doozy.Runtime.UIManager.Containers;

public class CognitoLogin : MonoBehaviour
{
    EventSystem system;

    [Header("Input Fields")]
    public TMP_InputField mailInput;
    public TMP_InputField passwordInput;

    [Header("Buttons")]
    public Button SubmitButton;
    public Button registerButton;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Doozy View")]
    public UIView loginView;        // Gán View - Login
    public UIView mainMenuView;     // Gán View - MainMenu

    [Header("UI Text")]
    public TMP_Text loginText;      // Text để đổi tên "Login" thành email/user

    [Header("AWS Cognito")]
    public string region = "ap-southeast-2";
    public string clientId = "1iv1l3avi8ah3gn2b3ua03i81v";

    void Start()
    {
        system = EventSystem.current;

        if (SubmitButton != null)
            SubmitButton.onClick.AddListener(OnLoginClicked);
        else
            Debug.LogWarning("❗ Chưa gán SubmitButton");

        if (registerButton != null)
            registerButton.onClick.AddListener(ShowRegisterPanel);
        else
            Debug.LogWarning("❗ Chưa gán registerButton");

        if (mailInput != null)
            mailInput.Select();
        else
            Debug.LogWarning("❗ Chưa gán mailInput");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            Selectable previous = system.currentSelectedGameObject?.GetComponent<Selectable>()?.FindSelectableOnUp();
            previous?.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject?.GetComponent<Selectable>()?.FindSelectableOnDown();
            next?.Select();
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
        Debug.Log("👉 Chuyển sang panel đăng ký");
    }

    void OnLoginClicked()
    {
        StartCoroutine(TryLogin());
    }

    IEnumerator TryLogin()
    {
        string email = mailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ Vui lòng nhập đầy đủ Email và Password");
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
            Debug.Log("✅ Đăng nhập thành công!");

            // Ẩn panel và View login
            if (loginPanel != null) loginPanel.SetActive(false);
            if (loginView != null) loginView.Hide();

            // Hiện Main Menu View
            if (mainMenuView != null)
            {
                Debug.Log("👉 Hiện View - MainMenu");
                mainMenuView.Show();
            }
            else
            {
                Debug.LogWarning("⚠️ Chưa gán UIView - MainMenu");
            }

            // Cập nhật tên user (ví dụ từ email)
            if (loginText != null)
            {
                string username = email.Split('@')[0];
                loginText.text = username;
            }
            else
            {
                Debug.LogWarning("⚠️ Chưa gán TMP_Text loginText");
            }
        }
        else
        {
            Debug.LogError("❌ Đăng nhập thất bại: " + request.downloadHandler.text);
        }
    }
}
