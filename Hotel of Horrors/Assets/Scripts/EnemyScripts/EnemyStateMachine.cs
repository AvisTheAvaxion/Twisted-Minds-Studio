using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AI))]
public class EnemyStateMachine : MonoBehaviour
{
    public enum FaceDirection
    {
        Left, Right
    }

    protected FaceDirection currentFaceDir = FaceDirection.Left;

    public enum States
    {
        Idle, Dialogue, Patrolling, Fighting, Death, Searching, Stun, Dash
    }

    [SerializeField] bool debug;
    [SerializeField] bool gizmos;

    [SerializeField] protected bool flipToRotate;
    [SerializeField] Transform transformToFlip;
    [SerializeField] protected Transform rayCastOrigin;

    protected AI navigation;

    protected Rigidbody2D rb;

    protected EnemySpawner mySpawner;

    protected EnemyHealth myHealth;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected bool hasWalkCycle = true;
    [SerializeField] protected ParticleSystem moveParticles;

    protected States currentState = States.Idle;
    public States CurrentState { get => currentState; }

    [Header("Settings")]
    [SerializeField] Attacks.AttackModes attackType = Attacks.AttackModes.Melee;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int emotionalEnergyWorth = 50;
    [SerializeField] protected float stunLength = 0.5f;
    protected float currentStunLength;

    [Header("Patrol Settings")]
    [SerializeField] bool movesOnPatrol = true;
    [SerializeField] protected float patrolRadius;
    [SerializeField] protected float patrolTime = 3f;
    [SerializeField] protected float pauseMin, pauseMax;
    [SerializeField] protected float aggroRadius = 9;

    [Header("Fight Settings")]
    [SerializeField] protected float contactDamage;
    [SerializeField] protected float contactKnockback;
    [SerializeField] protected float attackCooldownMin, attackCooldownMax;
    [SerializeField] protected float hitStopLength = 0.05f;
    [SerializeField] protected float maxDistanceToNavigate = 3f;
    [SerializeField] protected bool contWalkRightAfterAttack;

    [Header("Melee Fight Settings")]
    [SerializeField] protected Transform meleeAttackPoint;
    [SerializeField] protected float meleeRange = 0.5f;
    [SerializeField] protected float meleeRadius;
    [SerializeField] protected float meleeDamage;
    [SerializeField] protected float knockback;
    [SerializeField] [Range(0,1)] protected float chanceToInflictEffect;
    [SerializeField] protected EffectInfo[] effectsToInflict;

    [Header("Ranged Fight Settings")]
    [SerializeField] protected BasicShooter shooter;
    [SerializeField] protected float range = 2;
    [SerializeField] protected bool canMoveAndShoot = true;
    [SerializeField] protected LayerMask wallsLayerMask;

    [Header("Death Settings")]
    [SerializeField] protected float dropRadius = 0.5f;
    [SerializeField] protected float upwardsVelocity = 0.2f;
    [SerializeField] protected float startHeight = 0.2f;
    [SerializeField] protected int minDropAmount, maxDropAmount;
    [SerializeField] protected GameObject itemHolderPrefab;
    [SerializeField] protected ItemDrop[] itemDrops;
    [SerializeField] protected GameObject emotionalEnergyPrefab;
    [SerializeField] protected int deathLayer = 16;
    [SerializeField] protected EnemyVisuals enemyVisuals;
    public event EventHandler OnEnemyDeath;
    string enemyName;
    
    protected bool canMove;
    protected bool canAttack;

    protected bool patrolling;
    protected Vector2 patrolTargetPos;
    protected float patrolTimer;

    protected GameObject target;
    protected Vector3 lastTargetPos;

    protected float stunTimer;

    protected Transform currentRoom;

    protected Vector2 startPos;

    protected bool death;

    [Header("Audio Settings")]
    [SerializeField] protected EnemyAudioManager enemyAudioManager;

