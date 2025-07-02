using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Aetheric/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string towerName;
    public TowerRarity rarity;
    public bool isBasicTower = false;
    public bool isComingSoon = false;
    public Sprite icon;
    public float attackSpeed;
    public string descriptionSkill;
    public AttackType attackType;
    public float attackRange;
    public float baseDamage;
    public GameObject bulletPrefab;
    internal int towerType;

    // public EffectType effect;
    public enum AttackType
    {
        Projectile,
        Spray
    }

    public enum TowerRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    [System.Serializable]
    public class TowerStats
    {
        public TowerRarity rarity;
        public float baseUpgradeCost = TowerSpawnManager.Instance.GetCoinSummon();
        public float upgradeFactor = 1.2f;
        public float exponent = 1.4f;

        public int GetUpgradeCost(int level)
        {
            GetStats(rarity);
            return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(upgradeFactor, Mathf.Pow(level - 1, exponent)));
        }
    }
    public static TowerStats GetStats(TowerRarity rarity)
    {
        switch (rarity)
        {
            case TowerRarity.Common:
                return new TowerStats { rarity = rarity, upgradeFactor = 1.25f, exponent = 1.4f };
            case TowerRarity.Rare:
                return new TowerStats { rarity = rarity, upgradeFactor = 1.3f, exponent = 1.6f };
            case TowerRarity.Epic:
                return new TowerStats { rarity = rarity, upgradeFactor = 1.4f, exponent = 1.8f };
            case TowerRarity.Legendary:
                return new TowerStats { rarity = rarity, upgradeFactor = 1.6f, exponent = 2.0f };
            default:
                return new TowerStats();
        }
    }
}
