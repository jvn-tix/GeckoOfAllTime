using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public int damage = 10;
    public float lifeTime = 3f; // Peluru hancur otomatis setelah 3 detik jika tidak kena apa-apa
    public float knockbackForce = 6f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

            // Hancurkan peluru saat kena musuh
            Destroy(gameObject);
        }
    }
}