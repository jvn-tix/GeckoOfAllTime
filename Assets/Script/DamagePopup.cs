using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    [Header("Movement")]
    [SerializeField] private float jumpSpeed = 4f;
    [SerializeField] private float sideSpeed = 1.5f;
    [SerializeField] private float gravity = 14f;

    [Header("Timing")]
    [SerializeField] private float lifetime = 0.7f;
    [SerializeField] private float popScale = 1.4f;
    [SerializeField] private float popDuration = 0.12f;

    private Vector2 velocity;
    private float timer;
    private Color baseColor;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshPro>();
        baseColor = text.color;
    }

    public void Setup(int damage)
    {
        text.text = damage.ToString();
        velocity = new Vector2(Random.Range(-sideSpeed, sideSpeed), jumpSpeed);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Gerak lompat: kecepatan ke atas berkurang terus oleh gravitasi
        velocity.y -= gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Efek pop: mulai besar, mengecil ke ukuran normal
        float popT = Mathf.Clamp01(timer / popDuration);
        transform.localScale = Vector3.one * Mathf.Lerp(popScale, 1f, popT);

        // Memudar di paruh akhir umur
        float t = timer / lifetime;
        float alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;
        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Clamp01(alpha));

        if (timer >= lifetime) Destroy(gameObject);
    }
}