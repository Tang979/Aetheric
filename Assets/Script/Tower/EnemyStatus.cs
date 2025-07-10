using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum StatusEffectType
{
    Burn,
    Poison,
    Slow,
    Freeze,
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
    private class VFXInstance
    {
        public GameObject vfxObject;
        public Coroutine timerCoroutine;
    }
    private Dictionary<StatusEffectType, VFXInstance> activeVFX = new();


    private static readonly Dictionary<StatusEffectType, StatusEffectType[]> conflictingEffects = new()
    {
        { StatusEffectType.Burn, new[] { StatusEffectType.Slow, StatusEffectType.Freeze } },
        { StatusEffectType.Slow, new[] { StatusEffectType.Burn } },
        { StatusEffectType.Freeze, new[] { StatusEffectType.Burn } }
    };

    public void ApplyEffect(StatusEffectType type, float duration, float tickRate, float value)
    {
        if (conflictingEffects.TryGetValue(type, out var toRemoveList))
        {
            foreach (var conflict in toRemoveList)
            {
                if (activeEffects.TryGetValue(conflict, out var conflictEffect))
                {
                    StopCoroutine(conflictEffect.coroutine);
                    activeEffects.Remove(conflict);

                    // Gỡ VFX nếu có
                    if (activeVFX.TryGetValue(conflict, out var vfx))
                    {
                        Destroy(vfx.vfxObject);
                        StopCoroutine(vfx.timerCoroutine);
                        activeVFX.Remove(conflict);
                    }

                    // Reset trạng thái nếu là Slow
                    if (conflict == StatusEffectType.Slow)
                    {
                        GetComponent<EnemyMovement>()?.ResetSpeed();
                        GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
        }

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
        var movement = GetComponent<EnemyMovement>();
        var sprite = GetComponent<SpriteRenderer>();

        switch (effect.type)
        {
            case StatusEffectType.Slow:
                movement?.ApplySpeedMultiplier(effect.value);
                if (sprite != null) sprite.color = new Color(0f, 0.196f, 0.749f); // Xanh dương
                break;
        }

        while (timer < effect.duration)
        {
            if (effect.tickRate > 0f)
            {
                switch (effect.type)
                {
                    case StatusEffectType.Burn:
                    case StatusEffectType.Poison:
                        health?.TakeDamage(effect.value);
                        break;
                }

                yield return new WaitForSeconds(effect.tickRate);
                timer += effect.tickRate;
            }
            else
            {
                yield return new WaitForSeconds(effect.duration);
                break;
            }
        }

        activeEffects.Remove(effect.type);

        if (effect.type == StatusEffectType.Slow)
        {
            movement?.ResetSpeed();
            if (sprite != null) sprite.color = Color.white;
        }

        if (activeVFX.TryGetValue(effect.type, out var vfx))
        {
            Destroy(vfx.vfxObject);
            StopCoroutine(vfx.timerCoroutine);
            activeVFX.Remove(effect.type);
        }
    }

    public void TryPlayVFX(StatusEffectType type, GameObject prefab, float duration)
    {
        if (activeVFX.TryGetValue(type, out var instance))
        {
            if (instance.timerCoroutine != null)
                StopCoroutine(instance.timerCoroutine);

            instance.timerCoroutine = StartCoroutine(AutoRemoveVFX(type, duration));
            return;
        }

        if (prefab != null)
        {
            GameObject vfx = Instantiate(prefab, transform.position, Quaternion.identity, transform);

            var newInstance = new VFXInstance
            {
                vfxObject = vfx,
                timerCoroutine = StartCoroutine(AutoRemoveVFX(type, duration))
            };

            activeVFX[type] = newInstance;
        }
    }

    private IEnumerator AutoRemoveVFX(StatusEffectType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (activeVFX.TryGetValue(type, out var instance))
        {
            Destroy(instance.vfxObject);
            activeVFX.Remove(type);
        }
    }
}
