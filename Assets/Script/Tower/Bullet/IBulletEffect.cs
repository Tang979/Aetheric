using UnityEngine;

public interface IBulletEffect
{
    void ApplyEffect(Transform firstTarget);
}

public interface ISpecialBulletEffect : IBulletEffect
{
    bool ShouldSkipAutoReturn { get; }
}