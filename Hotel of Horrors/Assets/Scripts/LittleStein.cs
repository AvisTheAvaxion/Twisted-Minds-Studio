using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AI))]
public class LittleStein : MonoBehaviour
{
    public enum FaceDirection
    {
        Left, Right
    }

    FaceDirection currentFaceDir = FaceDirection.Left;

    public enum States
    {
        Follow, Fighting, Death, Teleporting, Spawning
    }

    States currentState = States.Follow;

    [SerializeField] bool flipToRotate;
    [SerializeField] Transform transformToFlip;

    AI combatAI;

    Rigidbody2D rb;

    Transform player;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] BasicShooter shooter;
    [SerializeField] Transform pivot;
    [SerializeField] ParticleSystem walkParticles;
    [SerializeField] ParticleSystem vfxParticles;
    [SerializeField] SpriteRenderer[] spriteRenderers;

    [Header("Settings")]
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float agroRadius = 2f;
    [SerializeField] float timeBtwChecks = 1f;
    [SerializeField] float followRadius = 0.5f;
    [SerializeField] float maxDistanceFromPlayer = 4f;
    [SerializeField] float dissolveRate = 0.5f;
    [SerializeField] int frameRate = 16;

    Collider2D[] possibleTargets;
    Transform currentTarget;

    Vector2 dirToTarget;

    float timer = 0;

    bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        combatAI = GetComponent<AI>();
        //navigation = GetComponent<AILerp>();
        rb = GetComponent<Rigidbody2D>();

        combatAI.SetCanMove(false);
        combatAI.SetSpeed(moveSpeed / 10f);

        //navigation.canSearch = false;
        //navigation.canMove = false;

        //navigation.speed = moveSpeed / 10f;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentState = States.Spawning;
        StartCoroutine(Spawn());
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public void SetCurrentState(States state)
    {
        currentState = state;

        if(currentState == States.Death)
        {
            StartCoroutine(Die());
        }
    }

    private void Update()
    {
        ChooseState();
    }

    void ChooseState()
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
        //Destroy(gameObject);
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
            if (animator) animator.SetBool("isWalking", true);

            Vector2 vectorToTarget = (Vector2)currentTarget.position - (Vector2)transform.position;
            float distToTarget = vectorToTarget.magnitude;
            dirToTarget = vectorToTarget.normalized;

            combatAI.SetCanMove(true);
            combatAI.SetTarget(currentTarget);
            combatAI.OrbitAroundTarget();

            if (canAttack && distToTarget <= attackRange)
            {
                if (animator) animator.SetTrigger("Attack");
                StartCoroutine(AttackCooldown(attackCooldown));
            }

            CheckToRotate(dirToTarget);
        }

        timer += Time.deltaTime;
    }

    public void Attack()
    {
        pivot.rotation = Quaternion.FromToRotation(pivot.up, dirToTarget) * pivot.rotation;
        shooter.SetTarget(currentTarget);
        shooter.Attack();
    }

    public void EmitWalkParticles()
    {
        if (walkParticles) walkParticles.Stop();
        if (walkParticles) walkParticles.Play();
    }

    private void Follow()
    {
        Vector2 vectorToPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distToPlayer = vectorToPlayer.magnitude;
        Vector2 dirToPlayer = vectorToPlayer.normalized;

        if(distToPlayer > maxDistanceFromPlayer)
        {
            //navigation.destination = player.position;

            /*if (!navigation.canSearch)
            {
                if (animator) animator.SetBool("isWalking", true);

                combatAI.SetCanMove(false);
                navigation.canSearch = true;
                navigation.canMove = true;

                navigation.SearchPath();
            }*/
            currentState = States.Teleporting;
            StartCoroutine(TeleportToPlayer());
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
            //navigation.canSearch = false;
            //navigation.canMove = false;
            bool moving = combatAI.MoveTowardsTarget((Vector2)player.position - dirToPlayer * followRadius);
            if (moving)
            {
                if (animator) animator.SetBool("isWalking", true);
            }
            else
            {
                if (animator) animator.SetBool("isWalking", false);
            }
        }

        CheckToRotate(dirToPlayer);

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

    void CheckToRotate(Vector2 dir)
    {
        if (flipToRotate)
        {
            float angle = Vector2.SignedAngle(dir, Vector2.up);
            if (angle <= 0 && currentFaceDir == FaceDirection.Right)
            {
                Vector2 newScale = transformToFlip.localScale;
                newScale.x *= -1;
                transformToFlip.localScale = newScale;

                currentFaceDir = FaceDirection.Left;
            }
            else if (angle > 0 && currentFaceDir == FaceDirection.Left)
            {
                Vector2 newScale = transformToFlip.localScale;
                newScale.x *= -1;
                transformToFlip.localScale = newScale;

                currentFaceDir = FaceDirection.Right;
            }
        }
    }

    IEnumerator AttackCooldown(float cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    IEnumerator Spawn()
    {
        float waitTime = 1f / frameRate;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        if (vfxParticles) vfxParticles.Play();
        float t = 0;
        while(t < dissolveRate)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color color = spriteRenderers[i].color;
                color.a = Mathf.Lerp(0.6f, 1f, (t / dissolveRate));
                spriteRenderers[i].color = color;
            }
            yield return wait;
            t += waitTime;
        }
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color color = spriteRenderers[i].color;
            color.a = 1f;
            spriteRenderers[i].color = color;
        }
        if (vfxParticles) vfxParticles.Stop();
        currentState = States.Follow;
    }
    IEnumerator TeleportToPlayer()
    {
        float waitTime = 1f / frameRate;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        float t = 0;
        if (vfxParticles) vfxParticles.Play();
        while (t < dissolveRate)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color color = spriteRenderers[i].color;
                color.a = Mathf.Lerp(1f, 0.6f, (t / dissolveRate) * (t / dissolveRate));
                spriteRenderers[i].color = color;
            }
            yield return wait;
            t += waitTime;
        }
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color color = spriteRenderers[i].color;
            color.a = 0f;
            spriteRenderers[i].color = color;
        }
        transform.position = player.position;
        yield return wait;

        t = 0;
        while (t < dissolveRate)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color color = spriteRenderers[i].color;
                color.a = Mathf.Lerp(0.6f, 1f, (t / dissolveRate) * (t / dissolveRate));
                spriteRenderers[i].color = color;
            }
            yield return wait;
            t += waitTime;
        }
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color color = spriteRenderers[i].color;
            color.a = 1f;
            spriteRenderers[i].color = color;
        }
        if (vfxParticles) vfxParticles.Stop();
        currentState = States.Follow;
    }
    IEnumerator Die()
    {
        rb.velocity = Vector2.zero;
        if (animator) animator.SetBool("isWalking", false);

        float waitTime = 1f / frameRate;
        GetComponent<Collider2D>().enabled = false;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        if(vfxParticles) vfxParticles.Play();
        float t = 0;
        while (t < dissolveRate)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color color = spriteRenderers[i].color;
                color.a = Mathf.Lerp(1f, 0.6f, (t / dissolveRate) * (t / dissolveRate));
                spriteRenderers[i].color = color;
            }
            yield return wait;
            t += waitTime;
        }
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color color = spriteRenderers[i].color;
            color.a = 0f;
            spriteRenderers[i].color = color;
        }
        if (vfxParticles) vfxParticles.Stop();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
