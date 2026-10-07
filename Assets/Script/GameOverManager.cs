using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk reload scene / ganti level

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Scene Settings")]
    [Tooltip("Ketik nama scene Main Menu persis seperti yang ada di Build Settings")]
    [SerializeField] private string mainMenuSceneName = "Home"; // Nilai default

    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Audio")]
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip gameOverSfx;

    private bool isGameFinished = false;

    private void Awake()
    {
        // Singleton pattern agar mudah dipanggil dari script lain
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Pastikan waktu berjalan normal dan panel tersembunyi di awal
        Time.timeScale = 1f;
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void TriggerVictory()
    {
        if (isGameFinished) return;
        isGameFinished = true;

        Time.timeScale = 0f; // Hentikan pergerakan game
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(victorySfx);
        
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        //Debug.Log("Game Finished: VICTORY!");
    }

    public void TriggerGameOver()
    {
        if (isGameFinished) return;
        isGameFinished = true;

        Time.timeScale = 0f; // Hentikan pergerakan game
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(gameOverSfx);
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        //Debug.Log("Game Finished: GAME OVER!");
    }

    // --- FUNGSI UNTUK BUTTON UI ---

    public void RestartGame()
    {
        Time.timeScale = 1f; // WAJIB kembalikan timeScale ke 1 sebelum reload!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Nama Scene Main Menu masih kosong di Inspector GameOverManager!");
        }
    }
}