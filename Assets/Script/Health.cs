using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Nanti bisa tambahkan efek ledakan / spawn Gem XP di sini
        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}