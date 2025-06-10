using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;

    private ITowerAttack attackLogic;

    void Start()
    {
        // Gắn component theo kiểu tấn công
        switch (data.attackType)
        {
            case TowerData.AttackType.Projectile:
                attackLogic = gameObject.AddComponent<ProjectileAttack>();
                break;
            case TowerData.AttackType.Spray:
                attackLogic = gameObject.AddComponent<SprayAttack>();
                break;
            case TowerData.AttackType.Zone:
                attackLogic = gameObject.AddComponent<ZoneAttack>();
                break;
            // sau này sẽ thêm các kiểu khác
        }

        attackLogic?.Init(this);
    }

    void Update()
    {
        attackLogic?.Tick(Time.deltaTime);
    }
}
