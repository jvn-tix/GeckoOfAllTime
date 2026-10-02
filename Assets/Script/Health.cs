using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    [Header("Hit Color Settings")]
    [SerializeField] private Color hitColor = Color.red;
    public float hitColorDuration = 0.2f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("UI Reference (Opsional - Cukup diisi untuk Player)")]
    [SerializeField] private PlayerHealthBar healthBar;

    void Awake()
    {
        // 1. Inisialisasi darah di Awake() agar nilainya LANGSUNG terisi sebelum frame pertama berjalan
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Start()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<PlayerHealthBar>();
        }

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " HP Sekarang: " + currentHealth);

        UpdateUI();

        if (spriteRenderer != null)
        {
            StartCoroutine(HitFlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitColorDuration);
        spriteRenderer.color = originalColor;
    }

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("Player Mati! Game Over.");
            // Untuk Player, JANGAN Destroy(gameObject) langsung agar game tidak error, 
            // melainkan matikan gerakan/tampilkan layar Game Over.
            gameObject.SetActive(false);
        }
        else
        {
            // Jika Enemy, hancurkan objeknya
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}