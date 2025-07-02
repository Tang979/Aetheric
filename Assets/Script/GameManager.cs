using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData PlayerData { get; private set; }
    [SerializeField] public TowerCardDatabase towerCardDatabase;

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
        Debug.Log("Player data loaded from: " + savePath);
        Debug.Log("Tower cards owned: " + PlayerData.ownedTowerCards.Count);
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
                levelProgresses = new List<LevelProgress>(),
                ownedTowerCards = new List<TowerCard>(),
                Team = new List<string>(5)
            };

            // Cấp 6 tháp mặc định cho người chơi
            string[] starterTowerIds = new string[] { "Fire", "Ice", "Poison", "Rock", "Basic" };

            foreach (string id in starterTowerIds)
            {
                var card = towerCardDatabase.GetTowerByName(id);
                InitDefaultTowerCards(card);
            }

            SavePlayerData();
        }
    }

    public void InitDefaultTowerCards(TowerData tower)
    {
        int unlock = GetUnlockRequirement(tower.rarity);
        int upgrade = GetUpgradeRequirement(tower.rarity);

        var card = new TowerCard(tower.towerName, unlock, upgrade);

        card.level = 1;
        card.ownedCards = 0;

        PlayerData.ownedTowerCards.Add(card);
        if (tower.towerName != "Basic")
            PlayerData.Team.Add(card.towerName);

        SavePlayerData();
    }

    public int GetUnlockRequirement(TowerData.TowerRarity rarity)
    {
        return rarity switch
        {
            TowerData.TowerRarity.Common => 5,
            TowerData.TowerRarity.Rare => 10,
            TowerData.TowerRarity.Epic => 15,
            TowerData.TowerRarity.Legendary => 20,
            _ => 5
        };
    }

    public int GetUpgradeRequirement(TowerData.TowerRarity rarity)
    {
        return rarity switch
        {
            TowerData.TowerRarity.Common => 5,
            TowerData.TowerRarity.Rare => 8,
            TowerData.TowerRarity.Epic => 12,
            TowerData.TowerRarity.Legendary => 20,
            _ => 5
        };
    }

    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(PlayerData, true);
        File.WriteAllText(savePath, json);
    }

    public bool TryAddToTeam(string towerName)
    {
        if (PlayerData.Team.Contains(towerName)) return false;
        if (PlayerData.Team.Count >= 5) return false;

        PlayerData.Team.Add(towerName);
        return true;
    }

    public bool ReplaceTowerInTeam(string oldTower, string newTower)
    {
        int index = PlayerData.Team.IndexOf(oldTower);
        if (index == -1 || PlayerData.Team.Contains(newTower)) return false;

        PlayerData.Team[index] = newTower;
        return true;
    }

    public bool RemoveAndAddToTeam(string toRemove, string toAdd)
    {
        if (!PlayerData.Team.Contains(toRemove)) return false;
        if (PlayerData.Team.Contains(toAdd)) return false;

        int index = PlayerData.Team.IndexOf(toRemove);
        PlayerData.Team[index] = toAdd;
        return true;
    }

    public bool IsTeamValid()
    {
        return PlayerData.Team.Count == 5;
    }
}

[Serializable]
public class PlayerData
{
    public List<LevelProgress> levelProgresses = new();
    public List<TowerCard> ownedTowerCards = new(); // Danh sách thẻ người chơi có
    public List<string> Team = new(5); // Chỉ lưu tên tháp // Danh sách thẻ trong đội hình

    public void AddTowerCard(string towerName, int quantity)
    {
        var card = ownedTowerCards.Find(c => c.towerName == towerName);
        if (card != null)
        {
            card.ownedCards += quantity;
        }
        else
        {
            TowerData towerData = GameManager.Instance.towerCardDatabase.GetTowerByName(towerName);
            int unlock = GameManager.Instance.GetUnlockRequirement(towerData.rarity);
            int upgrade = GameManager.Instance.GetUpgradeRequirement(towerData.rarity);

            card = new TowerCard(towerName, unlock, upgrade);
            card.ownedCards = quantity;

            ownedTowerCards.Add(card);
        }
    }

}

[Serializable]
public class LevelProgress
{
    public int levelId;
    public bool isInProgress;
    public bool isCompleted;
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