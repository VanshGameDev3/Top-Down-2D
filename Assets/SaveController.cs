using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SaveController : MonoBehaviour
{
    public static SaveController Instance;

    private SaveData data;
    private string path;
    private bool isTransitioning = false;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("Save Path: " + Application.persistentDataPath);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        path = Application.persistentDataPath + "/save.json";

        if (File.Exists(path))
            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        else
        {
            data = new SaveData();
            SaveGame();
        }

        StartCoroutine(DelayedLoad());
    }

    public bool IsArenaCompleted(string id)
    {
        return data.completedArenas.Contains(id);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartNewGame()
    {
        if (File.Exists(path))
            File.Delete(path);

        data = new SaveData();

        SaveGame();
    }

    public void LoadGame()
    {
        if (!File.Exists(path))
        {
            StartNewGame();
            return;
        }

        data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        StartCoroutine(ApplySaveNextFrame());
    }

    private IEnumerator ApplySaveNextFrame()
    {
        yield return new WaitForSeconds(0.05f);

        Time.timeScale = 1f;

        ArenaState.IsArenaActive = false;
        ArenaState.ActiveArena = null;

        foreach (var arena in FindObjectsOfType<ArenaController>())
        {
            arena.ApplySaveState(
                data.completedArenas.Contains(arena.ArenaId)
            );
        }

        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            
            LoadPlayerState(player.transform);

           
            var move = player.GetComponent<Movement>();

            if (move != null)
            {
                move.EnableControls();        
            }

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        RestoreCameraBoundary();
        PauseController.ForceUnpause();
    }

    public void SaveButton()
    {
        var player = GameObject.FindWithTag("Player");

        if (player != null)
            SavePlayerState(player.transform.position, data.currentMapId);

        SaveGame();
    }

    public void LoadButton()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        StartCoroutine(FullReload());
    }

    private IEnumerator FullReload()
    {
        Time.timeScale = 1f;
        PauseController.ForceUnpause();

        foreach (var a in FindObjectsOfType<ArenaController>())
            a.ForceReset();

        foreach (var e in FindObjectsOfType<EnemyAi>())
            Destroy(e.gameObject);

        ArenaState.IsArenaActive = false;
        ArenaState.ActiveArena = null;

        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene(2);

        yield return new WaitForSeconds(0.1f);

        Time.timeScale = 1f;
        isTransitioning = false;

        LoadGame();
    }

    public void ExitButton()
    {
        SaveButton();
        SceneManager.LoadScene(0);
    }

    public void SavePlayerState(Vector3 position, string mapId)
    {
        data.playerPosition = position;

        if (!string.IsNullOrEmpty(mapId))
            data.currentMapId = mapId;

        SaveGame();
    }

    public void LoadPlayerState(Transform player)
    {
        if (data.playerPosition != Vector3.zero)
            player.position = data.playerPosition;
    }

    public void MarkArenaCompleted(string id)
    {
        if (!data.completedArenas.Contains(id))
            data.completedArenas.Add(id);

        SaveGame();
    }
    public int GetCompletedArenaCount()
    {
        return data.completedArenas.Count;
    }

    private void RestoreCameraBoundary()
    {
        if (string.IsNullOrEmpty(data.currentMapId))
            return;

        foreach (var map in FindObjectsOfType<MapIdentifier>())
        {
            if (map.mapId == data.currentMapId)
            {
                map.GetComponent<MapTransition>()?.ForceApplyBoundary();
                break;
            }
        }
    }

    public void DeleteSave()
    {
        Debug.Log("DELETING SAVE FILE");

        if (File.Exists(path))
            File.Delete(path);

        data = new SaveData();

        SaveGame();
    }

    private void SaveGame()
    {
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isTransitioning = false;

        ArenaState.IsArenaActive = false;
        ArenaState.ActiveArena = null;

        Debug.Log("Scene Loaded -> ArenaState Reset");
    }

#if UNITY_EDITOR
    [ContextMenu("DEV: Delete Save File")]
#endif
    public void DeleteSaveForDeveloper()
    {
        string devPath = Application.persistentDataPath + "/save.json";

        Debug.Log("=== DEV: DELETING SAVE FILE ===");

        if (File.Exists(devPath))
        {
            File.Delete(devPath);
            Debug.Log("Save file deleted at: " + devPath);
        }
        else
        {
            Debug.Log("No save file found to delete.");
        }

        data = new SaveData();

        if (Application.isPlaying)
        {
            SaveGame();
            ArenaState.IsArenaActive = false;
            ArenaState.ActiveArena = null;
        }

        Debug.Log("Save reset complete.");
    }
    private IEnumerator DelayedLoad()
    {
        yield return null;
        LoadGame();
    }
    public string GetCurrentMapId()
    {
        return data.currentMapId;
    }
}