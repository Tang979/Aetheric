using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private float damage;

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target;
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    public float GetDamage()
    {
        return damage;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);

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

            foreach (var effect in GetComponents<IBulletEffect>())
            {
                effect.ApplyEffect(target);
            }

            ReturnToPool();
        }
    }

    public GameObject prefabOrigin;
    private float rotateSpeed = 360f; 

    void ReturnToPool()
    {
        BulletPool.Instance.ReturnBullet(gameObject, prefabOrigin);
    }
}
