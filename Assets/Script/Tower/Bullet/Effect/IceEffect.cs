using UnityEngine;

public class IceEffect : MonoBehaviour, IBulletEffect
{
    public float slowMultiplier = 0.5f;
    public float duration = 3f;

    public void ApplyEffect(Transform target)
    {
        var status = target.GetComponent<EnemyStatus>();
        if (status != null)
        {
            status.ApplyEffect(StatusEffectType.Slow, duration, 0f, slowMultiplier);
        }
    }
}
