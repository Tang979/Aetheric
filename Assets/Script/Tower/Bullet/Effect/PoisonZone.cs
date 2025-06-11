using UnityEngine;

public class PoisonZone : MonoBehaviour
{
    public float radius = 2f;
    public float duration = 4f;
    public float damagePerTick = 0.5f;
    public float tickRate = 1f;
    public LayerMask enemyLayer;

    private float tickTimer = 0f;
    private float lifeTimer = 0f;

    void Update()
    {
        lifeTimer += Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickRate)
        {

            ApplyPoisonToEnemies();
            tickTimer = 0f;
        }

        if (lifeTimer >= duration)
        {
            Destroy(gameObject);
        }
    }

    void ApplyPoisonToEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        foreach (var hit in hits)
        {
            hit.GetComponent<EnemyHealth>()?.TakeDamage(damagePerTick);
            var status = hit.GetComponent<EnemyStatus>();
            if (status != null)
            {
                status.ApplyEffect(StatusEffectType.Poison, tickRate, tickRate, damagePerTick);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
