using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;

    [Header("Position & Rotation Lock")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, -0.6f, 0f); // Jarak di bawah kaki Player

    private Transform playerParent;

    void Start()
    {
        // Ambil Induk Player
        if (transform.parent != null)
        {
            playerParent = transform.parent;
        }
    }

    void LateUpdate()
    {
        if (playerParent != null)
        {
            // 1. Kunci posisi agar selalu tepat di bawah Player + Offset
            transform.position = playerParent.position + localOffset;

            // 2. Kunci rotasi agar selalu tegak lurus (tidak ikut berputar/miring)
            transform.rotation = Quaternion.identity;
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthFillImage != null && maxHealth > 0)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}