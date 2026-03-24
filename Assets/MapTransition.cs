using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MapTransition : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private PolygonCollider2D mapBoundary;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Transform teleportTarget;

    [Header("Direction")]
    [SerializeField] private Direction moveDirection = Direction.Teleport;

    [Header("Arena Link (OPTIONAL)")]
    [SerializeField] private ArenaController targetArena;

    private bool isTransitioning;

    public enum Direction { Up, Down, Left, Right, Teleport }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (ArenaState.IsArenaActive)
        {
            Debug.Log("BLOCKED: ArenaState still active");
            return;
        }

        if (isTransitioning) return;

        if (targetArena != null && targetArena.IsCompleted)
        {
            Debug.Log("Arena locked (completed): " + targetArena.ArenaId);
            return;
        }

        StartCoroutine(Transition(col.transform));
    }

    private IEnumerator Transition(Transform player)
    {
        isTransitioning = true;

        MovePlayer(player);
        yield return null;

        if (confiner != null && mapBoundary != null)
        {
            confiner.BoundingShape2D = mapBoundary;
            confiner.InvalidateBoundingShapeCache();
        }

        if (mapBoundary != null)
        {
            MapIdentifier map = mapBoundary.GetComponent<MapIdentifier>();

            if (map != null)
            {
                SaveController.Instance?.SavePlayerState(
                    player.position,
                    map.mapId
                );

                MapAudioManager.Instance?.PlayMapSound(map.mapId);
            }
        }

        if (targetArena != null)
        {
            ArenaState.ActiveArena = targetArena;   
            targetArena.TriggerArenaStart(player);
        }

        isTransitioning = false;
    }

    public void ForceApplyBoundary()
    {
        if (confiner != null && mapBoundary != null)
        {
            confiner.BoundingShape2D = mapBoundary;
            confiner.InvalidateBoundingShapeCache();
        }
    }

    private void MovePlayer(Transform player)
    {
        if (moveDirection == Direction.Teleport && teleportTarget)
        {
            player.position = teleportTarget.position;
            return;
        }

        Vector3 pos = player.position;

        switch (moveDirection)
        {
            case Direction.Up: pos.y += 2f; break;
            case Direction.Down: pos.y -= 2f; break;
            case Direction.Left: pos.x -= 2f; break;
            case Direction.Right: pos.x += 2f; break;
        }

        player.position = pos;
    }
}