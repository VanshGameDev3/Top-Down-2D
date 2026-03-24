using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SaveController.Instance?.StartNewGame();
        SceneManager.LoadScene(2);
    }

    public void LoadGame()
    {
        SaveController.Instance?.LoadGame();
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}