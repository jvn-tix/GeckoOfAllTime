using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public int damage = 10;
    public float lifeTime = 3f;
    public float knockbackForce = 6f;

    [Header("VFX Settings")]
    [SerializeField] private GameObject hitVfxPrefab;

    [Header("Cluster Settings")]
    private int clusterCount;
    [SerializeField] private float clusterSpread = 40f;   // sudut total sebaran (derajat)
    [SerializeField] private float clusterLifeTime = 1f;  // umur pecahan (lebih singkat dari peluru utama)

    private Rigidbody2D rb;
    private TrailRenderer trail;

    private bool canCluster;
    private float clusterMultiplier = 0.5f;
    private GameObject clusterPrefab;
    private bool isClusterChild;
    private bool hasHit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, isClusterChild ? clusterLifeTime : lifeTime);
    }

    // --- FUNCTION DIPANGGIL DARI TURRETCONTROLLER ---
    public void SetDamage(float newDamage)
    {
        damage = Mathf.RoundToInt(newDamage);
    }

    public void SetCluster(bool enabled, float multiplier, GameObject prefab, int count)
    {
        canCluster = enabled;
        clusterMultiplier = multiplier;
        clusterPrefab = prefab;
        clusterCount = count;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Enemy"))
        {
            hasHit = true;

            Health enemy = collision.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            EnemyMovement movement = collision.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.ApplyKnockback(transform.up, knockbackForce);
            }

            if (hitVfxPrefab != null)
            {
                Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
            }

            if (canCluster && !isClusterChild && clusterPrefab != null)
            {
                SpawnCluster(collision);
            }

            DestroyBullet();
        }
    }

    private void SpawnCluster(Collider2D hitEnemy)
    {
        int childDamage = Mathf.Max(1, Mathf.RoundToInt(damage * clusterMultiplier));
        float spread = clusterSpread + (clusterCount - 3) * 10f;

        for (int i = 0; i < clusterCount; i++)
        {
            float t = clusterCount > 1 ? (float)i / (clusterCount - 1) : 0.5f;
            float angle = Mathf.Lerp(-clusterSpread * 0.5f, clusterSpread * 0.5f, t);

            Quaternion rot = transform.rotation * Quaternion.Euler(0f, 0f, angle);
            GameObject child = Instantiate(clusterPrefab, transform.position, rot);

            Bullet b = child.GetComponent<Bullet>();
            if (b != null)
            {
                b.isClusterChild = true;   // pecahan tidak boleh pecah lagi
                b.canCluster = false;
                b.SetDamage(childDamage);
            }

            // Pecahan tidak langsung mengenai musuh yang sama
            Collider2D childCol = child.GetComponent<Collider2D>();
            if (childCol != null && hitEnemy != null)
                Physics2D.IgnoreCollision(childCol, hitEnemy);
        }
    }

    private void OnDestroy()
    {
        DetachTrail();
    }

    private void DestroyBullet()
    {
        DetachTrail();
        Destroy(gameObject);
    }

    private void DetachTrail()
    {
        if (trail != null)
        {
            trail.transform.parent = null;
            trail.emitting = false;
            Destroy(trail.gameObject, trail.time);
        }
    }
}