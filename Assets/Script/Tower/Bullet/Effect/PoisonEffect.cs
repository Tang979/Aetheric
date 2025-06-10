using UnityEngine;

public class PoisonEffect : MonoBehaviour, IBulletEffect
{
    // This method applies the poison effect to the target Transform
    [Header("Poison Effect")]
    public float poisonDuration = 5f;
    public float poisonDamage = 10f;
    public float range = 3f;

    public void ApplyEffect(Transform target)
    {
        
    }
}
