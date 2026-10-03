using UnityEngine;

public class PlayerExp : MonoBehaviour
{
    [Header("Level & EXP Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToNextLevel = 100;
    [SerializeField] private float expGrowthFactor = 1.2f; // Pengali target EXP tiap level naik

    [Header("Pickup Radius")]
    [SerializeField] private float pickupRadius = 2.5f;
    [SerializeField] private LayerMask expOrbLayer;

    [Header("UI Reference")]
    [SerializeField] private ExpBarUI expBarUI;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // Deteksi Orb EXP di sekitar Player
        DetectAndAttractOrbs();
    }

    private void DetectAndAttractOrbs()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D hit in hitColliders)
        {
            // Cari script ExpOrb di object yang kena radius, atau di parent/child-nya
            ExpOrb orb = hit.GetComponent<ExpOrb>();
            if (orb == null) orb = hit.GetComponentInParent<ExpOrb>();

            if (orb != null)
            {
                orb.StartAttracting(transform);
            }
        }
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;

        // Cek apakah mencapai batas Level Up
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthFactor);

        Debug.Log("LEVEL UP! Sekarang Level: " + currentLevel);

        // Nanti bisa dipanggil Pop-up Upgrade Weapon/Skill di sini!
    }

    private void UpdateUI()
    {
        if (expBarUI != null)
        {
            expBarUI.UpdateExpUI(currentExp, expToNextLevel, currentLevel);
        }
    }

    // Visualisasi radius penyerapan Orb di Editor Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}