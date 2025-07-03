using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Aetheric/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string towerName;
    public GameObject specialTowerPrefab;
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
    public static float GetDamage(TowerData data, int level)
    {
        float damage = data.baseDamage * (1f + 0.1f * (level - 1)); // Tăng 10% mỗi cấp
        return (float)Math.Round(damage, 1); // Làm tròn 1 chữ số thập phân
    }

    public float attackSpeedPerLevel = 0.1f;
    public float minAttackInterval = 0.2f; // tối đa bắn 5 lần/giây

    public static float GetAttackSpeed(TowerData data, int level)
    {
        // Giảm 0.05 mỗi cấp, và không thấp hơn minSpeed
        float minSpeed = 0.2f; // tùy vào mức tối thiểu bạn cho phép
        float speed = data.attackSpeed - (level - 1) * 0.02f;
        speed = Mathf.Max(speed, minSpeed);

        // Làm tròn 1 chữ số thập phân
        return (float)Math.Round(speed, 1);
    }
}
