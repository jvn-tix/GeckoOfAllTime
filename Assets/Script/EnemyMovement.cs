using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationOffset = -90f; // Sesuaikan jika sprite miring (misal: 0, -90, 90, atau 45)

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
        if (playerTransform != null)
        {
            // Hitung arah menuju player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // 1. Gerakkan posisi menggunakan Rigidbody2D
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            // 2. Hitung sudut rotasi dalam derajat
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. Putar fisik Rigidbody2D ke arah player
            rb.MoveRotation(angle + rotationOffset);
        }
    }
}