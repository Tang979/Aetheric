using UnityEngine;

public class PoisonEffect : MonoBehaviour, IBulletEffect
{
    public float duration = 4f;
    public float damagePerTick = 0.5f;
    public float tickRate = 1f;
    [Range(0f, 1f)] public float areaChance = 0.2f;
    public GameObject poisonAreaPrefab;
    public GameObject poisonVFXPrefab;

    public void ApplyEffect(Transform target)
    {
        var status = target.GetComponent<EnemyStatus>();
        if (status != null)
        {
            status.ApplyEffect(StatusEffectType.Poison, duration, tickRate, damagePerTick);
            status.TryPlayVFX(StatusEffectType.Poison, poisonVFXPrefab,duration);

            if (Random.value <= areaChance && poisonAreaPrefab != null)
            {
                Instantiate(poisonAreaPrefab, target.position, Quaternion.identity);
            }
        }
    }
}
