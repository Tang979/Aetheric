using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    private float range;
    private Transform currentTarget;

    public void SetRange(float range)
    {
        this.range = range;
    }
    public Transform GetCurrentTarget()
    {
        // Nếu chưa có target hoặc target không hợp lệ
        if (currentTarget == null)
        {
            currentTarget = FindFirstEnemyInRange();
        }
        else if (currentTarget != null)
        {
            // Kiểm tra xem target còn trong phạm vi hay không
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            if (dist > range)
            {
                currentTarget = null;
            }
        }

        return currentTarget;
    }

    Transform FindFirstEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist <= range)
            {
                return enemy.transform;
            }
        }

        return null;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
