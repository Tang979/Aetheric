using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Aetheric/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    public UpgradeStats commonStats;
    public UpgradeStats rareStats;
    public UpgradeStats epicStats;
    public UpgradeStats legendaryStats;

    public UpgradeStats GetStatsForRarity(TowerData.TowerRarity rarity)
    {
        return rarity switch
        {
            TowerData.TowerRarity.Common => commonStats,
            TowerData.TowerRarity.Rare => rareStats,
            TowerData.TowerRarity.Epic => epicStats,
            TowerData.TowerRarity.Legendary => legendaryStats,
            _ => commonStats
        };
    }

    public float CalculateUpgradeCost(TowerData.TowerRarity rarity, int level)
    {
        UpgradeStats stats = GetStatsForRarity(rarity);
        return stats.baseUpgradeCost * Mathf.Pow(stats.upgradeFactor, Mathf.Pow(level - 1, stats.exponent));
    }
}
[System.Serializable]
public class UpgradeStats
{
    public float baseUpgradeCost;
    public float upgradeFactor;
    public float exponent;
}
