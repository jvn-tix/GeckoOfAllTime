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

    [Header("Flash Settings (Only if material has _FlashAmount)")]
    [SerializeField, Range(0f, 1f)] private float flashAmount = 0.5f;
    private MaterialPropertyBlock propertyBlock;
    private bool useFlashMaterial;
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    [Header("UI Reference (Only For Player)")]
    [SerializeField] private PlayerHealthBar healthBar;

    [Header("Drop Settings (Only For Enemy)")]
    [SerializeField] private GameObject expOrbPrefab;

    [Header("Audio (Only For Enemy)")]
    [SerializeField] private AudioClip deathSfx;
    [SerializeField, Range(0f, 1f)] private float deathSfxVolume = 1f;

    [Header("Damage Popup (Only For Enemy)")]
    [SerializeField] private DamagePopup damagePopupPrefab;

    void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;

            useFlashMaterial = spriteRenderer.sharedMaterial != null
                               && spriteRenderer.sharedMaterial.HasProperty(FlashAmountID);

            if (useFlashMaterial) propertyBlock = new MaterialPropertyBlock();
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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- FUNCTION DIPANGGIL DARI LEVEL UP MANAGER ---
    public void AddMaxHealth(float amount)
    {
        int additionalHP = Mathf.RoundToInt(amount);

        // 1. Tambah Max Health
        maxHealth += additionalHP;

        // 2. Isi HP player sejumlah penambahan max HP tersebut
        currentHealth += additionalHP;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // 3. Refresh UI Health Bar
        UpdateUI();

        //Debug.Log($"{gameObject.name} Max HP bertambah {additionalHP}! Total Max HP: {maxHealth}");
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
        if (CompareTag("Player"))
        {
            //Debug.Log("Player Mati! Game Over.");
            gameObject.SetActive(false);
            if(GameOverManager.Instance != null)
            {
                GameOverManager.Instance.TriggerGameOver();
            }
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(deathSfx, deathSfxVolume);
            if (expOrbPrefab != null)
            {
                Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}