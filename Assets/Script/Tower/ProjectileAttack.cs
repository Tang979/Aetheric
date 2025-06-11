using UnityEngine;

public class ProjectileAttack : MonoBehaviour, ITowerAttack
{
    private TowerInstance tower;
    private TargetingSystem targeting;
    private float cooldown;

    public void Init(TowerInstance tower)
    {
        this.tower = tower;
        targeting = tower.GetComponent<TargetingSystem>();
        targeting.SetRange(tower.data.attackRange);
        cooldown = 0f;
    }

    public void Tick(float deltaTime)
    {
        cooldown -= deltaTime;

        if (cooldown <= 0f)
        {
            var target = targeting.GetCurrentTarget();
            if (target != null)
            {
                Shoot(target);
                cooldown = tower.data.projectileConfig.attackSpeed;
            }
        }
    }

    void Shoot(Transform target)
    {
        GameObject prefabToUse = tower.data.bulletPrefab;
        float usedDamage = tower.data.baseDamage;

        foreach (var mod in GetComponents<ISpecialBullet>())
        {
            if (mod.TryModifyBullet(tower, ref prefabToUse, ref usedDamage))
                break;
        }

        GameObject bullet = BulletPool.Instance.GetBullet(prefabToUse);
        bullet.transform.position = tower.firePoint.position;
        bullet.transform.rotation = Quaternion.identity;

        Bullet b = bullet.GetComponent<Bullet>();
        b.prefabOrigin = prefabToUse;
        b.SetTarget(target);
        b.SetDamage(usedDamage);
    }

}
