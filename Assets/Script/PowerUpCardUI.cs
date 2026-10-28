using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private PowerUpOffer currentOffer;
    private LevelUpManager levelUpManager;

    public void Setup(PowerUpOffer offer, LevelUpManager manager)
    {
        currentOffer = offer;
        levelUpManager = manager;

        PowerUpData data = offer.data;

        if (titleText != null)
        {
            // Contoh: "Damage (Large)"
            titleText.text = offer.tierLabel != null
                ? $"{data.title} ({offer.tierLabel})"
                : data.title;
        }

        if (descriptionText != null)
            descriptionText.text = BuildDescription(offer);

        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelected);
    }

    private string BuildDescription(PowerUpOffer offer)
    {
        switch (offer.data.type)
        {
            case PowerUpType.Damage:
                return $"Damage +{offer.finalValue:0.##}";

            case PowerUpType.FireRate:
                return $"Fire rate +{offer.finalValue:0.##}";

            default:
                // AdditionalProjectile & ClusterBullet pakai deskripsi dari Inspector
                return offer.data.description;
        }
    }

    private void OnSelected()
    {
        levelUpManager.SelectPowerUp(currentOffer);
    }
}