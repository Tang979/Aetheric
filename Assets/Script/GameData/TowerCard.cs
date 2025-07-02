using System;
using UnityEngine;

[Serializable]
public class TowerCard
{
    public string towerName;
    public int level;
    public int ownedCards;

    public int cardsToUnlock;
    public int cardsToUpgrade;

    public TowerCard(string name, int unlockCards, int upgradeCards)
    {
        towerName = name;
        level = 0;
        ownedCards = 0;
        cardsToUnlock = unlockCards;
        cardsToUpgrade = upgradeCards;
    }

    public bool IsUnlocked => level > 0;

    /// <summary>
    /// Gọi khi lên cấp: tăng số thẻ cần thiết cho lần nâng cấp tiếp theo
    /// </summary>
    public void LevelUp(TowerData.TowerRarity rarity)
    {
        level++;
        cardsToUpgrade = GetUpgradeRequirement(level, rarity);
    }

    public static int GetUnlockRequirement(TowerData.TowerRarity rarity)
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

    public static int GetUpgradeRequirement(int currentLevel, TowerData.TowerRarity rarity)
    {
        int baseCost = rarity switch
        {
            TowerData.TowerRarity.Common => 5,
            TowerData.TowerRarity.Rare => 8,
            TowerData.TowerRarity.Epic => 12,
            TowerData.TowerRarity.Legendary => 20,
            _ => 5
        };

        // Tăng chậm ban đầu, tăng dần về sau (x1.5)
        float multiplier = 1.5f;
        return Mathf.RoundToInt(baseCost * Mathf.Pow(multiplier, currentLevel - 1));
    }

    public bool CanUnlock => !IsUnlocked && ownedCards >= cardsToUnlock;
    public bool CanUpgrade => IsUnlocked && ownedCards >= cardsToUpgrade;

    public int GetRequiredToUnlock()
    {
        return cardsToUnlock;
    }

    public int GetRequiredToUpgrade()
    {
        return cardsToUpgrade;
    }
}
