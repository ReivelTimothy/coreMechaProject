using UnityEngine;

public class EnemyPatrolFSM : TrapBase
{
    public enum State
    {
        Patrol,
        Chase,
        Return
    }

    [Header("FSM State")]
    public State currentState = State.Patrol;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float patrolSpeed = 3f;
    protected int currentWaypointIndex = 0;

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float detectionRadius = 5f;
    public float stopChaseRadius = 8f;

    protected Transform playerTransform;
    protected Vector2 startPatrolPosition;

    protected virtual void Start()
    {
        startPatrolPosition = transform.position;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // --- FSM LOGIC ---
        switch (currentState)
        {
            case State.Patrol:
                HandlePatrol();
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                HandleChase();
                if (distanceToPlayer > stopChaseRadius)
                {
                    currentState = State.Return;
                }
                break;

            case State.Return:
                HandleReturn();
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = State.Chase;
                }
                break;
        }
    }

    protected virtual void HandlePatrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    protected virtual void HandleChase()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
    }

    protected virtual void HandleReturn()
    {
        Transform target = (waypoints != null && waypoints.Length > 0) ? waypoints[currentWaypointIndex] : null;
        Vector3 returnTarget = target != null ? target.position : (Vector3)startPatrolPosition;

        transform.position = Vector2.MoveTowards(transform.position, returnTarget, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, returnTarget) < 0.1f)
        {
            currentState = State.Patrol;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRadius);
    }
}