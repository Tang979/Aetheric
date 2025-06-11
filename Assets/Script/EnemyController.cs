using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float moveSpeed;
    private int health;
    private float damage;

    public void Setup(EnemyData data)
    {
        moveSpeed = data.moveSpeed;
        health = data.health;
        damage = data.damage;
    }

    void Update()
    {
        // Apply logic using moveSpeed, damage, etc.
    }
}
