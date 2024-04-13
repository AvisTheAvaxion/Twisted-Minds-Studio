using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyStateMachine : MonoBehaviour
{
    public enum FaceDirection
    {
        Left, Right
    }

    protected FaceDirection currentFaceDir = FaceDirection.Left;

    public enum States
    {
        Idle, Dialogue, Patrolling, Fighting, Death
    }

    [SerializeField] bool debug;
    [SerializeField] bool gizmos;

    [SerializeField] protected bool flipToRotate;
    [SerializeField] Transform transformToFlip;

    [Header("References")]
    [SerializeField] protected AILerp navigation;
    [SerializeField] protected AIDestinationSetter destSetter;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected bool hasWalkCycle = true;

    protected States currentState = States.Idle;

    [Header("Settings")]
    [SerializeField] Attacks.AttackModes attackType = Attacks.AttackModes.Melee;
    [SerializeField] protected float moveSpeed;

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolRadius;
    [SerializeField] protected float pauseMin, pauseMax;
    [SerializeField] protected float aggroRadius = 9;

    [Header("Fight Settings")]
    [SerializeField] protected float contactDamage;
    [SerializeField] protected float contactKnockback;
    [SerializeField] protected float attackCooldownMin;
    [SerializeField] protected float attackCooldownMax;
    [SerializeField] protected float minDistToTarget;
    [SerializeField] protected float maxDistToTarget;

    [Header("Melee Fight Settings")]
    [SerializeField] protected Transform meleeAttackPoint;
    [SerializeField] protected float meleeRadius;
    [SerializeField] protected float meleeDamage;
    [SerializeField] protected float knockback;
    [SerializeField] [Range(0,1)] protected float chanceToInflictEffect;
    [SerializeField] protected EffectInfo[] effectsToInflict;

    [Header("Ranged Fight Settings")]
    [SerializeField] protected BasicShooter shooter;
    [SerializeField] protected bool maintainDistToTarget = true;
    [SerializeField] protected bool canMoveAndShoot = true;
    [SerializeField] LayerMask wallsLayerMask;

    [Header("Death Settings")]
    [SerializeField] protected float dropRadius = 0.5f;
    [SerializeField] protected int minDropAmount;
    [SerializeField] protected int maxDropAmount;
    [SerializeField] protected GameObject itemHolder;
    [SerializeField] protected ItemDrop[] itemDrops;
    
    protected bool canMove;
    protected bool isAttacking;
    protected bool canAttack;

    protected GameObject target;

    protected Transform currentRoom;

    protected Vector2 startPos;


    private void Start()
    {
        target = GameObject.Find("Player");
        if(target == null) Debug.LogError("Player not detected");

        startPos = transform.position;

        navigation.canSearch = false;
        navigation.canMove = false;

        navigation.speed = moveSpeed / 10f;

        isAttacking = false;

        StartCoroutine(WaitBeforeMoving(pauseMin, pauseMax));
    }

    private void Update()
    {
        ChooseState();
    }

    protected void ChooseState()
    {
        switch (currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Dialogue:
                Idle();
                break;
            case States.Patrolling:
                Patrol();
                break;
            case States.Fighting:
                Fight();
                break;
            case States.Death:
                Death();
                break;
        }
    }

    protected virtual void Idle()
    {
        //starts patrolling when the player is in the room
        if(canMove)
        {
            currentState = States.Patrolling;
        }
    }

    protected virtual void OnDeath()
    {
        currentState = States.Death;
    }
    protected virtual void Death() 
    {
        SpawnItemDrops();
        Destroy(gameObject);
    }
    protected virtual void SpawnItemDrops()
    {
        if (itemDrops == null || itemDrops.Length == 0) return;

        int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);
        float weightTotal = 0;
        for (int i = 0; i < itemDrops.Length; i++)
        {
            weightTotal += itemDrops[i].weight;
        }

        for (int i = 0; i < dropAmount; i++)
        {
            float rand = Random.Range(0, weightTotal);
            int e = 0;
            for (; e < itemDrops.Length; e++)
            {
                if (rand < itemDrops[e].weight)
                    break;
                weightTotal -= itemDrops[e].weight;
            }
            if(e < itemDrops.Length)
            {
                GameObject go = Instantiate(itemHolder, transform.position, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.forward));
                ItemData data = go.GetComponent<ItemData>();
                if (data) data.SetItemData(itemDrops[e].item);
                Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
                if(rb) rb.AddForce(GetRandomDir() * dropRadius, ForceMode2D.Impulse);
            }
        }
    }

    protected virtual void Patrol()
    {
        if(canMove)
        {
            if(navigation.canMove == false)
            {
                Vector2 randomPos = startPos + GetRandomDir() * patrolRadius;
                navigation.destination = randomPos;

                navigation.canMove = true;
                navigation.canSearch = true;

                Vector2 dir = (randomPos - (Vector2)transform.position).normalized;
                CheckToRotate(dir);

                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

                navigation.SearchPath();
                return;
            }

            if(navigation.reachedDestination || navigation.reachedEndOfPath)
            {
                navigation.canMove = false;
                navigation.canSearch = false;

                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);

                StartCoroutine(WaitBeforeMoving(pauseMin, pauseMax));
            }
        }

        if (GetDistanceToPlayer() < aggroRadius)
        {
            canMove = true;
            navigation.canMove = true;
            navigation.canSearch = true;

            canAttack = true;

            currentState = States.Fighting;

            if (debug) print("Switch to Fighting State");
        }
    }

    protected virtual void Fight()
    {
        if (canMove)
        {
            float distToTarget = GetDistanceToPlayer();
            Vector2 dirToPlayer = GetDirToPlayer();

            if (attackType == Attacks.AttackModes.Melee)
            {
                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);
                if(navigation.reachedEndOfPath) animator.SetBool("isWalking", false);

                navigation.destination = (Vector2)target.transform.position - dirToPlayer * maxDistToTarget;
                CheckToRotate(dirToPlayer);

                if (canAttack && distToTarget < minDistToTarget)
                {
                    MeleeAttackStart();

                    if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                }
            }
            else if (attackType == Attacks.AttackModes.Ranged)
            {
                if((distToTarget > minDistToTarget || !maintainDistToTarget) && distToTarget < maxDistToTarget)
                {
                    CheckToRotate(dirToPlayer);
                    if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                } 
                else
                {
                    if(!navigation.reachedEndOfPath)
                    {
                        CheckToRotate(dirToPlayer);
                        if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);
                    } 
                    else
                    {
                        CheckToRotate(dirToPlayer);
                        if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                    }

                    //Simple navigation
                    //navigation.destination = (Vector2)target.transform.position - dirToPlayer * (minDistToTarget + maxDistToTarget) / 2;

                    //Advanced navigation
                    RaycastHit2D hit = Physics2D.Raycast(target.transform.position, -dirToPlayer, float.MaxValue, wallsLayerMask);
                    if (hit.collider != null)
                    {
                        navigation.destination = (Vector2)target.transform.position - dirToPlayer * Mathf.Min(hit.distance, (minDistToTarget + maxDistToTarget) / 2);
                    } else
                    {
                        navigation.destination = (Vector2)target.transform.position - dirToPlayer * (minDistToTarget + maxDistToTarget) / 2;
                    }
                }

                if(distToTarget < maxDistToTarget && canAttack)
                {
                    RangedAttackStart();

                    if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                }
            }
        }
    }

    public virtual void MeleeAttackStart()
    {
        navigation.canMove = false;
        canMove = false;
        //isAttacking = true;
        canAttack = false;

        if (animator != null) animator.SetTrigger("Attack");
    }
    public virtual void MeleeAttack()
    {
        //Do melee attack
        Collider2D[] colliders = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag.Equals("Player"))
            {
                PlayerMovement playerMovement = colliders[i].gameObject.GetComponent<PlayerMovement>();
                IHealth health = colliders[i].gameObject.GetComponent<IHealth>();
                Vector2 dir = (colliders[i].transform.position - transform.position).normalized;


                health.TakeDamage(meleeDamage);
                if (effectsToInflict != null)
                {
                    foreach (EffectInfo effectInfo in effectsToInflict)
                    {
                        health.InflictEffect(new Effect(effectInfo, chanceToInflictEffect));
                    }
                }
                playerMovement.Knockback(dir, knockback);

                break;
            }
        }
    }
    public virtual void MeleeAttackEnd()
    {
        navigation.canMove = true;
        canMove = true;
        //isAttacking = false;

        if (debug) print("End Melee Attack");

        StartCoroutine(WaitBeforeAttacking(attackCooldownMin, attackCooldownMax));
    }

    public virtual void RangedAttackStart()
    {
        if (!canMoveAndShoot) 
        { 
            canMove = false;
            navigation.canMove = false;
        }

        canAttack = false;
        //isAttacking = true;

        if (animator != null) animator.SetTrigger("Attack");
    }
    public virtual void RangedAttack()
    {
        //Do ranged attack
        if(shooter != null)
        {
            shooter.Attack();
        }
    }
    public virtual void RangedAttackEnd()
    {
        canMove = true;
        navigation.canMove = true;
        //isAttacking = false;

        if (debug) print("End Ranged Attack");

        StartCoroutine(WaitBeforeAttacking(attackCooldownMin, attackCooldownMax));
    }

    protected IEnumerator WaitBeforeMoving(float minWait, float maxWait)
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        canMove = true;
    }

    protected IEnumerator WaitBeforeAttacking(float minWait, float maxWait)
    {
        canAttack = false;
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        canAttack = true;
    }

    /// <summary>
    /// Flips the sprite if necessary
    /// </summary>
    /// <param name="dir"></param>
    protected void CheckToRotate(Vector2 dir)
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

    /// <summary>
    /// Flips the sprite and its after image effect if necessary
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="afterImage"></param>
    protected void CheckToRotate(Vector2 dir, AfterImage afterImage)
    {
        if (flipToRotate)
        {
            float angle = Vector2.SignedAngle(dir, Vector2.up);
            if (angle <= 0 && currentFaceDir == FaceDirection.Right)
            {
                Vector2 newScale = transformToFlip.localScale;
                newScale.x *= -1;
                transformToFlip.localScale = newScale;

                if (afterImage != null)
                {
                    ParticleSystemRenderer psr = afterImage.GetComponent<ParticleSystemRenderer>();
                    if (psr != null) psr.flip = new Vector3(0, 0, 0);
                }

                currentFaceDir = FaceDirection.Left;
            }
            else if (angle > 0 && currentFaceDir == FaceDirection.Left)
            {
                Vector2 newScale = transformToFlip.localScale;
                newScale.x *= -1;
                transformToFlip.localScale = newScale;

                if (afterImage != null)
                {
                    ParticleSystemRenderer psr = afterImage.GetComponent<ParticleSystemRenderer>();
                    if (psr != null) psr.flip = new Vector3(1, 0, 0);
                }

                currentFaceDir = FaceDirection.Right;
            }
        }
    }

    #region Helper Functions
    protected float GetDistanceToPlayer()
    {
        return Vector3.Distance(this.transform.position, target.transform.position);
    }
    protected Vector2 GetDirToPlayer()
    {
        return (target.transform.position - transform.position).normalized;
    }
    protected Vector2 GetPosBetweenPlayer()
    {
        return Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime); // calculate distance to move).normalized;
    }
    protected Vector2 GetRandomDir()
    {
        return Random.insideUnitCircle; //Get a random position around a unit circle.
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Do damage and knockback to player for touching the monster
        if (collision.gameObject.tag.Equals("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;

            health.TakeDamage(contactDamage);
            playerMovement.Knockback(dir, contactKnockback*2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(gizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, patrolRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
        }
    }
}

[System.Serializable]
public struct ItemDrop
{
    public Useables item;
    public float weight;
}
