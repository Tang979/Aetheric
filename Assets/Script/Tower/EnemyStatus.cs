using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StatusEffectType
{
    Poison,
    Burn,
    Freeze,
    Slow,
    Stun
}

public class StatusEffect
{
    public StatusEffectType type;
    public float duration;
    public float tickRate;
    public float value;
    public Coroutine coroutine;

    public StatusEffect(StatusEffectType type, float duration, float tickRate, float value)
    {
        this.type = type;
        this.duration = duration;
        this.tickRate = tickRate;
        this.value = value;
    }
}

public class EnemyStatus : MonoBehaviour
{
    private Dictionary<StatusEffectType, StatusEffect> activeEffects = new();

    public void ApplyEffect(StatusEffectType type, float duration, float tickRate, float value)
    {
        if (activeEffects.TryGetValue(type, out var existing))
        {
            StopCoroutine(existing.coroutine);
        }

        var newEffect = new StatusEffect(type, duration, tickRate, value);
        newEffect.coroutine = StartCoroutine(HandleEffect(newEffect));
        activeEffects[type] = newEffect;
    }

    private IEnumerator HandleEffect(StatusEffect effect)
    {
        float timer = 0f;
        var health = GetComponent<EnemyHealth>();

        while (timer < effect.duration)
        {
            switch (effect.type)
            {
                case StatusEffectType.Poison:
                case StatusEffectType.Burn:
                    health?.TakeDamage(effect.value);
                    break;
                case StatusEffectType.Slow:
                    // Giảm tốc độ (chưa triển khai)
                    var sprite = GetComponent<SpriteRenderer>();
                    if (sprite != null) sprite.color = Color.cyan;
                    break;
                case StatusEffectType.Freeze:
                    // Đóng băng tạm thời (chưa triển khai)
                    break;
                case StatusEffectType.Stun:
                    // Làm choáng (chưa triển khai)
                    break;
            }

            yield return new WaitForSeconds(effect.tickRate);
            timer += effect.tickRate;
        }

        activeEffects.Remove(effect.type);
    }
}