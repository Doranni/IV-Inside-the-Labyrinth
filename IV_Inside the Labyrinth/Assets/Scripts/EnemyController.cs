using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    Transform player;
    public LayerMask playerLMask, hidingObstacleLMask;
    NavMeshAgent navMeshAgent;
    ActionStatus actionStatus = ActionStatus.undefined;
    Vector3 environmentExtents, environmentCenter;
    float yRange;

    // Walking
    Vector3 walkPoint;
    bool isWalkPointSet = false;
    public float walkingSpeed;
    public float walkPointRange;

    // Idle
    float idleTime;
    bool isIdleTimeSet = false, wasIdle = false;
    public float maxIdleTime, minIdleTime;

    // Chasing
    public float seeRange, feelRange;
    bool isPlayerSeen = false;
    public float chasingSpeed;

    // Attacking
    public float attackRange;
    bool isPlayerInAttackRange = false;

    // Seaching for Player
    public float searchTime;
    float searchTimeLeft;
    public float searchPointRange;
    Vector3 searchingPoint;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        Bounds environmentBounds = GameObject.Find("Plane").GetComponent<Collider>().bounds;
        environmentExtents = environmentBounds.extents;
        environmentCenter = environmentBounds.center;
        yRange = 2 * environmentExtents.y + 0.5f;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAction();

        switch (actionStatus)
        {
            case ActionStatus.walking:
                {
                    Walking();
                    break;
                }
            case ActionStatus.idle:
                {
                    Idling();
                    break;
                }
            case ActionStatus.chasing:
                {
                    Chasing();
                    break;
                }
            case ActionStatus.attacking:
                {
                    Attaking();
                    break;
                }
            case ActionStatus.searching:
                {
                    Seaching();
                    break;
                }
        }
    }

    private void CheckPlayer()
    {
        Vector3 toPlayer = player.position - transform.position;

        // If player is inside feel range
        if (Physics.CheckSphere(transform.position, feelRange, playerLMask))
        {
            isPlayerSeen = true;
        }
        // If player is inside see range
        else if (Physics.CheckSphere(transform.position, seeRange, playerLMask))
        {
            Vector3 playerLocal = transform.InverseTransformPoint(player.position);
            bool isPlayerAhead = playerLocal.z > 0 && playerLocal.x >= - playerLocal.z
                && playerLocal.x <= playerLocal.z;
            bool isPlayerHiden = Physics.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude,
                hidingObstacleLMask);
            isPlayerSeen = isPlayerAhead && !isPlayerHiden;
            Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1)) * seeRange, Color.green);
            Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 1)) * seeRange, Color.green);
        }
        else
        {
            isPlayerSeen = false;
        }
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLMask);

        // Debug
        if (isPlayerInAttackRange)
        {
            Debug.DrawRay(transform.position, toPlayer, Color.red);
        }
        else if (isPlayerSeen)
        {
            Debug.DrawRay(transform.position, toPlayer, Color.magenta);
        }
    }

    private void ChooseAction()
    {
        CheckPlayer();
        if (isPlayerSeen)
        {
            searchingPoint = player.position;
            if (isPlayerInAttackRange)
            {
                if (actionStatus != ActionStatus.attacking)
                {
                    // Starting attacking
                    actionStatus = ActionStatus.attacking;
                    isWalkPointSet = false;
                    navMeshAgent.speed = chasingSpeed;
                    navMeshAgent.ResetPath();
                    Debug.Log(name + "'s action status: " + actionStatus);
                    return;
                }
            }
            else
            {
                if (actionStatus != ActionStatus.chasing)
                {
                    // Starting chasing
                    actionStatus = ActionStatus.chasing;
                    isWalkPointSet = false;
                    navMeshAgent.speed = chasingSpeed;
                    Debug.Log(name + "'s action status: " + actionStatus);
                    return;
                }
            }
        }
        else
        {
            if (actionStatus == ActionStatus.attacking || actionStatus == ActionStatus.chasing)
            {
                // And of attacking or chasing, starting to search for the player
                actionStatus = ActionStatus.searching;
                walkPoint = searchingPoint;
                isWalkPointSet = navMeshAgent.SetDestination(walkPoint);
                searchTimeLeft = searchTime;
                Debug.Log(name + "'s Action status: " + actionStatus.ToString());
                return;
            }
        }

        if (actionStatus == ActionStatus.undefined)
        {
            isWalkPointSet = false;
            isIdleTimeSet = false;
            navMeshAgent.ResetPath();
            navMeshAgent.speed = walkingSpeed;
            if (wasIdle)
            {
                actionStatus = ActionStatus.walking;
                wasIdle = false;
            }
            else
            {
                actionStatus = (ActionStatus)Random.Range(1, 3);
            }
            Debug.Log(name + "'s Action status: " + actionStatus.ToString());
        }
        
    }

    private void Idling()
    {
        if (isIdleTimeSet)
        {
            if (idleTime > 0)
            {
                idleTime -= Time.deltaTime;
            }
            else
            {
                wasIdle = true;
                Debug.Log(name + "'s idle time is over.");
                actionStatus = ActionStatus.undefined;
            }
        }
        else
        {
            idleTime = Random.Range(minIdleTime, maxIdleTime);
            isIdleTimeSet = true;
            Debug.Log(name + "'s idle time " + idleTime);
        }
    }

    private void Walking()
    {
        if (isWalkPointSet)
        {
            Debug.DrawRay(walkPoint, Vector3.up * 3, Color.green);
            
            if (!navMeshAgent.pathPending && !navMeshAgent.hasPath)
            {
                actionStatus = ActionStatus.undefined;
            }
        }
        else
        {
            ChooseWalkPoint(transform.position, walkPointRange);
        }
    }

    private void Chasing()
    {
        navMeshAgent.SetDestination(player.position);
    }

    private void Attaking()
    {
        // TODO: To complete Attaking method
    }

    private void Seaching()
    {
        if (searchTimeLeft > 0)
        {
            searchTimeLeft -= Time.deltaTime;
            if (isWalkPointSet)
            {
                Debug.DrawRay(walkPoint, Vector3.up * 3, Color.yellow);

                if (!navMeshAgent.pathPending && !navMeshAgent.hasPath)
                {
                    isWalkPointSet = false;
                    ChooseWalkPoint(searchingPoint, searchPointRange);
                }
            }
            else
            {
                ChooseWalkPoint(searchingPoint, searchPointRange);
            }
        }
        else
        {
            Debug.Log(name + "'ve lost the player.");
            actionStatus = ActionStatus.undefined;
        }
    }

    private void ChooseWalkPoint(Vector3 startingPoint, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(-range, range);
            if (x > environmentExtents.x)
            {
                x = environmentExtents.x;
            }
            else if (x < -environmentExtents.x)
            {
                x = -environmentExtents.x;
            }

            float z = Random.Range(-range, range);
            if (z > environmentExtents.z)
            {
                z = environmentExtents.z;
            }
            else if (z < -environmentExtents.z)
            {
                z = -environmentExtents.z;
            }

            Vector3 point = new Vector3(startingPoint.x + x, environmentCenter.y, startingPoint.z + z);
            Debug.Log(name + ": ChooseWalkPoint - " + point);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, yRange, navMeshAgent.areaMask))
            {
                walkPoint = hit.position; 
                if (navMeshAgent.SetDestination(walkPoint))
                {
                    isWalkPointSet = true;
                    Debug.Log(name + "'s walking point: " + walkPoint);
                    return;
                }
            }
        }
    }

    enum ActionStatus
    {
        undefined,
        walking,
        idle,
        chasing,
        attacking,
        searching
    }
}
