using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyData[] enemyTypes;
    [SerializeField] private int playerHP = 20;
    [SerializeField] private int playerMoney = 100;
    [SerializeField] private int totalWaves = 10;


    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    public static UnityEvent OnEnemyReachEnd = new UnityEvent();


    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    private EnemyData currentEnemyData;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
        OnEnemyReachEnd.AddListener(EnemyReachedEnd);

    }

    private void EnemyReachedEnd()
    {
        playerHP--;
        UIManager.instance.UpdateHP(playerHP);

        if (playerHP <= 0)
        {
            Debug.Log("Game Over - You Lose!");
            // xử lý kết thúc game tại đây
        }
    }


    private void Start()
    {
        StartCoroutine(StartWave());
        UIManager.instance.UpdateMoney(playerMoney);
        UIManager.instance.UpdateWave(currentWave, totalWaves);
        UIManager.instance.UpdateHP(playerHP);

        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / currentEnemyData.enemiesPerSecond) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0)
        {
            EndWave();
        }
    }

    private IEnumerator StartWave()
    {
        currentEnemyData = GetEnemyDataForWave(currentWave);

        yield return new WaitForSeconds(currentEnemyData.timeBetweenWaves);

        isSpawning = true;

        if (currentWave >= 6)
        {
            enemiesLeftToSpawn = 3;
        }
        else
        {
            enemiesLeftToSpawn = Mathf.RoundToInt(currentEnemyData.baseEnemies * Mathf.Pow(currentWave, currentEnemyData.difficultyScalingFactor));
        }
    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;

        UIManager.instance.UpdateWave(currentWave, totalWaves);
        playerMoney += 10;
        UIManager.instance.UpdateMoney(playerMoney);

        if (currentWave > totalWaves)
        {
            Debug.Log("Game Over - You Win!");
            return;
        }

        StartCoroutine(StartWave());
    }


    private void SpawnEnemy()
    {
        GameObject prefabToSpawn;

        if (currentWave >= 6 && enemyTypes.Length >= 3)
        {
            prefabToSpawn = enemyTypes[2].prefab;
        }
        else if (currentWave < 3 && enemyTypes.Length >= 1)
        {
            prefabToSpawn = enemyTypes[0].prefab;
        }
        else if (enemyTypes.Length >= 2)
        {
            float chance = (currentWave - 2) * 0.1f; // wave 3 = 10%, wave 4 = 20%, ...
            chance = Mathf.Clamp01(chance);
            prefabToSpawn = UnityEngine.Random.value < chance ? enemyTypes[1].prefab : enemyTypes[0].prefab;
        }
        else
        {
            Debug.LogWarning("Không đủ enemyTypes để spawn.");
            prefabToSpawn = enemyTypes[0].prefab;
        }

        GameObject enemy = Instantiate(prefabToSpawn, LevelManager.main.StartPoint.position, quaternion.identity);

        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.Setup(currentEnemyData);
        }

        EnemyMovement move = enemy.GetComponent<EnemyMovement>();
        if (move != null)
        {
            move.Setup(currentEnemyData.moveSpeed);
        }

    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private EnemyData GetEnemyDataForWave(int wave)
    {
        if (wave >= 6 && enemyTypes.Length >= 3)
            return enemyTypes[2];

        if (wave < 3 && enemyTypes.Length >= 1)
            return enemyTypes[0];

        if (enemyTypes.Length >= 2)
        {
            // vẫn trả enemyTypes[0] làm default cho stats như tốc độ, delay
            return enemyTypes[0];
        }

        Debug.LogWarning("EnemyData không đủ!");
        return enemyTypes[0];
    }
}
