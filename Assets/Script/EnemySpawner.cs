using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Cho phép liên kết với Button

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SubWave
    {
        public EnemyData enemyData;
        public int count;
        public float spawnRate;
        public float delayBeforeNextSubWave;
    }

    [System.Serializable]
    public class Wave
    {
        public List<SubWave> subWaves;
    }

    public List<Wave> waves;
    public Transform[] spawnPoints;
    public GameObject nextWaveButton; // UI button
    private int currentSubWaveIndex = 0;
    private int enemiesAlive = 0;

    public float delayBeforeWaveStarts = 5f;
    private bool isWaitingForNextWave = false;
    private bool allEnemiesSpawned = false;

    public static EnemySpawner main;

    public delegate void WaveEndHandler();
    public event WaveEndHandler OnWaveEnd;

    private void Awake()
    {
        main = this;
        nextWaveButton.SetActive(true);
        nextWaveButton.GetComponent<Button>().onClick.AddListener(OnNextWaveButtonClicked);
        isWaitingForNextWave = true; // Trạng thái đợi ngay từ đầu
    }

    IEnumerator SpawnWaveRoutine()
    {
        var CurrentWave = LevelManager.main.currentWave - 1;// Giảm 1 vì currentWave bắt đầu từ 1 nhưng danh sách waves bắt đầu từ 0
        while (CurrentWave < waves.Count)
        {
            yield return new WaitForSeconds(delayBeforeWaveStarts);
            Wave wave = waves[CurrentWave];

            allEnemiesSpawned = false;

            for (currentSubWaveIndex = 0; currentSubWaveIndex < wave.subWaves.Count; currentSubWaveIndex++)
            {
                SubWave subWave = wave.subWaves[currentSubWaveIndex];

                for (int i = 0; i < subWave.count; i++)
                {
                    SpawnEnemy(subWave.enemyData);
                    yield return new WaitForSeconds(1f / subWave.spawnRate);
                }

                yield return new WaitForSeconds(subWave.delayBeforeNextSubWave);
            }

            allEnemiesSpawned = true;
            LevelManager.main.currentWave++;
            Debug.Log($"Wave {CurrentWave} đã spawn tất cả quái.");

            // Chờ đến khi tất cả quái đã spawn và bị tiêu diệt hết
            yield return new WaitUntil(() => allEnemiesSpawned && enemiesAlive == 0);

            // Gọi event nếu cần
            OnWaveEnd?.Invoke();

            // Đợi người chơi ấn nút tiếp tục
            isWaitingForNextWave = true;
            nextWaveButton.SetActive(true);
            yield return new WaitUntil(() => !isWaitingForNextWave);
        }

        Debug.Log("Tất cả wave đã hoàn thành.");
    }

    void SpawnEnemy(EnemyData data)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(data.prefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<EnemyController>().Setup(data);
        enemy.GetComponent<EnemyMovement>().Setup(data.moveSpeed);
        enemy.GetComponent<EnemyHealth>().maxHealth = data.health;
        enemiesAlive++;

        // Khi enemy chết, giảm đếm
        enemy.GetComponent<EnemyHealth>().OnEnemyDeath += () =>
        {
            enemiesAlive--;
        };
    }

    public void OnNextWaveButtonClicked()
    {
        nextWaveButton.SetActive(false);
        isWaitingForNextWave = false;

        // Nếu đây là lần đầu tiên => bắt đầu Coroutine
        if (LevelManager.main.currentWave == 1 && currentSubWaveIndex == 0)
        {
            StartCoroutine(SpawnWaveRoutine());
        }
        else
        {
            LevelManager.main.currentWave++;
            UIManager.instance.UpdateWave(LevelManager.main.currentWave, waves.Count);
        }

        Debug.Log("Bắt đầu wave kế tiếp.");
    }

    public void ForceNextWave()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnWaveRoutine());
    }

    internal int GetTotalWaves()
    {
        return waves.Count;
    }
}