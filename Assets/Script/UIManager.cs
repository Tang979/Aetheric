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

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
        // TODO: Hiển thị UI Game Over hoặc load scene
    }


}
