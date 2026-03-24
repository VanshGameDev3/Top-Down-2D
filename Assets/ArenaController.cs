using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ArenaController : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string arenaId;

    public string ArenaId => arenaId;
    public bool IsCompleted => arenaCompleted;

    [System.Serializable]
    public class Wave
    {
        public string waveText;
        public GameObject enemyPrefab;
        public int enemyCount;
    }

    [Header("Arena Setup")]
    [SerializeField] private EnemySpawner[] spawners;
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 2f;

    [Header("Arena Flow")]
    [SerializeField] private Transform arenaEntry;
    [SerializeField] private Transform arenaExit;

    [Header("Camera")]
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private PolygonCollider2D arenaBounds;
    [SerializeField] private PolygonCollider2D worldBounds;
    [SerializeField] private CinemachineCamera cam;

    [Header("UI")]
    [SerializeField] private ArenaUIController arenaUI;

    private int currentWave;
    private int enemiesRemaining;
    private bool arenaStarted;
    private bool arenaCompleted;

    private void Awake()
    {
        arenaCompleted = false;
        arenaStarted = false;
    }

    public void TriggerArenaStart(Transform player)
    {
        if (arenaCompleted)
        {
            Debug.Log("Arena already completed: " + arenaId);
            return;
        }

        if (arenaStarted)
            return;

        arenaStarted = true;
        ArenaState.IsArenaActive = true;
        ArenaState.ActiveArena = this;

        player.position = arenaEntry.position;

        confiner.BoundingShape2D = arenaBounds;
        confiner.InvalidateBoundingShapeCache();

        cam.Priority = 50;

        ForceCameraRefresh(player.position);

        MapAudioManager.Instance?.PlayArenaMusic();

        StartWave(0);
    }

    private void StartWave(int index)
    {
        currentWave = index;
        enemiesRemaining = waves[index].enemyCount;

        arenaUI?.ShowWaveText(waves[index].waveText);

        int total = waves[index].enemyCount;
        int perSpawner = total / spawners.Length;
        int remainder = total % spawners.Length;

        for (int i = 0; i < spawners.Length; i++)
        {
            int count = perSpawner + (i < remainder ? 1 : 0);

            if (count > 0)
                spawners[i].StartSpawning(waves[index].enemyPrefab, count);
        }
    }

    public void NotifyEnemyKilled()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
            StartCoroutine(NextWave());
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;

        if (currentWave < waves.Length)
            StartWave(currentWave);
        else
            CompleteArena();
    }

    private void CompleteArena()
    {
        Debug.Log($" ARENA COMPLETE: {arenaId} ");
        Debug.Log("arenaExit: " + arenaExit);
        Debug.Log("worldBounds: " + worldBounds);
        Debug.Log("confiner: " + confiner);
        Debug.Log("cam: " + cam);
        Debug.Log("arenaUI: " + arenaUI);

        arenaCompleted = true;
        arenaStarted = false;

        Debug.Log(" ARENA COMPLETE: " + arenaId);

        if (arenaExit == null)
            Debug.LogError(" arenaExit NOT ASSIGNED in " + arenaId);

        if (confiner == null)
            Debug.LogError(" confiner NOT ASSIGNED in " + arenaId);

        if (worldBounds == null)
            Debug.LogError(" worldBounds NOT ASSIGNED in " + arenaId);

        if (cam == null)
            Debug.LogError("? cam NOT ASSIGNED in " + arenaId);

        if (arenaUI == null)
            Debug.LogError("? arenaUI NOT ASSIGNED in " + arenaId);

        MapAudioManager.Instance?.ReturnToMapMusic();

        ArenaState.IsArenaActive = false;
        ArenaState.ActiveArena = null;

        arenaUI?.ShowArenaCompleted();

        SaveController.Instance?.MarkArenaCompleted(arenaId);
        FindAnyObjectByType<ArenaProgressUI>()?.RefreshUI();

        if (AreAllArenasCompleted())
        {
            StartCoroutine(EndGameFlow());
            return;
        }

        StartCoroutine(ReturnToWorld());
    }

    private bool AreAllArenasCompleted()
    {
        if (SaveController.Instance == null)
        {
            Debug.LogError("SaveController.Instance is NULL!");
            return false;
        }

        int totalArenas = 4;
        int done = SaveController.Instance.GetCompletedArenaCount();

        Debug.Log($"Completed {done} / {totalArenas}");

        return done >= totalArenas;
    }

    private IEnumerator EndGameFlow()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(3);
    }

    private IEnumerator ReturnToWorld()
    {
        yield return null;

        var playerObj = GameObject.FindWithTag("Player");

        if (playerObj == null)
        {
            Debug.LogError("PLAYER NOT FOUND during arena exit!");
            yield break;
        }

        Transform player = playerObj.transform;

        ArenaState.IsArenaActive = false;
        ArenaState.ActiveArena = null;
        arenaStarted = false;

        player.position = arenaExit.position;

        player.GetComponent<Movement>()?.Respawn();

        confiner.BoundingShape2D = worldBounds;
        confiner.InvalidateBoundingShapeCache();

        StartCoroutine(FixCameraNextFrame(arenaExit.position));

        string mapId = SaveController.Instance?.GetCurrentMapId();

        if (!string.IsNullOrEmpty(mapId))
        {
            MapAudioManager.Instance?.ForceMapMusic(mapId);
        }

        Debug.Log("Returned to map from arena: " + arenaId);
    }

    private IEnumerator FixCameraNextFrame(Vector3 pos)
    {
        yield return null;
        yield return null; // IMPORTANT second frame

        confiner.BoundingShape2D = worldBounds;
        confiner.InvalidateBoundingShapeCache();

        cam.ForceCameraPosition(
            new Vector3(pos.x, pos.y, cam.transform.position.z),
            cam.transform.rotation
        );
    }

    private void ForceCameraRefresh(Vector3 pos)
    {
        confiner.InvalidateBoundingShapeCache();

        cam.ForceCameraPosition(
            new Vector3(pos.x, pos.y, cam.transform.position.z),
            cam.transform.rotation
        );
    }

    public void ApplySaveState(bool completed)
    {
        arenaCompleted = completed;
        arenaStarted = false;

        gameObject.SetActive(true);

        if (!completed)
        {
            ResetArenaCompletely();
        }
    }

    public void ForceReset()
    {
        arenaStarted = false;
        enemiesRemaining = 0;
        currentWave = 0;

        StopAllCoroutines();
    }
    private void ResetArenaCompletely()
    {
        foreach (var e in FindObjectsOfType<EnemyAi>())
            Destroy(e.gameObject);

        currentWave = 0;
        enemiesRemaining = 0;

        StopAllCoroutines();

        Debug.Log("ARENA RESET AFTER DEATH: " + arenaId);
    }

    public void OnPlayerDied()
    {
        Debug.Log("PLAYER DIED IN ARENA: " + arenaId);

        arenaStarted = false;

        ArenaState.IsArenaActive = false;
        ArenaState.ActiveArena = null;

        foreach (var e in FindObjectsOfType<EnemyAi>())
            Destroy(e.gameObject);
        
        currentWave = 0;
        enemiesRemaining = 0;

        StopAllCoroutines();

        arenaUI?.ShowPlayerDied();

        StartCoroutine(ReturnToWorld());
    }
}