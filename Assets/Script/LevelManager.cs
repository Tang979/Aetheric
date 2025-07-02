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
            if (progress.isCompleted)
            {
                // Nếu level đã hoàn thành, reset lại thông tin
                ResetLevel();
            }
            else
            {
                // Nếu level đang chơi, load thông tin hiện tại
                currentWave = progress.currentWave;
                currentGold = progress.currentGold;
                remainingHealth = progress.remainingHealth;
                // Cập nhật UI
                UILevelManager.instance.UpdateMoney();
                UILevelManager.instance.UpdateHP(remainingHealth);
                UILevelManager.instance.UpdateWave(currentWave, EnemySpawner.main.waves.Count);
            }
        }
        else
        {
            ResetLevel();
        }
    }

    public void ResetLevel()
    {
        currentWave = 1;
        currentGold = 100;
        remainingHealth = 20;

        // Reset UI
        UILevelManager.instance.UpdateMoney();
        UILevelManager.instance.UpdateHP(remainingHealth);
        UILevelManager.instance.UpdateWave(currentWave, EnemySpawner.main.waves.Count);
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

    public void CompleteLevel()
    {
        var data = GameManager.Instance.PlayerData;
        var progress = data.levelProgresses.Find(p => p.levelId == levelId);

        if (progress == null)
        {
            progress = new LevelProgress { levelId = levelId };
            data.levelProgresses.Add(progress);
        }

        progress.isInProgress = false;
        progress.isCompleted = true;
        progress.currentWave = currentWave;
        progress.remainingHealth = remainingHealth;
        progress.currentGold = currentGold;
        // Thêm phần thưởng nếu có
        var rewardedTowers = RewardManager.Instance.GrantLevelRewards(levelId);
        UILevelManager.instance.ShowLevelCompleteUI(rewardedTowers);
    }

    public bool TrySpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UILevelManager.instance.UpdateMoney();
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UILevelManager.instance.UpdateMoney();
    }
}
