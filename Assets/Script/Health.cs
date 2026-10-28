using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

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

    [Header("Flash Settings (Only if material has _FlashAmount)")]
    [SerializeField, Range(0f, 1f)] private float flashAmount = 0.5f;
    private MaterialPropertyBlock propertyBlock;
    private bool useFlashMaterial;
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    [Header("Hit Squish Settings")]
    [SerializeField] private bool enableSquish = true;
    [SerializeField] private float squishDuration = 0.15f;
    [SerializeField] private Vector2 squishScale = new Vector2(1.25f, 0.75f); // X melebar, Y memipih

    [Header("UI Reference (Only For Player)")]
    [SerializeField] private PlayerHealthBar healthBar;

    [Header("Camera Shake Settings (Only For Player)")]
    [SerializeField] private bool shakeCameraOnHit = false;
    [SerializeField] private float shakeForce = 0.3f;

    [Header("Drop Settings (Only For Enemy)")]
    [SerializeField] private GameObject expOrbPrefab;

    [Header("Audio (Only For Enemy)")]
    [SerializeField] private AudioClip deathSfx;
    [SerializeField, Range(0f, 1f)] private float deathSfxVolume = 1f;

    [Header("Damage Popup (Only For Enemy)")]
    [SerializeField] private DamagePopup damagePopupPrefab;

    [Header("Health Scaling (Only For Enemy)")]
    [SerializeField] private bool ScaleWithGeckLevel = false;
    [SerializeField] private float HeatlthPerLevel = 0.1f;
    private Animator animator;
    private Vector3 originalScale;
    private Coroutine squishRoutine;
    private bool isInitialized = false;
    private int baseMaxHealth;
    private CinemachineImpulseSource impulseSource;


    private void InitScale()
    {
        if (!isInitialized)
        {
            originalScale = transform.localScale;
            isInitialized = true;
        }
    }

    void Awake()
    {
        baseMaxHealth = maxHealth;
        currentHealth = maxHealth;
        originalScale = transform.localScale;

        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;

            useFlashMaterial = spriteRenderer.sharedMaterial != null
                               && spriteRenderer.sharedMaterial.HasProperty(FlashAmountID);

            if (useFlashMaterial) propertyBlock = new MaterialPropertyBlock();
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<PlayerHealthBar>();
        }

        if (ScaleWithGeckLevel)
        {
            ApplyHealthScaling();
        }

        UpdateUI();

    }

    private void ApplyHealthScaling()
    {
        if(PlayerExp.Instance == null) return;

        int level = PlayerExp.Instance.GetCurrentLevel();
        float multiplier = 1f + (level - 1) * HeatlthPerLevel;

        maxHealth = Mathf.RoundToInt(baseMaxHealth * multiplier);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        InitScale(); // Pastikan skala asli tersimpan

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(animator != null && currentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }

        UpdateUI();

        if (damagePopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), 0.5f, 0f);
            DamagePopup popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            popup.Setup(damage);
        }

        if (spriteRenderer != null)
        {
            StartCoroutine(HitFlashRoutine());
        }

        if (enableSquish)
        {
            // Hentikan coroutine squish sebelumnya agar tidak numpuk & nyangkut
            if (squishRoutine != null) StopCoroutine(squishRoutine);
            squishRoutine = StartCoroutine(HitSquishRoutine());
        }

        if(impulseSource != null && shakeCameraOnHit)
        {
            impulseSource.GenerateImpulseWithForce(shakeForce);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        int healAmount = Mathf.RoundToInt(amount);
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateUI();
    }
    private IEnumerator HitFlashRoutine()
    {
        if (useFlashMaterial) SetFlash(flashAmount);
        else spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitColorDuration);

        if (useFlashMaterial) SetFlash(0f);
        else spriteRenderer.color = originalColor;
    }

    private IEnumerator HitSquishRoutine()
    {
        InitScale();

        // Selalu mulai dari ukuran asli yang bersih (mencegah penumpukan gepeng)
        transform.localScale = originalScale;

        // Hitung target skala gepeng berdasarkan originalScale asli
        Vector3 squishedScale = new Vector3(originalScale.x * squishScale.x, originalScale.y * squishScale.y, originalScale.z);

        // Ubah instan ke bentuk gepeng
        transform.localScale = squishedScale;

        float timer = 0f;
        while (timer < squishDuration)
        {
            timer += Time.deltaTime;
            float t = timer / squishDuration;
            
            // Kembali secara mulus dari bentuk gepeng ke ukuran original
            transform.localScale = Vector3.Lerp(squishedScale, originalScale, t);
            yield return null;
        }

        // Pastikan di akhir benar-benar kembali normal
        transform.localScale = originalScale;
        squishRoutine = null;
    }

    private void SetFlash(float amount)
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FlashAmountID, amount);
        spriteRenderer.SetPropertyBlock(propertyBlock);
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
        if(animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (CompareTag("Player"))
        {
            // Debug.Log("Player Mati! Game Over.");
            gameObject.SetActive(false);
            if(GameOverManager.Instance != null)
            {
                GameOverManager.Instance.TriggerGameOver();
            }
        }
        else
        {
            Collider2D coll = GetComponent<Collider2D>();
            if(coll != null) coll.enabled = false;
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(deathSfx, deathSfxVolume);
            if (expOrbPrefab != null)
            {
                Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject, 0.5f);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}