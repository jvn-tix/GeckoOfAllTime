using System.Collections;
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
    [SerializeField] private float levelUpFillSpeed = 1.5f;

    [Header("Fill Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.2f;

    [Header("Level Up Punch")]
    [SerializeField] private RectTransform punchTarget; 
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.3f;

    private Vector3 punchBaseScale = Vector3.one;
    private Coroutine punchRoutine;

    private float targetFillAmount = 0f;
    private int lastLevel = -1;

    private bool waitingForFull = false;
    private float pendingTarget;
    private int pendingLevel;

    private Color baseColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        if (expFillImage != null) baseColor = expFillImage.color;
        if (punchTarget != null) punchBaseScale = punchTarget.localScale;
    }

    void Update()
    {
        if (expFillImage == null) return;

        float dt = Time.unscaledDeltaTime;

        if (waitingForFull)
        {
            // Naik ke penuh dengan kecepatan tetap
            expFillImage.fillAmount = Mathf.MoveTowards(expFillImage.fillAmount, 1f, dt * levelUpFillSpeed);

            if (expFillImage.fillAmount >= 1f)
            {
                waitingForFull = false;
                PlayFlash();                       // flash sekali saat penuh
                PlayPunch();

                expFillImage.fillAmount = 0f;
                targetFillAmount = pendingTarget;

                if (levelText != null) levelText.text = "LVL " + pendingLevel;
            }
        }
        else
        {
            expFillImage.fillAmount = Mathf.Lerp(expFillImage.fillAmount, targetFillAmount, dt * fillSpeed);
        }
    }

    public void UpdateExpUI(int currentExp, int maxExp, int currentLevel)
    {
        float newTarget = maxExp > 0 ? (float)currentExp / maxExp : 0f;

        // Pertama kali dipanggil: tanpa flash
        if (lastLevel < 0)
        {
            lastLevel = currentLevel;
            targetFillAmount = newTarget;
            if (levelText != null) levelText.text = "LVL " + currentLevel;
            return;
        }

        if (currentLevel > lastLevel)
        {
            lastLevel = currentLevel;
            waitingForFull = true;
            pendingTarget = newTarget;
            pendingLevel = currentLevel;
            return;
        }

        if (waitingForFull)
        {
            pendingTarget = newTarget;
            pendingLevel = currentLevel;
            return;
        }

        // Dapat EXP biasa: flash sekali kalau bar bertambah
        if (newTarget > targetFillAmount)
            PlayFlash();

        targetFillAmount = newTarget;

        if (levelText != null)
            levelText.text = "LVL " + currentLevel;
    }

    private void PlayFlash()
    {
        if (expFillImage == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.unscaledDeltaTime;
            // Mulai dari flashColor, kembali mulus ke warna asli
            expFillImage.color = Color.Lerp(flashColor, baseColor, timer / flashDuration);
            yield return null;
        }

        expFillImage.color = baseColor;
        flashRoutine = null;
    }

    private void PlayPunch()
    {
        if (punchTarget == null) return;
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        float timer = 0f;

        while (timer < punchDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / punchDuration;

            // Naik ke puncak lalu turun lagi ke ukuran asli (0 -> 1 -> 0)
            float pulse = Mathf.Sin(t * Mathf.PI);
            punchTarget.localScale = punchBaseScale * Mathf.Lerp(1f, punchScale, pulse);

            yield return null;
        }

        punchTarget.localScale = punchBaseScale;
        punchRoutine = null;
    }
}