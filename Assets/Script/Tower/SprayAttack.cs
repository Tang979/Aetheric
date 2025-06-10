using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.VFX;

public class SprayAttack : MonoBehaviour, ITowerAttack
{
    private TowerInstance tower;
    private TargetingSystem targetingSystem;
    private GameObject sprayPrefab;
    private Spray spray;
    private float cooldown = 0f;
    private bool isSpraying = false;

    public void Init(TowerInstance tower)
    {
        this.tower = tower;
        targetingSystem = tower.GetComponent<TargetingSystem>();
        targetingSystem.SetRange(tower.data.attackRange);
        sprayPrefab = Instantiate(tower.data.bulletPrefab, tower.firePoint.position, Quaternion.identity);
        spray = sprayPrefab.GetComponent<Spray>();
        spray.Stop();
        spray.SetTickRate(tower.data.sprayConfig.tickRate);
        spray.SetDamage(tower.data.baseDamage);
    }

    public void Tick(float deltaTime)
    {
        var target = targetingSystem.GetCurrentTarget();
        if (target == null)
        {
            spray.SetTarget(target);
        }
        if (spray.IsActive()) return;
        cooldown -= deltaTime;

        // Nếu chưa spray và cooldown đã hết → bắt đầu
        if (!isSpraying && cooldown <= 0f && target != null)
        {
            spray.Play(target, tower.data.sprayConfig.attackDuration);
            isSpraying = true;
            cooldown = tower.data.sprayConfig.cooldown;
        }
        // Nếu spray đã kết thúc → reset flag
        if (isSpraying && !spray.IsActive())
        {
            isSpraying = false;
        }
        if (target == null)
        {
            spray.Stop();
            isSpraying = false;
        }
    }
}
