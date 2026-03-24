using System.Collections;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    [Header("Path")]
    public Transform WayPointParent;
    public float movespeed = 2f;
    public float waitTime = 1f;
    public bool loopWapPoint = true;

    [Header("Default Idle Facing (if never moved)")]
    public Vector2 idleFacing = new Vector2(0, -1);

    private Transform[] waypoints;
    private int currentWayPoint;
    private bool isWaiting;

    private Animator animator;
    private NPC npc;         

    private Vector2 lastDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        npc = GetComponent<NPC>();   

        waypoints = new Transform[WayPointParent.childCount];
        for (int i = 0; i < WayPointParent.childCount; i++)
        {
            waypoints[i] = WayPointParent.GetChild(i);
        }

        lastDirection = idleFacing;
        ApplyFacing(lastDirection);
    }

    void Update()
    {
        bool inDialogue = npc != null && npc.IsInDialogue;

        if (PauseController.isGamePaused || isWaiting || inDialogue)
        {
            StopAnimation();
            return;
        }

        MoveToWayPoint();
    }

    void MoveToWayPoint()
    {
        Transform target = waypoints[currentWayPoint];

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > 0.05f)
        {
            Vector2 direction = (target.position - transform.position).normalized;

            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                movespeed * Time.deltaTime
            );

            UpdateAnimation(direction);
        }
        else
        {
            StartCoroutine(WaitAtWayPoint());
        }
    }

    void UpdateAnimation(Vector2 direction)
    {
        if (animator == null)
            return;

        lastDirection = direction;

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    void StopAnimation()
    {
        ApplyFacing(lastDirection);
    }

    void ApplyFacing(Vector2 dir)
    {
        if (animator == null)
            return;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
    }

    IEnumerator WaitAtWayPoint()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        currentWayPoint = loopWapPoint
            ? (currentWayPoint + 1) % waypoints.Length
            : Mathf.Min(currentWayPoint + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}