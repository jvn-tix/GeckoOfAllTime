using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private PowerUpData currentData;
    private LevelUpManager levelUpManager;

    public void Setup(PowerUpData data, LevelUpManager manager)
    {
        currentData = data;
        levelUpManager = manager;

        if (titleText != null) titleText.text = data.title;
        if (descriptionText != null) descriptionText.text = data.description;
        if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelected);
    }

    private void OnSelected()
    {
        levelUpManager.SelectPowerUp(currentData);
    }
}