using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    private Transform target;
    private int partIndex = 0;

    private float moveSpeed;
    private float baseSpeed;

    public void Setup(float speed)
    {
        baseSpeed = speed;
        moveSpeed = baseSpeed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (LevelManager.main == null)
        {
            Debug.LogError("LevelManager.main is NULL!");
            return;
        }

        if (LevelManager.main.Path.Length == 0)
        {
            Debug.LogError("Path array is empty!");
            return;
        }

        target = LevelManager.main.Path[partIndex];
    }

    void Update()
    {
        if (target == null || LevelManager.main.Path.Length == 0) return;

        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            partIndex++;

            if (partIndex >= LevelManager.main.Path.Length)
            {
                // Enemy đã đến đích
                LevelManager.main.remainingHealth--;
                UILevelManager.instance.DecreaseHP(1); // nếu dùng
                EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
                enemyHealth.Die();
                // EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }

            target = LevelManager.main.Path[partIndex];
        }
    }

    public void ApplySpeedMultiplier(float multiplier)
    {
        moveSpeed = baseSpeed * Mathf.Clamp01(1f - multiplier);
    }

    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

    private void FixedUpdate()
    {
        if (rb != null && target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
