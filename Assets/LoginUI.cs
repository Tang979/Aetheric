using UnityEngine;
using TMPro;
using Doozy.Runtime.UIManager.Components; // Dành cho UIButton

public class LoginPanelToggle : MonoBehaviour
{
    [Header("Doozy UIButton")]
    public UIButton loginUIButton; // Nút Login nằm trong HorizontalLayout

    [Header("UI Elements")]
    public GameObject horizontalLayout; // Layout chứa các nút (Play, Setting, Profile...)
    public GameObject loginPanel;       // Panel chứa form đăng nhập

    void Start()
    {
        // Gán sự kiện click cho UIButton
        if (loginUIButton != null)
        {
            loginUIButton.onClickEvent.AddListener(ShowLoginPanel);
        }
        else
        {
            Debug.LogWarning("⚠️ UIButton Login chưa được gán trong Inspector!");
        }

        // Mặc định: hiện HorizontalLayout, ẩn LoginPanel
        if (horizontalLayout != null) horizontalLayout.SetActive(true);
        if (loginPanel != null) loginPanel.SetActive(false);
    }

    void ShowLoginPanel()
    {
        Debug.Log("🔐 Đã nhấn nút Login - Hiện loginPanel, ẩn HorizontalLayout");

        // Ẩn layout chứa các nút
        if (horizontalLayout != null)
        {
            horizontalLayout.SetActive(false);
            Debug.Log("✅ HorizontalLayout đã bị ẩn");
        }

        // Hiện panel đăng nhập
        if (loginPanel != null)
        {
            loginPanel.SetActive(true);
            Debug.Log("✅ LoginPanel đã được hiện");
        }
    }
}
