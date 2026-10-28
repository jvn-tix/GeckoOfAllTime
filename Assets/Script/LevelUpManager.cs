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
    [SerializeField] private TurretController playerAttack;

    [Header("Audio")]
    [SerializeField] private AudioClip levelUpSfx;

    public void TriggerLevelUp()
    {
        ShowUpgradeScreen();
    }

    private void ShowUpgradeScreen()
    {
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(levelUpSfx);
        levelUpPanel.SetActive(true);

        GenerateOptions();
    }

    private void GenerateOptions()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Pilih 2 power up unik, lalu roll preset-nya SEKALI di sini
        List<PowerUpData> chosen = GetRandomPowerUps(2);

        foreach (PowerUpData data in chosen)
        {
            PowerUpOffer offer = data.CreateOffer();

            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            PowerUpCardUI cardUI = newCard.GetComponent<PowerUpCardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(offer, this);
            }
        }
    }

    public void SelectPowerUp(PowerUpOffer offer)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        ApplyPowerUp(offer);

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ApplyPowerUp(PowerUpOffer offer)
    {
        if (playerAttack == null) return;

        switch (offer.data.type)
        {
            case PowerUpType.Damage:
                playerAttack.AddDamage(offer.finalValue);
                break;

            case PowerUpType.FireRate:
                playerAttack.AddFireRate(offer.finalValue);
                break;

            case PowerUpType.AdditionalProjectile:
                playerAttack.AddProjectileCount((int)offer.finalValue);
                break;

            case PowerUpType.ClusterBullet:
                playerAttack.EnableCluster(offer.finalValue);
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
            pool.RemoveAt(randomIndex);
        }

        return result;
    }
}