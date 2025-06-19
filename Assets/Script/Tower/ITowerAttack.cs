using UnityEngine;

public interface ITowerAttack
{
    void Init(TowerInstance tower);
    void Tick(float deltaTime);
}
