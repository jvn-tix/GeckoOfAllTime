using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public int damage = 10;
    public float lifeTime = 3f; // Peluru hancur otomatis setelah 3 detik jika tidak kena apa-apa

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifeTime);
    }

    //void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // Cek jika menabrak musuh (pastikan GameObject musuh diberi Tag "Enemy")
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        // Panggil fungsi TakeDamage di script musuh (jika ada)
    //        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
    //        if (enemy != null)
    //        {
    //            enemy.TakeDamage(damage);
    //        }

    //        // Hancurkan peluru saat kena musuh
    //        Destroy(gameObject);
    //    }
    //}
}