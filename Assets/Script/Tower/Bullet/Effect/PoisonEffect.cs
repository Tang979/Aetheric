using UnityEngine;

public class PoisonEffect : MonoBehaviour, IBulletEffect
{
    public float duration = 4f;
    public float tickRate = 1f;
    [Range(0f, 1f)] public float areaChance = 0.2f;
    public GameObject poisonAreaPrefab;
    public GameObject poisonVFXPrefab;

    public void ApplyEffect(Transform target)
    {
        var status = target.GetComponent<EnemyStatus>();
        var bullet = GetComponent<Bullet>();
        if (status != null)
        {
            var damage = bullet.GetDamage() * 0.3f;

            status.ApplyEffect(StatusEffectType.Poison, duration, tickRate, damage);
            status.TryPlayVFX(StatusEffectType.Poison, poisonVFXPrefab, duration);

            if (Random.value <= areaChance && poisonAreaPrefab != null)
            {
                Instantiate(poisonAreaPrefab, target.position, Quaternion.identity);
            }
        }
    }
}
