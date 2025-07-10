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
                Team = new List<string>(5),
                username = "Guest",
                email = "unknown@email.com",
                phone = "N/A"
            };

            // Cấp 6 tháp mặc định cho người chơi
            string[] starterTowerIds = new string[] { "Fire", "Ice", "Poison", "Rock", "Electric", "Basic" };

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

        var card = new TowerCard(tower.towerName, unlock, upgrade)
        {
            level = 1,
            ownedCards = 0
        };

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

    
    // ✅ Thêm hàm lưu dữ liệu người dùng từ Cognito
    public void UpdateUserInfoFromCognito(string username, string email, string phone)
    {
        PlayerData.username = username;
        PlayerData.email = email;
        PlayerData.phone = phone;

        SavePlayerData();
        Debug.Log($"✅ User info updated: {username}, {email}, {phone}");
    }

    // Các phương thức quản lý team
    public void TryAddTowerToTeam(string towerName)
    {
        if (PlayerData.Team.Contains(towerName))
        {
            Debug.Log("Tháp đã có trong team");
            return;
        }

        if (!IsTeamValid())
        {
            PlayerData.Team.Add(towerName);
            UIManager.Instance.towerDetailView.Hide();
            TowerCardDisplayManager cardDisplayManager = FindFirstObjectByType<TowerCardDisplayManager>();
            SavePlayerData();
            cardDisplayManager.LoadTeamUI();
            return;
        }

        Debug.Log("Team đã đầy");
        if (towerName == "Basic") return;
        UIManager.Instance.loadTempTeam(towerName);
        return;
    }

    public void ReplaceTowerInTeam(string oldTower, string newTower)
    {
        int index = PlayerData.Team.IndexOf(oldTower);
        if (index == -1)
        {
            Debug.Log("Không tìm thấy tháp cũ trong team");
            return;
        }

        if (PlayerData.Team.Contains(newTower))
        {
            Debug.Log("Tháp mới đã tồn tại trong team");
            return;
        }

        PlayerData.Team[index] = newTower;
        UIManager.Instance.tempTeam.Hide();
        TowerCardDisplayManager cardDisplayManager = FindFirstObjectByType<TowerCardDisplayManager>();
        SavePlayerData();
        cardDisplayManager.LoadTeamUI();
        return;
    }

    public void RemoveTowerFromTeam(string towerName)
    {
        if (PlayerData.Team.Contains(towerName))
        {
            PlayerData.Team.Remove(towerName);
            UIManager.Instance.towerDetailView.Hide();
            TowerCardDisplayManager cardDisplayManager = FindFirstObjectByType<TowerCardDisplayManager>();
            SavePlayerData();
            cardDisplayManager.LoadTeamUI();
            return;
        }

        Debug.Log("Tháp không có trong team");
        return;
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
    public List<TowerCard> ownedTowerCards = new();
    public List<string> Team = new(5);

    // ✅ Thông tin người dùng từ Cognito
    public string username;
    public string email;
    public string phone;

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
