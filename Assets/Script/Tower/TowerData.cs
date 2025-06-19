using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Aetheric/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string towerName;
    public bool isSpecialTower = false;
    public bool isBasicTower = false;
    public Sprite icon;
    public string descriptionSkill;
    public int cost;
    public AttackType attackType;
    [Header("Cấu hình tấn công")]
    public SprayConfig sprayConfig;
    public ProjectileConfig projectileConfig;
    public float attackRange;
    public float baseDamage;
    public GameObject bulletPrefab;
    internal int towerType;

    // public EffectType effect;
    public enum AttackType
    {
        Projectile,
        Spray,
        Zone
    }

}
[System.Serializable]
public class SprayConfig
{
    public float attackDuration;
    public float cooldown;
    public float tickRate;
}
[System.Serializable]
public class ProjectileConfig
{
    public float attackSpeed;
}
