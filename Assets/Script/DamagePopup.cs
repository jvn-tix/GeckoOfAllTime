using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    [Header("Movement")]
    [SerializeField] private float jumpSpeed = 4f;
    [SerializeField] private float sideSpeed = 1.5f;
    [SerializeField] private float gravity = 14f;

    [Header("Timing & Pop Effect")]
    [SerializeField] private float lifetime = 0.7f;
    [SerializeField] private float popDuration = 0.12f;
    [SerializeField] private float initialPopMultiplier = 1.5f; // Seberapa besar efek "pop" melar di awal

    [Header("Damage Scaling")]
    [SerializeField] private float globalSizeMultiplier = 1f;  // Atur ini di Unity buat gedein/kecilin semua teks secara instan
    [SerializeField] private float damageScaleFactor = 0.1f;   // Makin besar angka ini, makin drastis perbedaan teks kecil vs besar

    private Vector2 velocity;
    private float timer;
    private Color baseColor;
    private float targetScale = 1f;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshPro>();
        baseColor = text.color;
    }

    public void Setup(int damage)
    {
        text.text = damage.ToString();
        velocity = new Vector2(Random.Range(-sideSpeed, sideSpeed), jumpSpeed);

        // Menghitung ukuran berdasarkan besar damage + dikali pengatur dari inspector
        // Contoh: damage 10 -> skala 1, damage 50 -> skala jauh lebih besar
        targetScale = globalSizeMultiplier * (1f + (damage * damageScaleFactor));
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Gerak lompat
        velocity.y -= gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Efek pop (mulai dari lebih besar, lalu mengecil ke targetScale)
        float popT = Mathf.Clamp01(timer / popDuration);
        float currentScale = Mathf.Lerp(targetScale * initialPopMultiplier, targetScale, popT);
        transform.localScale = Vector3.one * currentScale;

        // Memudar di paruh akhir umur
        float t = timer / lifetime;
        float alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;
        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Clamp01(alpha));

        if (timer >= lifetime) Destroy(gameObject);
    }
}