    public void ApplyRoomModifier(Room.RoomModifiers modifier)
    {
        if(myHealth == null) myHealth = GetComponent<EnemyHealth>();
        if(navigation == null) navigation = GetComponent<AI>();

        switch (modifier)
        {
            case Room.RoomModifiers.increasedHealth:
                Stat healthStat = myHealth.stats.GetStat(Stat.StatType.Health);
                healthStat.ChangeMaxValue(1.5f);
                break;
            case Room.RoomModifiers.increasedDamage:
                Stat damageStat = myHealth.stats.GetStat(Stat.StatType.MeleeDamage);
                if(damageStat != null) damageStat.ChangeMaxValue(1.5f);

                damageStat = myHealth.stats.GetStat(Stat.StatType.RangedDamage);
                if (damageStat != null) damageStat.ChangeMaxValue(1.5f);
                break;
            case Room.RoomModifiers.increasedAttackSpeed:
                Stat attackSpeedStat = myHealth.stats.GetStat(Stat.StatType.AttackSpeed);
                if (attackSpeedStat != null) attackSpeedStat.ChangeMaxValue(1.5f);
                break;
            case Room.RoomModifiers.increasedMovementSpeed:
                Stat movementSpeedStat = myHealth.stats.GetStat(Stat.StatType.MovementSpeed);
                if (movementSpeedStat != null) movementSpeedStat.ChangeMaxValue(1.5f);
                navigation.SetSpeed((moveSpeed + myHealth.stats.GetCurrentValue(Stat.StatType.MovementSpeed)) / 10f);
                break;
        }
    }


    private void Start()
    {
        enemyName = gameObject.name;
        target = GameObject.Find("Player");
        if(target == null) Debug.LogError("Player not detected");

        navigation = GetComponent<AI>();
        rb = GetComponent<Rigidbody2D>();
        myHealth = GetComponent<EnemyHealth>();

        navigation.SetTarget(target.transform);
        navigation.SetCanMove(true);
        navigation.SetSpeed((moveSpeed + myHealth.stats.GetCurrentValue(Stat.StatType.MovementSpeed)) / 10f);

        startPos = transform.position;

        canAttack = true;
        death = false;

        currentStunLength = stunLength;

        Initialize();

        StartCoroutine(WaitBeforeMoving(pauseMin, pauseMax));

        if (enemyVisuals)
        {
            enemyVisuals.StartAppear(0.6f);
        }
    }
    protected virtual void Initialize()
    {
    }
    public void SetSpawner(EnemySpawner spawner)
    {
        mySpawner = spawner;
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
            case States.Searching:
                Search();
                break;
            case States.Stun:
                Stun();
                break;
            case States.Dash:
                Dash();
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
        if (!death)
        {
            death = true;

            //rb.velocity = Vector2.zero;

            FindObjectOfType<Floor>().KillEnemy();

            OnEnemyDeath?.Invoke(enemyName, EventArgs.Empty);
            enemyAudioManager.Die();

            animator.enabled = false;

            gameObject.layer = deathLayer;

            if (enemyVisuals)
            {
                enemyVisuals.StartDissolve(0.3f);
            }
            else
            {
                OnSpawnItemDrops();
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnSpawnItemDrops()
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
                rand -= itemDrops[e].weight;
            }
            if(e < itemDrops.Length)
            {
                GameObject go = Instantiate(itemHolderPrefab, transform.position, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.forward));
                ItemData data = go.GetComponent<ItemData>();
                if (data) data.SetItemData(itemDrops[e].item, Random.Range(itemDrops[e].minCount, itemDrops[e].maxCount + 1));
                CustomRigidbody2D rb = go.GetComponent<CustomRigidbody2D>();
                if (rb) 
                {
                    rb.Initialize(startHeight, Random.Range(0f, upwardsVelocity));
                    rb.AddForce(GetRandomDir() * dropRadius * 2, ForceMode2D.Impulse); 
                }
            }
        }

