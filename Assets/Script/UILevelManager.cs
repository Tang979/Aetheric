using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelManager : MonoBehaviour
{
    public static UILevelManager instance;

    [Header("Text UI")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI hpText;
    public GameObject iconX1;
    public GameObject iconX2;
    private bool isSpeedUp = false;


    [Header("Gameplay")]
    [SerializeField] private int startingHP = 20;
    private int currentHP;
    private bool isGameOver = false;
    private bool isPaused;

    private TowerInstance currentTower;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Giữ UIManager khi chuyển scene

        currentHP = startingHP;
        UpdateHP(currentHP);
    }

    public void UpdateMoney()
    {
        moneyText.text = $"${LevelManager.main.currentGold}";
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
        Debug.Log("Đã nhấn nút Pause");

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }

    public void ToggleSpeed()
    {
        isSpeedUp = !isSpeedUp;
        Time.timeScale = isSpeedUp ? 2f : 1f;

        if (iconX1 != null && iconX2 != null)
        {
            iconX1.SetActive(!isSpeedUp); // hiện icon 1 gạch khi x1
            iconX2.SetActive(isSpeedUp);  // hiện icon 2 gạch khi x2
        }

        Debug.Log(isSpeedUp ? "Tốc độ x2" : "Tốc độ thường");
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

    public GameObject panel;
    public Transform rewardContainer;
    public GameObject rewardSlotPrefab;

    public void ShowLevelCompleteUI(List<(TowerData tower, int quantity)> rewards)
    {
        panel.SetActive(true);

        // Xoá cũ
        foreach (Transform child in rewardContainer)
            Destroy(child.gameObject);
        // Tạo mới
        foreach (var reward in rewards)
        {
            var slot = Instantiate(rewardSlotPrefab, rewardContainer);
            slot.transform.Find("Icon").GetComponent<Image>().sprite = reward.tower.icon;
            slot.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = $"x{reward.quantity}";
        }
    }
}