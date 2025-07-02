using UnityEngine;

public class IceEffect : MonoBehaviour, IBulletEffect
{
    public float slowMultiplier = 0.2f;
    public float duration = 5f;

    [Header("VFX")]
    public GameObject iceVFXPrefab;

    public void ApplyEffect(Transform target)
    {
        var status = target.GetComponent<EnemyStatus>();
        var spray = GetComponent<Spray>();
        if (status != null)
        {
            var tickRate = spray.GetTickRate();
            status.ApplyEffect(StatusEffectType.Slow, duration, tickRate, slowMultiplier);
            status.TryPlayVFX(StatusEffectType.Slow, iceVFXPrefab, duration);
        }
    }
}
