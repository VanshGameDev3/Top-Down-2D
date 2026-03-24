using UnityEngine;
using System.Collections.Generic;

public class MapAudioManager : MonoBehaviour
{
    public static MapAudioManager Instance;

    [System.Serializable]
    public class MapSound
    {
        public string mapId;
        public AudioClip music;
    }

    public AudioSource audioSource;
    public List<MapSound> mapSounds = new();

    [Header("Arena")]
    public AudioClip arenaMusic;

    [Header("Ending")]
    public AudioClip endingMusic;

    private string currentMap = "";
    private bool inArena = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource.loop = true;
        audioSource.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }
    public void ReturnToMapMusic()
    {
        inArena = false;

        if (string.IsNullOrEmpty(currentMap))
            return;

        foreach (var map in mapSounds)
        {
            if (map.mapId == currentMap)
            {
                audioSource.clip = map.music;
                audioSource.Play();
                return;
            }
        }
    }

    public void PlayMapSound(string mapId)
    {
        if (inArena)
            inArena = false;

        if (currentMap == mapId)
            return;

        foreach (var map in mapSounds)
        {
            if (map.mapId == mapId)
            {
                currentMap = mapId;
                audioSource.clip = map.music;
                audioSource.Play();
                return;
            }
        }
    }

    public void PlayArenaMusic()
    {
        inArena = true;
        audioSource.clip = arenaMusic;
        audioSource.Play();
    }

    public void ForceMapMusic(string mapId)
    {
        inArena = false;
        currentMap = mapId;

        foreach (var map in mapSounds)
        {
            if (map.mapId == mapId)
            {
                audioSource.clip = map.music;
                audioSource.Play();
                return;
            }
        }
    }

    public void PlayEndingMusic()
    {
        inArena = false;
        currentMap = "ENDING";

        audioSource.clip = endingMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
        currentMap = "";
        inArena = false;
    }
}