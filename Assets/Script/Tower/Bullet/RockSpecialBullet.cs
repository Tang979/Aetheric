using UnityEngine;

public class RockSpecialBullet : MonoBehaviour, ISpecialBullet
{
    public GameObject specialBulletPrefab;
    [Range(0f, 1f)] public float chance = 0.2f;
    public float damageMultiplier = 3f;

    public bool TryModifyBullet(TowerInstance tower, ref GameObject prefab, ref float damage)
    {
        if (Random.value <= chance)
        {
            prefab = specialBulletPrefab;
            damage *= damageMultiplier;
            return true;
        }

        return false;
    }
}
