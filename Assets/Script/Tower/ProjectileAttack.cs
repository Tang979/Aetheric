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
        GameObject bullet = BulletPool.Instance.GetBullet(tower.data.bulletPrefab);
        bullet.transform.position = tower.firePoint.position;
        bullet.transform.rotation = Quaternion.identity;
        Bullet b = bullet.GetComponent<Bullet>();
        b.prefabOrigin = tower.data.bulletPrefab;
        b.SetTarget(target);
        b.damage = tower.data.baseDamage;
        if (b != null)
        {
            b.SetTarget(target);
            b.damage = tower.data.baseDamage;
        }
    }

}
