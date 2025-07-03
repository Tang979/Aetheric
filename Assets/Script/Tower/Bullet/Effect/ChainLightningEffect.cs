using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

public class ChainLightningEffect : MonoBehaviour, ISpecialBulletEffect
{
    public int maxTargets = 3;
    private List<Transform> listTarget = new();
    public int indexTarget = 1;
    public float chainRadius = 1.5f;
    private float baseDame;
    public float damageMultiplier = 0.33f;
    public GameObject lightning;
    private Bullet bullet;

    public bool ShouldSkipAutoReturn => true;

    private void Start()
    {
        bullet = GetComponent<Bullet>();
        baseDame = bullet.GetDamage();
    }

    public void ApplyEffect(Transform firstTarget)
    {
        if (firstTarget == null || indexTarget == 3)
        {
            listTarget.Clear();
            indexTarget = 1;
            bullet.SetDamage(baseDame);
            EndEffect();
            return;
        }
        listTarget.Add(firstTarget);
        indexTarget++;
        bullet.SetTarget(FindNextTarget(firstTarget, listTarget));
        bullet.SetDamage(baseDame * damageMultiplier);
    }

    private Transform FindNextTarget(Transform from, List<Transform> alreadyHit)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from.position, chainRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyHealth>(out var enemy) && !alreadyHit.Contains(enemy.transform))
            {
                return enemy.transform;
            }
        }
        return null;
    }

    private void EndEffect()
    {
        bullet.ReturnToPool();
    }
}
