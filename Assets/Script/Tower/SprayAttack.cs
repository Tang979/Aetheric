using UnityEngine;

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
        sprayPrefab = Instantiate(tower.data.bulletPrefab, tower.transform.position, Quaternion.identity, tower.transform);
        spray = sprayPrefab.GetComponent<Spray>();
        spray.Stop();
        spray.SetRange(tower.data.attackRange);
        spray.SetTickRate(tower.CurrentTickRate);
        spray.SetDamage(tower.CurrentDamage);
    }

    public void Tick(float deltaTime)
    {
        var target = targetingSystem.GetCurrentTarget();
        spray.SetTarget(target);
        spray.Play(target);
    }
}
