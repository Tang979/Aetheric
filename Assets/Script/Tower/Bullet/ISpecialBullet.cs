using UnityEngine;

public interface ISpecialBullet
{
    bool TryModifyBullet(TowerInstance tower, ref GameObject prefab, ref float damage);
}

