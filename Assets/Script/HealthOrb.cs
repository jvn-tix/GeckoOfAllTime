using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [Header("Orb Settings")]
    [SerializeField] private float healAmount = 25f; // Jumlah HP yang dipulihkan
    [SerializeField] private float lifeTime = 15f;   // Hancur otomatis jika tidak diambil dalam 15 detik

    [Header("Audio")]
    [SerializeField] private AudioClip collectSfx;

    private void Start()
    {
        // Hancurkan orb jika tidak diambil dalam batas waktu lifeTime
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(collectSfx);
                Destroy(gameObject);
            }
        }
    }
}