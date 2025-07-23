using UnityEngine;
using TMPro;
using Doozy.Runtime.UIManager.Components;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

[System.Serializable]
public class FeedbackData
{
    public string subject;
    public string user_email;
    public string message;
}

public class UserInfoToggle : MonoBehaviour
{
    [Header("Doozy UIButton")]
    public UIButton userButton;

    [Header("UI Panels")]
    public GameObject horizontalLayout;
    public GameObject userPanel;
    public GameObject loginPanel;

    [Header("User Info Texts")]
    public TMP_Text usernameText;
    public TMP_Text emailText;
    public TMP_Text phoneText;

    [Header("User Button Text")]
    public TMP_Text userButtonText;

    [Header("Back Button")]
    public Button backButton;

    [Header("Logout Button")]
    public Button logoutButton;

    [Header("Feedback UI")]
    public TMP_InputField feedbackInput;
    public Button sendFeedbackButton;

    private bool isLoggedIn = false;

    void Awake()
    {
        string savedUsername = GameManager.Instance?.PlayerData.username;

        if (!string.IsNullOrEmpty(savedUsername) && savedUsername != "Guest")
        {
            isLoggedIn = true;
            if (userButtonText != null)
                userButtonText.text = savedUsername;

            if (userButton != null)
            {
                userButton.onClickEvent.RemoveAllListeners();
                userButton.onClickEvent.AddListener(ToggleUserInfo);
            }
        }
        else
        {
            isLoggedIn = false;
            if (userButtonText != null)
                userButtonText.text = "Login";

            if (userButton != null)
            {
                userButton.onClickEvent.RemoveAllListeners();
                userButton.onClickEvent.AddListener(OpenLoginPanelDirectly);
            }
        }
    }

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);

        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogoutClicked);

        if (sendFeedbackButton != null)
            sendFeedbackButton.onClick.AddListener(SendFeedbackToServer);

        if (horizontalLayout != null) horizontalLayout.SetActive(true);
        if (userPanel != null) userPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(false);
    }

    public void ToggleUserInfo()
    {
        if (!isLoggedIn)
        {
            Debug.LogWarning("⚠️ Chưa đăng nhập, không hiển thị UserPanel!");
            return;
        }

        if (horizontalLayout != null) horizontalLayout.SetActive(false);
        if (userPanel != null) userPanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            var data = GameManager.Instance.PlayerData;

            if (usernameText != null)
                usernameText.text = data.username ?? "Unknown";

            if (emailText != null)
                emailText.text = data.email ?? "no@email.com";

            if (phoneText != null)
                phoneText.text = data.phone ?? "Chưa có số";

            if (feedbackInput != null)
                feedbackInput.text = data.lastFeedback ?? "";
        }
    }

    public void BackToMainMenu()
    {
        if (userPanel != null) userPanel.SetActive(false);
        if (horizontalLayout != null) horizontalLayout.SetActive(true);
    }

    public void OpenLoginPanelDirectly()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (horizontalLayout != null) horizontalLayout.SetActive(false);
        if (userPanel != null) userPanel.SetActive(false);
    }

    public void OnLogoutClicked()
    {
        Debug.Log("🚪 Đăng xuất...");

        GameManager.Instance?.ClearLoginInfo();
        isLoggedIn = false;

        if (userButtonText != null)
            userButtonText.text = "Login";

        if (userButton != null)
        {
            userButton.onClickEvent.RemoveAllListeners();
            userButton.onClickEvent.AddListener(OpenLoginPanelDirectly);
        }

        if (userPanel != null) userPanel.SetActive(false);
        if (horizontalLayout != null) horizontalLayout.SetActive(true);
    }

    public void SetUserButtonToUserPanel()
    {
        isLoggedIn = true;

        if (userButton != null)
        {
            userButton.onClickEvent.RemoveAllListeners();
            userButton.onClickEvent.AddListener(ToggleUserInfo);
        }

        if (userButtonText != null)
            userButtonText.text = GameManager.Instance.PlayerData.username ?? "Login";
    }

    void SendFeedbackToServer()
    {
        string feedback = feedbackInput.text.Trim();
        if (string.IsNullOrEmpty(feedback))
        {
            Debug.LogWarning("⚠️ Feedback rỗng!");
            return;
        }

        // Lưu lại nếu muốn hiển thị lại lần sau
        GameManager.Instance.PlayerData.lastFeedback = feedback;
        GameManager.Instance.SavePlayerData();

        string username = GameManager.Instance.PlayerData.username;
        string email = GameManager.Instance.PlayerData.email;

        FeedbackData data = new FeedbackData
        {
            subject = $"Feedback từ {username}",
            user_email = email,
            message = feedback
        };

        StartCoroutine(SendFeedbackRoutine(data));
    }

    IEnumerator SendFeedbackRoutine(FeedbackData data)
    {
        string json = JsonUtility.ToJson(data);
        string url = "https://n9v8io21z5.execute-api.ap-southeast-2.amazonaws.com/SendFeedbackEmail";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("✅ Gửi phản hồi thành công!");
        else
            Debug.LogError("❌ Gửi phản hồi thất bại: " + request.downloadHandler.text);
    }
}
