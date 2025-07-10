using UnityEngine;
using UnityEngine.SceneManagement;
public class SettingMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelSetting;

    private bool isOpen = false;

    // Gọi khi ấn nút Setting
    public void ToggleSettingPanel()
    {
        isOpen = !isOpen;

        if (panelSetting != null)
            panelSetting.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f; // Tạm dừng hoặc tiếp tục game
        Debug.Log(isOpen ? "Game Paused from Setting" : "Game Resumed from Setting");
    }

    // Gọi khi ấn nút Resume
    public void ResumeGame()
    {
        isOpen = false;

        if (panelSetting != null)
            panelSetting.SetActive(false);

        Time.timeScale = 1f;
        Debug.Log("Game Resumed from Resume Button");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Demo 1");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Đảm bảo game không bị pause khi restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
