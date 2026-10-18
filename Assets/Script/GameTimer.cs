using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeRemaining = 300f; // 5 menit = 300 detik
    private bool isTimerRunning = false;
    public float TimeRemaining => timeRemaining;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Urgent Settings (10 Seconds Left)")]
    [SerializeField] private float urgentThreshold = 10f; // Mulai efek panik saat 10 detik terakhir
    [SerializeField] private float maxScaleMultiplier = 1.4f; // Seberapa besar loncatan skalanya
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip tickSfx;
    [SerializeField, Range(0f, 1f)] private float tickVolume = 1f;

    private Vector3 originalScale;
    private int lastSecondInt = -1;
    private float pulseTimer = 0f;
    private float pulseDuration = 0.3f; // Durasi animasi denyut per detik

    void Start()
    {
        // Mulai timer saat game berjalan
        isTimerRunning = true;

        if (timerText != null)
        {
            originalScale = timerText.transform.localScale;
            timerText.color = normalColor;
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                // Kurangi waktu tiap frame
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                // Waktu Habis!
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerDisplay(timeRemaining);
                OnTimeExpired();
            }
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        // Tambahkan sedikit offset agar tampilan tidak negatif
        float displayValue = timeToDisplay + 1;

        // Hitung Menit dan Detik
        float minutes = Mathf.FloorToInt(displayValue / 60);
        float seconds = Mathf.FloorToInt(displayValue % 60);
        int currentSecondInt = Mathf.FloorToInt(displayValue);
        
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Cek apakah sudah masuk 10 detik terakhir
            if (timeToDisplay <= urgentThreshold && timeToDisplay > 0)
            {
                timerText.color = warningColor;

                // Jika detik berubah, reset timer denyut untuk efek "jedug" pas detik baru mulai
                if (currentSecondInt != lastSecondInt)
                {
                    pulseTimer = pulseDuration;
                    lastSecondInt = currentSecondInt;

                    // Memanggil SFX dari AudioManager yang sudah ada di scene
                    if (tickSfx != null && AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX(tickSfx, tickVolume);
                    }
                }

                // Jalankan animasi denyut mengecil kembali ke ukuran normal
                if (pulseTimer > 0)
                {
                    pulseTimer -= Time.deltaTime;
                    float progress = pulseTimer / pulseDuration; // Dari 1 ke 0
                    // Ukuran membesar di awal detik, lalu melandai kembali ke originalScale
                    float currentScale = Mathf.Lerp(1f, maxScaleMultiplier, progress);
                    timerText.transform.localScale = originalScale * currentScale;
                }
                else
                {
                    timerText.transform.localScale = originalScale;
                }
            }
            else if (timeToDisplay <= 60f)
            {
                timerText.transform.localScale = originalScale;
                timerText.color = warningColor;
            }
            else
            {
                timerText.transform.localScale = originalScale;
                timerText.color = normalColor;
            }
        }
    }

    private void OnTimeExpired()
    {
        // Debug.Log("Waktu Habis! Player Menang!");

        // Hentikan jalannya game
        Time.timeScale = 0f;

        // Kembalikan skala text ke normal biar ga nyangkut pas game over
        if (timerText != null)
        {
            timerText.transform.localScale = originalScale;
        }

        // Tampilkan Win Panel jika ada
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerVictory();
        }
    }
}