using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private void Start()
    {
        menu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        bool opening = !menu.activeSelf;

        menu.SetActive(opening);

        if (opening)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        PauseController.SetPause(true);
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        PauseController.SetPause(false);
        Time.timeScale = 1f;
    }
    private void OnDisable()
    {
        PauseController.SetPause(false);
        Time.timeScale = 1f;
    }
}