        int currentEE = emotionalEnergyWorth;
        while(currentEE > 0)
        {
            int ee = Mathf.Clamp(Random.Range(EmotionalEnergy.minEmotionalEnergyPickup, EmotionalEnergy.maxEmotionalEnergyPickup + 1), 0, currentEE);

            GameObject go = Instantiate(emotionalEnergyPrefab, transform.position, emotionalEnergyPrefab.transform.rotation);
            EmotionalEnergy data = go.GetComponent<EmotionalEnergy>();
            if (data) data.SetEmotionalEnergy(ee);
            CustomRigidbody2D rb = go.GetComponent<CustomRigidbody2D>();
            if (rb)
            {
                rb.Initialize(startHeight, Random.Range(0f, upwardsVelocity));
                rb.AddForce(GetRandomDir() * dropRadius * 3, ForceMode2D.Impulse);
            }

            currentEE -= ee;
        }
    }

    protected RaycastHit2D RaycastPlayer(Vector2 dir, float distance)
    {
        return Physics2D.Raycast(rayCastOrigin.position, dir, distance, wallsLayerMask);
    }

    protected virtual void Patrol()
    {
        if(canMove && movesOnPatrol)
        {
            if (!patrolling)
            {
                patrolling = true;
                Vector2 randomPos = startPos;
                if (mySpawner) randomPos = mySpawner.GetRandomPosition(startPos, patrolRadius);
                else randomPos = startPos + GetRandomDir() * patrolRadius;
                patrolTargetPos = randomPos;

                patrolTimer = 0;
            }

            bool arrived = !navigation.MoveTowardsTarget(patrolTargetPos);

            Vector2 dir = (patrolTargetPos - (Vector2)transform.position).normalized;
            CheckToRotate(dir);

            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

            if(arrived || patrolTimer >= patrolTime)
            {
                patrolling = false;

                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);

                StartCoroutine(WaitBeforeMoving(pauseMin, pauseMax));
            }

            patrolTimer += Time.deltaTime;
        }

        float distToPlayer = GetDistanceToPlayer();
        Vector2 dirToPlayer = GetDirToPlayer();
        if (distToPlayer < aggroRadius && RaycastPlayer(dirToPlayer, distToPlayer).collider == null)
        {
            canMove = true;
            canAttack = true;

            currentState = States.Fighting;

            navigation.SetCanMove(true);

            if (debug) print("Switch to Fighting State");
        }
    }
    protected virtual void Dash()
    {
    }

    protected virtual void Fight()
    {
        if (canMove)
        {
            float distToTarget = GetDistanceToPlayer();
            Vector2 dirToPlayer = GetDirToPlayer();

            lastTargetPos = target.transform.position;

            if (distToTarget > maxDistanceToNavigate || RaycastPlayer(dirToPlayer, distToTarget).collider != null)
            {
                currentState = States.Searching;
            }
            else
            {
                if (attackType == Attacks.AttackModes.Melee)
                {
                    if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

                    CheckToRotate(dirToPlayer);

                    if (canAttack)
                    {
                        navigation.MoveTowardsTarget();
                        if (distToTarget < meleeRange)
                        {
                            MeleeAttackStart();
                            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                        }
                    }
                    else
                    {
                        navigation.OrbitAroundTarget();
                    }
                }
                else if (attackType == Attacks.AttackModes.Ranged)
                {
                    if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

                    CheckToRotate(dirToPlayer);

                    navigation.OrbitAroundTarget();

                    if (distToTarget < range && canAttack)
                    {
                        RangedAttackStart();

                        if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                    }
                }
            }
        }
    }

    protected virtual void Search()
    {
        if(canMove)
        {
            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

            bool arrived = !navigation.MoveTowardsTarget(lastTargetPos);

            float distToTarget = GetDistanceToPlayer();
            Vector2 dirToPlayer = GetDirToPlayer();

            if(!(distToTarget > maxDistanceToNavigate || RaycastPlayer(dirToPlayer, distToTarget).collider != null))
            {
                currentState = States.Fighting;
            }
            else if (arrived)
            {
                startPos = transform.position;
                currentState = States.Patrolling;
            }
        }
    }

    protected virtual void Stun()
    {
        stunTimer += Time.deltaTime;

        if(stunTimer > currentStunLength)
        {
            currentStunLength = stunLength;
            currentState = States.Fighting;

            if (contWalkRightAfterAttack)
            {
                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);
            }
        }
    }

    #region Melee Attack
    public virtual void MeleeAttackStart()
    {
        if (animator != null) animator.SetTrigger("Attack");
        canMove = false;
        canAttack = false;
    }
    public virtual void MeleeAttack()
    {
        //Do melee attack
        Collider2D[] colliders = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeRadius);

        enemyAudioManager.Attack(gameObject);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag.Equals("Player"))
            {
                IHealth health = colliders[i].gameObject.GetComponent<IHealth>();
                Vector2 dir = (colliders[i].transform.position - transform.position).normalized;

                health.TakeDamage(meleeDamage + myHealth.stats.GetCurrentValue(Stat.StatType.MeleeDamage));
                health.Knockback(dir, knockback);
                if (effectsToInflict != null)
                {
                    foreach (EffectInfo effectInfo in effectsToInflict)
                    {
                        health.InflictEffect(new Effect(effectInfo, chanceToInflictEffect));
                    }
                }
                break;
            }
        }
    }
    public virtual void MeleeAttackEnd()
    {
        canMove = true;

        if (debug) print("End Melee Attack");

        if(contWalkRightAfterAttack)
        {
            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);
        }

        StartCoroutine(WaitBeforeAttacking(attackCooldownMin - myHealth.stats.GetCurrentValue(Stat.StatType.AttackSpeed), 
            attackCooldownMax - myHealth.stats.GetCurrentValue(Stat.StatType.AttackSpeed)));
    }
    #endregion

    #region Ranged Attack
    public virtual void RangedAttackStart()
    {
        if (!canMoveAndShoot) 
        { 
            canMove = false;
        }

        if (animator != null) animator.SetTrigger("Attack");
        canAttack = false;
    }
    public virtual void RangedAttack()
    {
        //Do ranged attack
        if(shooter != null)
        {
            shooter.Attack();

            enemyAudioManager.Attack(gameObject);
        }
    }
    public virtual void RangedAttackEnd()
    {
        canMove = true;

        if (debug) print("End Ranged Attack");

        if (contWalkRightAfterAttack)
        {
            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);
        }

        StartCoroutine(WaitBeforeAttacking(attackCooldownMin - myHealth.stats.GetCurrentValue(Stat.StatType.AttackSpeed), 
            attackCooldownMax - myHealth.stats.GetCurrentValue(Stat.StatType.AttackSpeed)));
    }
    #endregion

    public virtual void Knockback(Vector2 dir, float strength)
    {
        if(currentState != States.Death) 
        { 
            stunTimer = 0;
            currentState = States.Stun;
            if (animator != null) animator.SetTrigger("Hit");
            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
        }

        navigation.PauseMovement(0.5f);
        rb.AddForce(dir * strength, ForceMode2D.Impulse);
    }

    //Used with Patrol
    protected IEnumerator WaitBeforeMoving(float minWait, float maxWait)
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        canMove = true;
    }

    //Attack cooldown
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

    //Animation Event
    public void EmitMoveParticles()
    {
        if (currentState == States.Death) return;

        if (moveParticles) moveParticles.Stop();
        if (moveParticles) moveParticles.Play();
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
        if (currentState == States.Fighting && collision.gameObject.tag.Equals("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;

            if (contactKnockback > 0 && contactDamage > 0)
            {
                health.TakeDamage(contactDamage);
                playerMovement.Knockback(dir, contactKnockback * 2);
            }
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

            Gizmos.color = Color.blue;

            if (target != null)
            {
                float distToPlayer = GetDistanceToPlayer();
                Vector2 dirToPlayer = GetDirToPlayer();
                Gizmos.DrawLine(rayCastOrigin.position, rayCastOrigin.position + (Vector3)(dirToPlayer * distToPlayer));
            }

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeRadius);
        }
    }
}

[System.Serializable]
public struct ItemDrop
{
    public UseableInfo item;
    public int minCount;
    public int maxCount;
    public float weight;
}
