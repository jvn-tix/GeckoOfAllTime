using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image expFillImage;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Animation Settings")]
    [SerializeField] private float fillSpeed = 5f;
    private float targetFillAmount = 0f;

    void Update()
    {
        if (expFillImage != null)
        {
            // Lerp bergerak mulus menuju targetFillAmount
            expFillImage.fillAmount = Mathf.Lerp(expFillImage.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        }
    }

    public void UpdateExpUI(int currentExp, int maxExp, int currentLevel)
    {
        if (maxExp > 0)
        {
            // UBAH BAGIAN INI: Set targetFillAmount, jangan langsung set expFillImage.fillAmount!
            targetFillAmount = (float)currentExp / maxExp;
        }

        if (levelText != null)
        {
            levelText.text = "LVL " + currentLevel;
        }
    }
}