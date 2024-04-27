using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

[RequireComponent(typeof(AILerp), typeof(AI))]
public class LittleStein : MonoBehaviour
{
    public enum FaceDirection
    {
        Left, Right
    }

    protected FaceDirection currentFaceDir = FaceDirection.Left;

    public enum States
    {
        Follow, Fighting, Death
    }

    protected States currentState = States.Follow;

    [SerializeField] protected bool flipToRotate;
    [SerializeField] Transform transformToFlip;

    protected AILerp navigation;
    protected AI combatAI;

    protected Rigidbody2D rb;

    Transform player;

    [Header("References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected BasicShooter shooter;
    [SerializeField] protected Transform pivot;

    [Header("Settings")]
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float agroRadius = 2f;
    [SerializeField] protected float timeBtwChecks = 1f;
    [SerializeField] protected float followRadius = 0.5f;
    [SerializeField] protected float maxDistanceFromPlayer = 4f;

    Collider2D[] possibleTargets;
    Transform currentTarget;

    float timer = 0;

    bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        combatAI = GetComponent<AI>();
        navigation = GetComponent<AILerp>();
        rb = GetComponent<Rigidbody2D>();

        combatAI.SetCanMove(false);
        combatAI.SetSpeed(moveSpeed / 10f);

        navigation.canSearch = false;
        navigation.canMove = false;

        navigation.speed = moveSpeed / 10f;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public void SetCurrentState(States state)
    {
        currentState = state;
    }

    private void Update()
    {
        ChooseState();
    }

    protected void ChooseState()
    {
        switch (currentState)
        {
            case States.Follow:
                Follow();
                break;
            case States.Fighting:
                Fight();
                break;
            case States.Death:
                Death();
                break;
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    private void Fight()
    {
        Vector2 vectorToPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distToPlayer = vectorToPlayer.magnitude;
        Vector2 dirToPlayer = vectorToPlayer.normalized;

        if (distToPlayer > maxDistanceFromPlayer) currentState = States.Follow;

        if (timer > timeBtwChecks || currentTarget == null)
        {
            CheckForTargets();
            timer = 0;

            if (possibleTargets.Length == 0) currentState = States.Follow;
            else FindClosestTarget();
        }

        if (currentTarget != null)
        {
            Vector2 vectorToTarget = (Vector2)currentTarget.position - (Vector2)transform.position;
            float distToTarget = vectorToTarget.magnitude;
            Vector2 dirToTarget = vectorToTarget.normalized;

            combatAI.SetCanMove(true);
            combatAI.SetTarget(currentTarget);
            combatAI.OrbitAroundTarget();

            if (canAttack && distToTarget <= attackRange)
            {
                pivot.rotation = Quaternion.FromToRotation(pivot.up, dirToTarget) * pivot.rotation;
                shooter.SetTarget(currentTarget);
                shooter.Attack();
            }
        }

        timer += Time.deltaTime;
    }

    private void Follow()
    {
        Vector2 vectorToPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distToPlayer = vectorToPlayer.magnitude;
        Vector2 dirToPlayer = vectorToPlayer.normalized;

        if(distToPlayer > maxDistanceFromPlayer)
        {
            navigation.destination = player.position;

            if (!navigation.canSearch)
            {
                combatAI.SetCanMove(false);
                navigation.canSearch = true;
                navigation.canMove = true;

                navigation.SearchPath();
            }
        }
        else
        {
            if (timer > timeBtwChecks)
            {
                CheckForTargets();
                timer = 0;

                if (possibleTargets.Length > 0) 
                {
                    FindClosestTarget();
                    currentState = States.Fighting; 
                }
            }

            combatAI.SetCanMove(true);
            navigation.canSearch = false;
            navigation.canMove = false;
            combatAI.MoveTowardsTarget((Vector2)player.position - dirToPlayer * followRadius);
        }

        timer += Time.deltaTime;
    }

    void CheckForTargets()
    {
        possibleTargets = Physics2D.OverlapCircleAll(transform.position, agroRadius, enemyMask);
    }

    void FindClosestTarget()
    {
        float closestDistanceSqr = 999f * 999f;
        if (currentTarget != null) closestDistanceSqr = (transform.position - currentTarget.transform.position).sqrMagnitude;
        for (int i = 0; i < possibleTargets.Length; i++)
        {
            float distSqr = (transform.position - possibleTargets[i].transform.position).sqrMagnitude;
            if(distSqr < closestDistanceSqr)
            {
                currentTarget = possibleTargets[i].transform;
                closestDistanceSqr = distSqr;
            }
        }
    }

    IEnumerator AttackCooldown(float cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
