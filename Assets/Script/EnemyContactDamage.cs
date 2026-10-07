using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageInterval = 0.5f;
    private float nextDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}