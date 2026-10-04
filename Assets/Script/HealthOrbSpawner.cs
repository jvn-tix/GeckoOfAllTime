using UnityEngine;

public class HealthOrbSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject healthOrbPrefab;
    [SerializeField] private float spawnInterval = 10f;
    private Transform playerTransform;

    [Header("Spawn Area")]
    [SerializeField] private float spawnRadiusMin = 4f; 
    [SerializeField] private float spawnRadiusMax = 10f;

    private float nextSpawnTime;

    private void Start()
    {
        // Cari player secara otomatis jika slot belum diisi di Inspector
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Set waktu spawn pertama
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnOrbRandomly();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnOrbRandomly()
    {
        if (healthOrbPrefab == null || playerTransform == null) return;

        // Ambil vektor arah acak di sekitar lingkaran
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(spawnRadiusMin, spawnRadiusMax);

        // Hitung koordinat spawn relatif dari posisi Player
        Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDirection * randomDistance);

        Instantiate(healthOrbPrefab, spawnPosition, Quaternion.identity);
    }

    // Visualisasi area spawn di Scene View (Opsional)
    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, spawnRadiusMin);
            Gizmos.DrawWireSphere(playerTransform.position, spawnRadiusMax);
        }
    }
}