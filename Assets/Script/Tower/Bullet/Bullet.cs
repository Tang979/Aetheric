using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private float damage;
    private Transform target;
    private float rotateSpeed = 360f;
    public GameObject prefabOrigin;
    public GameObject objectEfect;

    private bool isProcessingEffect = false;

    public void SetTarget(Transform _target)
    {
        if(objectEfect != null)
            objectEfect.SetActive(true);
        target = _target;
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void SetDamage(float _damage) => damage = _damage;
    public float GetDamage() => damage;

    void Update()
    {

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        // Di chuyển
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        // Va chạm đơn giản
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            ApplyHitEffects();
        }
    }

    private void ApplyHitEffects()
    {
        var health = target.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        bool skipAutoReturn = false;

        foreach (var effect in GetComponents<IBulletEffect>())
        {
            effect.ApplyEffect(target);

            if (effect is ISpecialBulletEffect special && special.ShouldSkipAutoReturn)
            {
                skipAutoReturn = true;
            }
        }

        if (!skipAutoReturn)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        // Dừng tất cả hiệu ứng VFX nếu có
        if (objectEfect != null)
            objectEfect.SetActive(false);

        // isProcessingEffect = false;
            BulletPool.Instance.ReturnBullet(gameObject, prefabOrigin);
    }
}
