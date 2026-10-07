using UnityEngine;
using UnityEngine.InputSystem; // Diperlukan untuk InputAction.CallbackContext
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Scene Settings")]
    [Tooltip("Ketik nama scene Main Menu persis seperti di Build Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI Reference")]
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;
    private float timeScaleBeforePause = 1f;

    private void Awake()
    {
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
        // Pastikan waktu berjalan normal di awal
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // --- FUNGSI UNTUK PLAYER INPUT COMPONENT ---
    // Hubungkan fungsi ini ke Action "Pause" / "Cancel" di Player Input (Events / Message)
    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            TogglePause();
        }
    }

    // Alternatif jika menggunakan Unity Events pada Player Input Component:
    public void OnPauseEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Jangan izinkan pause jika game sedang freeze karena Level Up / Game Over / Victory
        if (Time.timeScale == 0f && !isPaused) return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f; // Freeze pergerakan & timer game
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = timeScaleBeforePause; // Kembalikan waktu normal
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Selalu kembalikan timeScale sebelum reload
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
            Debug.LogError("Nama Scene Main Menu masih kosong di Inspector PauseManager!");
        }
    }
}