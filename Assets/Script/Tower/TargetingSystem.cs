using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    private float range;
    private Transform currentTarget;
    private LayerMask enemyLayer;

    public void SetRange(float range)
    {
        this.range = range;
    }

    private void Awake() {
        enemyLayer = LayerMask.GetMask("Enemy");
        currentTarget = null;
    }

    public Transform GetCurrentTarget()
    {
        if (currentTarget == null || !IsTargetValid(currentTarget))
        {
            currentTarget = FindFirstEnemyInRange();
        }

        return currentTarget;
    }

    private bool IsTargetValid(Transform target)
    {
        if (target == null) return false;

        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= range;
    }

    private Transform FindFirstEnemyInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                return hit.transform;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
