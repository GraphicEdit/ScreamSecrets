
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Patrol
    }

    protected NavMeshAgent navAgent;

    [SerializeField] protected string enemyName;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float rotSpeed;
    [SerializeField] protected float followDistance;
    [SerializeField] protected float stopDistance;
    [SerializeField] protected float gravity = -12;
    [SerializeField] protected int waitSecondsToKeepPatrilling = 0;
    [SerializeField] protected bool isPatroling = false;

    protected float velocityY;
    protected Transform target;
    protected Animator anim;

    [SerializeField]protected GameObject[] patrolPoints;

    protected bool playerIsDetected;

    protected EnemyState currentState;

    protected virtual void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerIsDetected = false;

        Introduction();

        patrolPoints = GameObject.FindGameObjectsWithTag("patrolPoint");
        ChangeState(EnemyState.Patrol);

    }

    protected void Update()
    {

        if (Input.GetKeyUp(KeyCode.T))
        {
            StopAllCoroutines();
            MoveTowardsTarget();
        }
        
        CheckForTargetDistance();

    }

    protected virtual void FixedUpdate()
    {
        UpdateState();
    }

    protected virtual void Introduction()
    {
        //Debug.Log("My Name is " + enemyName + ", HP: " + healthPoint + ", moveSpeed: " + moveSpeed);
    }

    protected virtual void UpdateState()
    {
        float objDistance = Vector3.Distance(transform.position, target.position);

        objDistance = (int)objDistance;

        switch (currentState)
        {
            case EnemyState.Idle:
                if (objDistance <= followDistance)
                {
                    ChangeState(EnemyState.Chasing);
                }
                break;

            case EnemyState.Chasing:
                if (objDistance > followDistance)
                {

                    ChangeState(EnemyState.Idle);
                }
                else if (objDistance <= stopDistance)
                {
                    ChangeState(EnemyState.Attacking);
                }
                else
                {
                    MoveTowardsTarget();
                }
                break;

            case EnemyState.Attacking:
                if (objDistance > stopDistance)
                {
                    ChangeState(EnemyState.Chasing);
                }
                else
                {
                    //PerformAttack();
                }
                break;

            case EnemyState.Patrol:

                Debug.Log("is patroling");

                break;
        }
    }

    protected void ChangeState(EnemyState newState)
    {
        currentState = newState;
        OnStateEnter(newState);
    }

    protected virtual void OnStateEnter(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                // Do something when entering idle state
                break;

            case EnemyState.Chasing:
                // For example, play a chasing animation or sound
                break;

            case EnemyState.Attacking:
                // For example, play an attacking animation or sound
                break;

            case EnemyState.Patrol:
                StopAllCoroutines();
                GetNextPatrolPoint();
                break;
        }
    }

    protected void PlayerDetected(float objDistance)
    {
        if (playerIsDetected)
        {

            

        }
    }

    protected void MoveTowardsTarget()
    {
        // Calculate the direction towards the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Ensure the y-component of the direction is zero to keep the enemy on the ground
        direction.y = 0;

        // Move the enemy towards the target
        navAgent.SetDestination(target.position);

        //RotateToTarget(target);
    }

    protected void CheckForTargetDistance()
    {

        Vector3 currentPosition = this.transform.position; // Current position of the object
        Vector3 targetPosition = navAgent.destination;     // Target position the NavMeshAgent is heading towards

        float distanceToTravel = Vector3.Distance(currentPosition, targetPosition);

        //Debug.Log(Mathf.Round(distanceToTravel));

        if (distanceToTravel > 1)
        {
            if (distanceToTravel < 2)
            {
                StartCoroutine(WaitBeforePatrol());
            }
        }
    }

    protected void GetNextPatrolPoint()
    {
        int randomPoint = Random.Range(0, patrolPoints.Length);

        navAgent.SetDestination(patrolPoints[randomPoint].transform.position);

        CheckForTargetDistance();
    }

    protected IEnumerator WaitBeforePatrol()
    {
        Debug.Log("is waiting");

        ChangeState(EnemyState.Idle);

        yield return new WaitForSeconds(waitSecondsToKeepPatrilling);

        ChangeState(EnemyState.Patrol);

    }

    // Method to check if the enemy is grounded
    protected virtual bool IsGrounded()
    {
        // Raycast down to check if the enemy is on the ground
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo);
    }

    protected virtual void RotateToTarget(Transform target)
    {
        // Calculate direction to look at the target
        Vector3 direction = target.transform.position - this.gameObject.transform.position;
        // Calculate the rotation required to look in that direction
        Quaternion lookRotation = Quaternion.LookRotation(direction * Time.deltaTime);
        // Assign the rotation to the game object
        this.gameObject.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, rotSpeed * Time.deltaTime);
    }

    // Method to perform the attack
    protected abstract void PerformAttack();
}