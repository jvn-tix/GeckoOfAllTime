using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public int damage = 10;
    public float lifeTime = 3f; // Peluru hancur otomatis setelah 3 detik jika tidak kena apa-apa
    public float knockbackForce = 6f;

    [Header("VFX Settings")]
    [SerializeField] private GameObject hitVfxPrefab;

    private Rigidbody2D rb;
    private TrailRenderer trail;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        // Menggunakan linearVelocity untuk Unity versi terbaru / 2D Physics
        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifeTime);
    }

    // --- FUNCTION DIPANGGIL DARI TURRETCONTROLLER ---
    public void SetDamage(float newDamage)
    {
        damage = Mathf.RoundToInt(newDamage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek jika menabrak musuh (pastikan GameObject musuh diberi Tag "Enemy")
        if (collision.CompareTag("Enemy"))
        {
            // Panggil fungsi TakeDamage di script musuh
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

            if(hitVfxPrefab != null)
            {
                // Buat efek visual saat peluru menabrak musuh
                Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
            }

            DestroyBullet();
        }
    }

    private void OnDestroy()
    {
        // Jika peluru hancur karena waktu (lifetime habis), lepaskan trail-nya
        DetachTrail();
    }

    // Fungsi khusus untuk menghancurkan peluru tanpa memutus jejak trail secara mendadak
    private void DestroyBullet()
    {
        DetachTrail();
        Destroy(gameObject);
    }

    private void DetachTrail()
    {
        if (trail != null)
        {
            // Lepas trail dari peluru agar bisa memudar dengan mulus
            trail.transform.parent = null;
            trail.emitting = false;

            // Hancurkan objek trail secara otomatis setelah waktu Trail habis
            Destroy(trail.gameObject, trail.time);
        }
    }
}