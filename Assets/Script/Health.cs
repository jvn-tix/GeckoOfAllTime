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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

    }
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(spriteRenderer != null)
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

    private void Die()
    {
        // Nanti bisa tambahkan efek ledakan / spawn Gem XP di sini
        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}