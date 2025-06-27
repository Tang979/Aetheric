using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Create Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Base Info")]
    public string enemyName;
    public GameObject prefab;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public int health = 100;
    public float damage = 10f;

    [Header("Spawn Settings")]
    public int baseEnemies = 8;
    public float enemiesPerSecond = 2f;
    public float timeBetweenWaves = 5f;
    public float difficultyScalingFactor = 0.75f;
    // internal GameObject enemyPrefab;
}
