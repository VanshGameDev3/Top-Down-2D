using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool isGamePaused { get; private set; }

    public static void SetPause(bool pause)
    {
        isGamePaused = pause;
    }

    [Header("Optional UI")]
    [SerializeField] private GameObject pauseMenuUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isGamePaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isGamePaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI)
            pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        isGamePaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI)
            pauseMenuUI.SetActive(false);
    }

    public static void ForceUnpause()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
}