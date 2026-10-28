using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public string enemyName = "Basic Enemy";
        public GameObject enemyPrefab;

        [Header("Weight per fase")]
        [Min(0)] public int earlyWeight = 10;
        [Min(0)] public int midWeight = 10;
        [Min(0)] public int lateWeight = 10;
    }

    private enum Phase { Early, Mid, Late }

    [Header("Enemy Types Configuration")]
    [SerializeField] private EnemyData[] enemyTypes;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnDistance = 20f;

    [Header("Progression")]
    [SerializeField] private float totalGameDuration = 600f; // samakan dengan durasi GameTimer (detik)
    [SerializeField, Range(0f, 1f)] private float midPhaseStart = 0.33f;
    [SerializeField, Range(0f, 1f)] private float latePhaseStart = 0.66f;

    private Transform playerTransform;
    private float nextSpawnTime;

    [Header("Endgame Settings")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private float endgameDuration = 120f;
    [SerializeField] private AnimationCurve endgameCurve = AnimationCurve.Linear(0f, 1f, 1f, 3f);
    [SerializeField] private int maxEnemiesAlive = 150;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("EnemySpawner: Tidak menemukan GameObject dengan Tag 'Player'!");
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

    // 0 = awal game, 1 = akhir game
    private float GetGameProgress()
    {
        if (gameTimer == null || totalGameDuration <= 0f) return 0f;

        float elapsed = totalGameDuration - gameTimer.TimeRemaining;
        return Mathf.Clamp01(elapsed / totalGameDuration);
    }

    private Phase GetCurrentPhase()
    {
        float progress = GetGameProgress();
        if (progress >= latePhaseStart) return Phase.Late;
        if (progress >= midPhaseStart) return Phase.Mid;
        return Phase.Early;
    }

    private int GetWeight(EnemyData enemy, Phase phase)
    {
        switch (phase)
        {
            case Phase.Early: return enemy.earlyWeight;
            case Phase.Mid: return enemy.midWeight;
            default: return enemy.lateWeight;
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

        GameObject selectedEnemyPrefab = GetRandomEnemyPrefab();

        if (selectedEnemyPrefab != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * spawnDistance;

            Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        Phase phase = GetCurrentPhase();

        int totalWeight = 0;
        foreach (var enemy in enemyTypes)
            totalWeight += GetWeight(enemy, phase);

        // Kalau semua weight 0, fallback ke tipe pertama
        if (totalWeight <= 0) return enemyTypes[0].enemyPrefab;

        int randomValue = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        foreach (var enemy in enemyTypes)
        {
            currentWeightSum += GetWeight(enemy, phase);
            if (randomValue < currentWeightSum)
                return enemy.enemyPrefab;
        }

        return enemyTypes[0].enemyPrefab;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, spawnDistance);
        }
    }
}