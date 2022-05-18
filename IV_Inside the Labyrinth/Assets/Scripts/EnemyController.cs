using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    Transform player;
    public LayerMask playerLMask, hidingObstacleLMask;
    NavMeshAgent navMeshAgent;
    ActionStatus actionStatus = ActionStatus.walking;

    // Walking
    Vector3 walkPoint;
    bool isWalkPointSet = false;
    public float walkPointRange, pathRange;
    public float walkingSpeed;

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

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
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

        // TODO: to add interaction with other enemies

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
            if (isPlayerInAttackRange)
            {
                if (actionStatus != ActionStatus.attacking)
                {
                    // Starting attacking
                    actionStatus = ActionStatus.attacking;
                    isWalkPointSet = false;
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
                    Debug.Log(name + "'s action status: " + actionStatus);
                    return;
                }
            } 
        }
        else
        {
            if (actionStatus == ActionStatus.attacking || actionStatus == ActionStatus.chasing)
            {
                // And of attacking or chasing
                actionStatus = ActionStatus.searching;
                isWalkPointSet = false;
                navMeshAgent.ResetPath();
                searchTimeLeft = searchTime;
                Debug.Log(name + "'s Action status: " + actionStatus.ToString());
            }
        }

        if (actionStatus == ActionStatus.undefined)
        {
            isWalkPointSet = false;
            isIdleTimeSet = false;
            navMeshAgent.ResetPath();
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
            if (!navMeshAgent.hasPath || !(navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete))
            {
                actionStatus = ActionStatus.undefined;
            }
        }
        else
        {
            ChooseWalkPoint();
        }

        // TODO: to fix detecting if enemy stacked
    }

    private void Chasing()
    {
        navMeshAgent.SetDestination(player.position);
        // TODO: To complete Chasing method
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
        }
        else
        {
            Debug.Log(name + "'ve lost the player.");
            actionStatus = ActionStatus.undefined;
        }
        // TODO: To complete Seaching method
    }

    private void ChooseWalkPoint()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 pointV2 = Random.insideUnitCircle * walkPointRange;
            Vector3 point = new Vector3(transform.position.x + pointV2.x, transform.position.y,
                transform.position.z + pointV2.y);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas))
            {
                walkPoint = hit.position; 
                if (navMeshAgent.SetDestination(walkPoint))
                {
                    isWalkPointSet = true;
                    Debug.Log(name + "'s walking point: " + walkPoint);
                }
                return;
            }
        }
        // TODO: To check if the path is too long
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
