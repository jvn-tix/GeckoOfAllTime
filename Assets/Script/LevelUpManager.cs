using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("Power Up Pool")]
    [SerializeField] private List<PowerUpData> allPowerUps = new List<PowerUpData>();

    [Header("UI References")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [Header("Player References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private TurretController playerAttack; // Sesuaikan dengan nama script attack kamu

    private List<PowerUpData> currentOfferedPowerUps = new List<PowerUpData>();

    public void TriggerLevelUp()
    {
        ShowUpgradeScreen();
    }

    private void ShowUpgradeScreen()
    {
        Time.timeScale = 0f; // Pause Game
        levelUpPanel.SetActive(true);

        GenerateOptions();
    }

    private void GenerateOptions()
    {
        // Bersihkan kartu dari pemilihan sebelumnya
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Ambil 2 opsi acak unik dari pool
        currentOfferedPowerUps = GetRandomPowerUps(2);

        // Spawn 2 Kartu UI ke layar
        foreach (PowerUpData data in currentOfferedPowerUps)
        {
            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            PowerUpCardUI cardUI = newCard.GetComponent<PowerUpCardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(data, this);
            }
        }
    }

    public void SelectPowerUp(PowerUpData selectedPowerUp)
    {
        // Tambahkan status ke Player
        ApplyPowerUp(selectedPowerUp);

        // Tutup UI & Resume Game
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ApplyPowerUp(PowerUpData powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.Damage:
                if (playerAttack != null) playerAttack.AddDamage(powerUp.value);
                break;

            case PowerUpType.FireRate:
                if (playerAttack != null) playerAttack.AddFireRate(powerUp.value);
                break;

            case PowerUpType.AdditionalProjectile:
                if (playerAttack != null) playerAttack.AddProjectileCount((int)powerUp.value);
                break;

            case PowerUpType.MaxHP:
                if (playerHealth != null) playerHealth.AddMaxHealth(powerUp.value);
                break;
        }
    }

    private List<PowerUpData> GetRandomPowerUps(int count)
    {
        List<PowerUpData> pool = new List<PowerUpData>(allPowerUps);
        List<PowerUpData> result = new List<PowerUpData>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex); // Cegah opsi ganda
        }

        return result;
    }
}