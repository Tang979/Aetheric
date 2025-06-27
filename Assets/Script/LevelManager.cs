using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public int levelId;
    public int currentWave;
    public int currentGold;
    public int remainingHealth;

    public Transform StartPoint;
    public Transform[] Path;

    private void Awake()
    {
        main = this;
        LoadLevelProgress();
    }

    public void LoadLevelProgress()
    {
        var progress = GameManager.Instance.PlayerData.levelProgresses.Find(p => p.levelId == levelId);

        if (progress != null)
        {
            currentWave = progress.currentWave;
            currentGold = progress.currentGold;
            remainingHealth = progress.remainingHealth;
        }
        else
        {
            currentWave = 1;
            currentGold = 100;
            remainingHealth = 20;
            UIManager.instance.UpdateMoney();
            UIManager.instance.UpdateHP(remainingHealth);
            UIManager.instance.UpdateWave(currentWave, 10);
        }
    }

    public void SaveProgress()
    {
        var data = GameManager.Instance.PlayerData;
        var progress = data.levelProgresses.Find(p => p.levelId == levelId);

        if (progress == null)
        {
            progress = new LevelProgress { levelId = levelId };
            data.levelProgresses.Add(progress);
        }

        progress.isInProgress = true;
        progress.currentWave = currentWave;
        progress.currentGold = currentGold;
        progress.remainingHealth = remainingHealth;

        GameManager.Instance.SavePlayerData();
    }

    public bool TrySpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UIManager.instance.UpdateMoney();
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UIManager.instance.UpdateMoney();
    }
} 
