using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 5;
    private float currentHealth;
    [SerializeField] private int coinReward = 10;

    public Action OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
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
        var levelManager = LevelManager.main;
        if (levelManager != null)
        {
            levelManager.currentGold += coinReward;
            UIManager.instance.UpdateMoney();
        }        
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }
}
