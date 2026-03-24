using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    [Header("Timings")]
    public float waitBeforeEnd = 4f;
    public float thankYouDuration = 4f;

    private IEnumerator Start()
    {
        if (MapAudioManager.Instance != null)
            MapAudioManager.Instance.PlayEndingMusic();

        yield return new WaitForSeconds(waitBeforeEnd);

        if (SaveController.Instance != null)
            SaveController.Instance.DeleteSave();

        yield return new WaitForSeconds(thankYouDuration);

        if (MapAudioManager.Instance != null)
            MapAudioManager.Instance.StopMusic();

        SceneManager.LoadScene(1);
    }
}