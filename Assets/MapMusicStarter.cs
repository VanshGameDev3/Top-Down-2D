using UnityEngine;

public class MainMenuMusicStarter : MonoBehaviour
{
    private void Start()
    {
        if (MapAudioManager.Instance != null)
            MapAudioManager.Instance.ForceMapMusic("Map1");
    }
}