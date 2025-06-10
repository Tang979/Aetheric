using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage;

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Kiểm tra va chạm đơn giản bằng khoảng cách
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            // Gây sát thương
            var health = target.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            ReturnToPool();
        }
    }

    public GameObject prefabOrigin;

    void ReturnToPool()
    {
        BulletPool.Instance.ReturnBullet(gameObject, prefabOrigin);
    }
}
