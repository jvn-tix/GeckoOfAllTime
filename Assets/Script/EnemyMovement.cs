using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationOffset = -90f; // Sesuaikan jika sprite miring (misal: 0, -90, 90, atau 45)

    [Header("Knockback Settings")]
    [SerializeField, Range(0f, 1f)] private float knockbackResistance = 0f;
    [SerializeField] private float knockbackDuration = 0.12f;
    private float knockbackTimer;

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cari Player berdasarkan Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Rotasi tetap jalan, supaya musuh tetap menghadap player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle + rotationOffset);

        // Selama knockback, jangan gerak mengejar
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f) rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (rb == null) return;

        rb.linearVelocity = direction.normalized * force * (1f - knockbackResistance);
        knockbackTimer = knockbackDuration;
    }
}