using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public string enemyName = "Basic Enemy";
        public GameObject enemyPrefab; // Prefab musuh
        [Range(1, 100)] public int spawnWeight = 10; // Semakin besar angka, semakin sering muncul
    }

    [Header("Enemy Types Configuration")]
    [SerializeField] private EnemyData[] enemyTypes;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f; // Jeda waktu antar spawn (detik)
    [SerializeField] private float spawnDistance = 12f; // Jarak spawn di luar kamera dari player

    private Transform playerTransform;
    private float nextSpawnTime;

    [Header("Endgame Settings")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private float endgameDuration = 120f;
    [SerializeField] private AnimationCurve endgameCurve = AnimationCurve.Linear(0f, 1f, 1f, 3f);
    [SerializeField] private int maxEnemiesAlive = 150;

    void Start()
    {
        // Cari Player berdasarkan Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("EnemySpawner: Tidak menemukan GameObject dengan Tag 'Player'!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (Time.time >= nextSpawnTime)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemiesAlive)
            {
                SpawnRandomEnemy();
            }
            nextSpawnTime = Time.time + spawnInterval / GetSpawnMultiplier();
        }
    }

    private float GetSpawnMultiplier()
    {
        if (gameTimer == null) return 1f;

        float remaining = gameTimer.TimeRemaining;
        if (remaining > endgameDuration) return 1f;

        float t = 1f - Mathf.Clamp01(remaining / endgameDuration);
        return Mathf.Max(0.1f, endgameCurve.Evaluate(t));
    }

    void SpawnRandomEnemy()
    {
        if (enemyTypes == null || enemyTypes.Length == 0) return;

        // Pilih musuh secara acak berdasarkan Bobot (Spawn Weight)
        GameObject selectedEnemyPrefab = GetRandomEnemyPrefab();

        if (selectedEnemyPrefab != null)
        {
            // Hitung posisi melingkar acak di luar jarak pandang Player
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * spawnDistance;

            // Spawn Musuh
            Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        int totalWeight = 0;
        foreach (var enemy in enemyTypes)
        {
            totalWeight += enemy.spawnWeight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        foreach (var enemy in enemyTypes)
        {
            currentWeightSum += enemy.spawnWeight;
            if (randomValue < currentWeightSum)
            {
                return enemy.enemyPrefab;
            }
        }

        return enemyTypes[0].enemyPrefab;
    }

    // Visualisasi jarak spawn di Scene View
    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, spawnDistance);
        }
    }
}