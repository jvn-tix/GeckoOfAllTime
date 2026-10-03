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

    [Header("UI Reference (Only For Player)")]
    [SerializeField] private PlayerHealthBar healthBar;

    [Header("Drop Settings (Only For Enemy)")]
    [SerializeField] private GameObject expOrbPrefab;

    void Awake()
    {
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
            gameObject.SetActive(false);
        }
        else
        {
            if(expOrbPrefab != null)
            {
                Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}