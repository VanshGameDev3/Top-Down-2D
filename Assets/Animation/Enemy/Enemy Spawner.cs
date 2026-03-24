using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 2f;
    [SerializeField] private ArenaController arena;

    private GameObject enemyPrefab;
    private int maxEnemies;
    private int spawned;
    private float timer;
    private bool active;

    void Update()
    {
        if (!active || spawned >= maxEnemies)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            EnemyAi ai = enemy.GetComponent<EnemyAi>();
            if (ai != null)
            {
                ai.SetArena(ArenaState.ActiveArena);   
            }

            spawned++;
            timer = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    public void StartSpawning(GameObject prefab, int count)
    {
        enemyPrefab = prefab;
        maxEnemies = count;
        spawned = 0;
        active = true;
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    public void StopSpawning()
    {
        active = false;
    }
}