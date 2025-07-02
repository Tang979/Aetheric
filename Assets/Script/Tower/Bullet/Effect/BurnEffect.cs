using UnityEngine;

public class BurnEffect : MonoBehaviour,IBulletEffect
{
    public float duration = 4f; // Thời gian hiệu ứng cháy
    public GameObject burnVFXPrefab; // Hiệu ứng VFX cháy

    public void ApplyEffect(Transform target)
    {
        var status = target.GetComponent<EnemyStatus>();
        var spray = GetComponent<Spray>();
        if (status != null)
        {
            var damage = spray.GetDamage() * 0.3f;
            var tickRate = spray.GetTickRate();
            status.ApplyEffect(StatusEffectType.Burn, duration, tickRate, damage);
            status.TryPlayVFX(StatusEffectType.Burn, burnVFXPrefab, duration);
        }
    }
}
