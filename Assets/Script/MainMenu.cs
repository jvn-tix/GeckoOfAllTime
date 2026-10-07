using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Main";

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}