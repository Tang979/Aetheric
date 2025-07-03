using UnityEngine;
using TMPro;
using Doozy.Runtime.UIManager.Components;
using UnityEngine.UI;

public class UserInfoToggle : MonoBehaviour
{
    [Header("Doozy UIButton")]
    public UIButton userButton; // Nút tên người dùng (hoặc Login)

    [Header("UI Panels")]
    public GameObject horizontalLayout; // Layout chứa các nút chính
    public GameObject userPanel;        // Panel thông tin người dùng
    public GameObject loginPanel;       // Panel login (hiện lại khi logout)

    [Header("User Info Texts")]
    public TMP_Text usernameText;
    public TMP_Text emailText;
    public TMP_Text phoneText;

    [Header("User Button Text")]
    public TMP_Text userButtonText; // Text của UIButton (Login hoặc Username)

    [Header("Back Button")]
    public Button backButton; // Nút Back trong userPanel

    [Header("Logout Button")]
    public Button logoutButton; // Nút Logout

    private bool isLoggedIn = false;

    void Awake()
    {
        // Gán sẵn callback tùy theo trạng thái đăng nhập
        string savedUsername = PlayerPrefs.GetString("LoggedUsername", "");

        if (!string.IsNullOrEmpty(savedUsername))
        {
            // ✅ Đã đăng nhập → gán hiển thị userPanel
            isLoggedIn = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerData.username = savedUsername;
                GameManager.Instance.PlayerData.email = PlayerPrefs.GetString("LoggedEmail", "");
                GameManager.Instance.PlayerData.phone = PlayerPrefs.GetString("LoggedPhone", "");
            }

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
            // ❌ Chưa login → gán để mở loginPanel
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
        // Gán nút Back và Logout
        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);

        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogoutClicked);

        // Mặc định
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
            if (usernameText != null)
                usernameText.text = GameManager.Instance.PlayerData.username ?? "Unknown";

            if (emailText != null)
                emailText.text = GameManager.Instance.PlayerData.email ?? "no@email.com";

            if (phoneText != null)
                phoneText.text = GameManager.Instance.PlayerData.phone ?? "Chưa có số";
        }
    }

    public void BackToMainMenu()
    {
        if (userPanel != null) userPanel.SetActive(false);
        if (horizontalLayout != null) horizontalLayout.SetActive(true);
    }

    public void OpenLoginPanelDirectly()
    {
        Debug.Log("🔐 Mở loginPanel");

        if (loginPanel != null) loginPanel.SetActive(true);
        if (horizontalLayout != null) horizontalLayout.SetActive(false);
        if (userPanel != null) userPanel.SetActive(false);
    }

    public void OnLogoutClicked()
    {
        Debug.Log("🚪 Đăng xuất...");

        // Xóa dữ liệu login
        PlayerPrefs.DeleteKey("LoggedUsername");
        PlayerPrefs.DeleteKey("LoggedEmail");
        PlayerPrefs.DeleteKey("LoggedPhone");
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerData.username = null;
            GameManager.Instance.PlayerData.email = null;
            GameManager.Instance.PlayerData.phone = null;
            GameManager.Instance.SavePlayerData();
        }

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

        if (userButtonText != null && GameManager.Instance != null)
            userButtonText.text = GameManager.Instance.PlayerData.username ?? "Login";
    }
}
