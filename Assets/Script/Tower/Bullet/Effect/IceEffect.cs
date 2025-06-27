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
        if (status != null)
        {
            status.ApplyEffect(StatusEffectType.Slow, duration, 0f, slowMultiplier);
            status.TryPlayVFX(StatusEffectType.Slow, iceVFXPrefab, duration);
        }
    }
}
