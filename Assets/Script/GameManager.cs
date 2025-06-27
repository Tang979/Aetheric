using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData PlayerData { get; private set; }

    private string savePath => Path.Combine(Application.persistentDataPath, "playerdata.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            PlayerData = new PlayerData()
            {
                coin = 0,
                gem = 0,
                unlockedLevels = new int[] { 1 },
                unlockedTowers = new string[] { "Basic" },
                levelProgresses = new List<LevelProgress>()
            };

            SavePlayerData();
        }
    }

    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(PlayerData, true);
        File.WriteAllText(savePath, json);
    }

    public void AddCoins(int amount)
    {
        PlayerData.coin += amount;
        SavePlayerData();
        Debug.Log("Coin: " + PlayerData.coin);
    }

    public void AddGems(int amount)
    {
        PlayerData.gem += amount;
        SavePlayerData();
        Debug.Log("Gem: " + PlayerData.gem);
    }

    public void EnsureLevelProgress(int levelId)
    {
        if (!PlayerData.levelProgresses.Exists(p => p.levelId == levelId))
        {
            PlayerData.levelProgresses.Add(new LevelProgress()
            {
                levelId = levelId,
                isInProgress = false,
                currentWave = 0,
                remainingHealth = 100,
                currentGold = 40
            });
            SavePlayerData();
        }
    }
}

[Serializable]
public class PlayerData
{
    public int coin;
    public int gem;
    public int[] unlockedLevels;
    public string[] unlockedTowers;
    public List<LevelProgress> levelProgresses = new();
}

[Serializable]
public class LevelProgress
{
    public int levelId;
    public bool isInProgress;
    public int currentWave;
    public int remainingHealth;
    public int currentGold;
}

public static class UpgradeCostCalculator
{
    public static int GetUpgradeCost(int level, TowerData.TowerRarity rarity)
    {
        float baseCost = 40f;
        float rarityMultiplier = rarity switch
        {
            TowerData.TowerRarity.Common => 1.0f,
            TowerData.TowerRarity.Rare => 1.3f,
            TowerData.TowerRarity.Epic => 1.6f,
            TowerData.TowerRarity.Legendary => 2.0f,
            _ => 1f
        };

        float upgradeFactor = 1.5f;
        float exponent = level - 1;

        return Mathf.RoundToInt(baseCost * rarityMultiplier * Mathf.Pow(upgradeFactor, exponent));
    }
}