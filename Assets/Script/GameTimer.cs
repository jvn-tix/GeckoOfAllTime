using UnityEngine;
using TMPro; // Diperlukan untuk TextMeshPro

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeRemaining = 600f; // 10 menit = 600 detik
    private bool isTimerRunning = false;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Over / Win UI (Optional)")]
    [SerializeField] private GameObject victoryPanel; // Panel yang muncul saat menang (opsional)

    void Start()
    {
        // Mulai timer saat game berjalan
        isTimerRunning = true;
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
        timeToDisplay += 1;

        // Hitung Menit dan Detik
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if(timeToDisplay <= 60f)
        {
            timerText.color = Color.red;
        }
    }

    private void OnTimeExpired()
    {
        Debug.Log("Waktu Habis! Player Menang!");

        // Hentikan jalannya game
        Time.timeScale = 0f;

        // Tampilkan Win Panel jika ada
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }
}