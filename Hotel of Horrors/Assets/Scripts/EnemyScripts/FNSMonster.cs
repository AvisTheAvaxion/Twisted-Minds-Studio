using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNSMonster : BossStateMachine
{
    [System.Serializable]
    public struct AttackSettings
    {
        public float walkSpeed;
        public float normalKnockback;
        public float normalContactDamage;

        [Header("Charge Settings")]
        public float chargeSpeed;
        public float chargeLength;
        public float chargeDamage;
        public float chargeKnockback;
        public float chargeStun;
        [Range(0,1)] public float chargeSensitivity;
        public float chargeMaxAttackRange;
        public float chargeCooldownMin;
        public float chargeCooldownMax;

        [Header("Slam Settings")]
        public float slamMinAttackRange;
        public float slamDamage;
        public float slamRadius;
        public float slamKnockback;
        public float slamCooldownMin;
        public float slamCooldownMax;
    }

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] AfterImage afterImage;
    [SerializeField] BasicShooter shooter;

    [Header("Attack Settings")]
    [SerializeField] AttackSettings normalAttackSettings;
    [SerializeField] AttackSettings enragedAttackSettings;

    AttackSettings currentSettings;

    Vector2 moveDir;
    bool isCharging;

    protected override void Init()
    {
        Disenrage();

        currentStage = stages[0];
    }


    protected override void Death()
    {
        throw new System.NotImplementedException();
    }

    protected override void Dialogue()
    {
        throw new System.NotImplementedException();
    }

    protected override void Fight()
    {
        if(!isAttacking)
            Move();

        switch(currentStage.stage)
        {
            case 1:
                Fight_Stage1();
                break;
            case 2:
                Fight_Stage2();
                break;
        }
    }

    void Fight_Stage1()
    {
        if (isAttacking || onCooldown)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if(distToPlayer <= currentSettings.slamMinAttackRange)
        {
            SlamBegin();
        }
    }

    void Fight_Stage2()
    {
        if (isAttacking || onCooldown)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if(distToPlayer >= currentSettings.chargeMaxAttackRange)
        {
            Charge();
        }

        if (distToPlayer <= currentSettings.slamMinAttackRange)
        {
            SlamBegin();
        }
    }

    protected override void Idle()
    {
        if (bossFightStarted)
            currentState = States.Fighting;
    }

    void Move()
    {
        moveDir = GetDirTowardsPlayer();
        
        animator.SetBool("isWalking", true);

        rb.velocity = moveDir * currentSettings.walkSpeed * Time.deltaTime * 10;

        if (afterImage == null) 
            CheckToRotate(moveDir, null);
        else
            CheckToRotate(moveDir);
    }

    void Charge()
    {
        isAttacking = true;
        animator.SetBool("isCharging", true);
        animator.SetBool("isWalking", false);
        StartCoroutine(ChargeSequence());
    }
    IEnumerator ChargeSequence()
    {
        isCharging = true;
        moveDir = GetDirTowardsPlayer();
        float timer = 0;

        if(afterImage) afterImage.StartEffect();

        while(timer < currentSettings.chargeLength)
        {
            moveDir = Vector2.Lerp(moveDir, GetDirTowardsPlayer(), currentSettings.chargeSensitivity * Time.deltaTime * 10);
            rb.velocity = moveDir * currentSettings.chargeSpeed * Time.deltaTime * 10;

            if (afterImage == null)
                CheckToRotate(moveDir, null);
            else
                CheckToRotate(moveDir, afterImage);

            yield return null;

            timer += Time.deltaTime;
        }

        if (afterImage) afterImage.StopEffect();

        isAttacking = false;
        animator.SetBool("isCharging", false);

        isCharging = false;

        rb.velocity = Vector3.zero;

        StartCoroutine(WaitBeforeAttack(currentSettings.chargeCooldownMin, currentSettings.chargeCooldownMax));
    }

    void SlamBegin()
    {
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Slam");
        rb.velocity = Vector3.zero;
        isAttacking = true;
    }
    void Slam()
    {
        //Detect things around monster at given radius
        //Deal damage and knockback
        //Possibly spawn ring of projectiles

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentSettings.slamRadius);
        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].tag.Equals("Player"))
            {
                PlayerMovement playerMovement = colliders[i].gameObject.GetComponent<PlayerMovement>();
                IHealth health = colliders[i].gameObject.GetComponent<IHealth>();
                Vector2 dir = (colliders[i].transform.position - transform.position).normalized;

                health.TakeDamage(currentSettings.slamDamage);
                playerMovement.Knockback(dir, currentSettings.slamKnockback);

                break;
            }
        }

        shooter.Attack();
    }
    public void SlamEnd()
    {
        isAttacking = false;
        StartCoroutine(WaitBeforeAttack(currentSettings.slamCooldownMin, currentSettings.slamCooldownMax));
    }

    protected override void Enrage()
    {
        enraged = true;
        currentSettings = enragedAttackSettings;
    }

    protected override void Disenrage()
    {
        enraged = false;
        currentSettings = normalAttackSettings;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Do damage and knockback to player for touching the monster
        //If charging, do even more damage and knockback

        if (collision.gameObject.tag.Equals("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;

            if (isCharging)
            {
                health.TakeDamage(currentSettings.chargeDamage, currentSettings.chargeStun);
                playerMovement.Knockback(dir, currentSettings.chargeKnockback);
            }
            else
            {
                health.TakeDamage(currentSettings.normalContactDamage);
                playerMovement.Knockback(dir, currentSettings.normalKnockback);
            }
        }
    }
}
