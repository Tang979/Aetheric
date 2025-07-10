using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5;

    private float currentHealth;
    [SerializeField] private int coinReward = 10;

    private bool isDead = false;
    public Action OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth; // Reset current health to max when setting a new max
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return; // Bảo vệ không cho chạy lần 2
        isDead = true;

        var levelManager = LevelManager.main;
        if (levelManager != null)
        {
            levelManager.currentGold += coinReward;
            UILevelManager.instance.UpdateMoney();
        }
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }
}
