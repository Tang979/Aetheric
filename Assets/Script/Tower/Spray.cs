using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Spray : MonoBehaviour
{
    public float range = 3f;
    public float coneAngle = 45f;
    private float damage;
    public float tickRate;
    private float tickTimer = 0f;
    private float rotateSpeed = 90f; // Rotation speed of the cone
    private VisualEffect sprayVFX;
    public LayerMask enemyLayer;
    private Transform currentTarget = null;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public float GetDamage()
    {
        return damage;
    }
    public void SetTickRate(float rate)
    {
        tickRate = rate;
    }
    public float GetTickRate()
    {
        return tickRate;
    }
    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    private void Awake()
    {
        sprayVFX = GetComponentInChildren<VisualEffect>();
    }

    private List<Transform> cachedTargets = new List<Transform>();
    private float lastCollisionCheckTime = 0f;
    private const float COLLISION_CHECK_INTERVAL = 0.1f; // Check every 0.1 seconds

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null)
        {
            Stop();
            return;
        }

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);

        Vector2 origin = transform.position;

        if (Time.time - lastCollisionCheckTime >= COLLISION_CHECK_INTERVAL)
        {
            cachedTargets = GetCollidersInCone(origin, range, coneAngle, enemyLayer);
            lastCollisionCheckTime = Time.time;
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;
            foreach (var target in cachedTargets)
            {
                EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
                foreach (var effect in GetComponents<IBulletEffect>())
                {
                    effect.ApplyEffect(target);
                }
            }

        }
    }

    // Get all colliders in a 2D cone shape on the Y plane
    public List<Transform> GetCollidersInCone(Vector2 origin, float range, float coneAngle, LayerMask enemyLayer)
    {
        List<Transform> targets = new List<Transform>();
        Vector2 direction = transform.up; // Sweep direction is Y+

        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, range, enemyLayer);


        foreach (var col in colliders)
        {
            Vector2 toTarget = ((Vector2)col.transform.position - origin).normalized;
            float angle = Vector2.Angle(direction, toTarget);

            if (angle <= coneAngle)
            {
                targets.Add(col.transform);
            }
        }

        return targets;
    }

    public void Play(Transform target)
    {
        currentTarget = target;

        if (sprayVFX != null) sprayVFX.Play();
    }

    public void Stop()
    {
        if (sprayVFX != null) sprayVFX.Stop();
    }

    // Vẽ vùng quét hình nón trong Scene view trên mặt phẳng Y
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.up; // luôn hướng lên

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f); // màu cam nhạt
        int segments = 30;
        float angleStep = (coneAngle * 2f) / segments;

        Vector2 lastPoint = origin + Rotate(direction, -coneAngle) * range;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -coneAngle + angleStep * i;
            Vector2 nextPoint = origin + Rotate(direction, angle) * range;

            Gizmos.DrawLine(origin, nextPoint);
            Gizmos.DrawLine(lastPoint, nextPoint);

            lastPoint = nextPoint;
        }

        // Tuỳ chọn: Vẽ đường tròn vùng phát hiện
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawWireSphere(origin, range);
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}
