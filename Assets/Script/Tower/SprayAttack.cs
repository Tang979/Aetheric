using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.VFX;

public class SprayAttack : MonoBehaviour, ITowerAttack
{
    private TowerInstance tower;
    private TargetingSystem targetingSystem;
    private GameObject sprayPrefab;
    private Spray spray;

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
        if (target != null)
        {
            spray.SetTarget(target);
            spray.Play(target);
        }
    }
}
