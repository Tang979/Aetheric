using UnityEngine;

public class EnamyMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    private Transform target;
    private int partIndex = 0;

    private float moveSpeed;

    public void Setup(float speed)
    {
        moveSpeed = speed;
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
                UIManager.instance.DecreaseHP(1); // nếu dùng
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }

            target = LevelManager.main.Path[partIndex];
        }
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
