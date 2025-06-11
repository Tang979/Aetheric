using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Text UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI hpText;

    [Header("Gameplay")]
    [SerializeField] private int startingHP = 20;
    private int currentHP;
    private bool isGameOver = false;
    private bool isPaused;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        currentHP = startingHP;
        UpdateHP(currentHP);
    }

    public void UpdateMoney(int amount)
    {
        moneyText.text = $"${amount}";
    }

    public void UpdateWave(int current, int total)
    {
        waveText.text = $"Wave {current}/{total}";
    }

    public void UpdateHP(int hp)
    {
        hpText.text = $"Máu {Mathf.Max(0, hp)}";
    }

    public void DecreaseHP(int amount)
    {
        if (isGameOver) return;

        currentHP -= amount;
        UpdateHP(currentHP);

        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
        Time.timeScale = 0f; // Dừng thời gian trong game

        // TODO: Nếu có màn hình Game Over, kích hoạt nó ở đây
        // Ví dụ: gameOverPanel.SetActive(true);
    }

}
