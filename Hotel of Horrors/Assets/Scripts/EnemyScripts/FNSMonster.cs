using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNSMonster : BossStateMachine
{
    [System.Serializable]
    public struct AttackSettings
    {
        public float walkSpeed;

        [Header("Charge Settings")]
        public float chargeSpeed;
        public float chargeLength;
        public float chargeDamage;
        public float chargeKnockback;
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

    private void OnCollisionEnter(Collision collision)
    {
        //Do damage and knockback to player for touching the monster
        //If charging, do even more damage and knockback
    }
}
