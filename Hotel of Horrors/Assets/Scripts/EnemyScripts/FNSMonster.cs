using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] float chargeCameraShake = 0.65f;
    [SerializeField] float slamCameraShake = 0.9f;
    [SerializeField] float walkingCameraShake = 0.4f;

    AttackSettings currentSettings;

    Vector2 moveDir;
    bool isCharging;

    
    protected override void Init()
    {
        Disenrage();

        currentStageIndex = 0;
    }


    protected override void Death()
    {
        //dialogueSystem.UnsubscribeToBoss(this);
    }

    protected override void Dialogue()
    {
        rb.velocity = Vector3.zero;

        if (!dialogueSegmentStarted)
        {
            base.Dialogue();
            dialogueSegmentStarted = true;
        }
        else
        {

        }
    }

    protected override void Fight()
    {
        if(!isAttacking)
            Move();

        switch(stages[currentStageIndex].stage)
        {
            case 1:
                Fight_Stage1();
                break;
            case 2:
                Fight_Stage2();
                break;
        }

        if(bossHealth.stats.GetHealthValue() <= stages[currentStageIndex].healthThresholdToNextStage)
        {
            currentState = States.Dialogue;
            Disenrage();
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
        float distToPlayer = GetDistanceToPlayer();
        if(distToPlayer <= 3.5f)
        {
            currentState = States.Dialogue;
        }

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
                health.Knockback(dir, currentSettings.slamKnockback);

                break;
            }
        }
        if(cameraShake != null) cameraShake.ShakeCamera(slamCameraShake);
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
                health.Knockback(dir, currentSettings.chargeKnockback);
            }
            else
            {
                health.TakeDamage(currentSettings.normalContactDamage);
                health.Knockback(dir, currentSettings.normalKnockback);
            }
        }
    }

    protected override void DialogueEnd(object sender, EventArgs args)
    {
        if (bossFightStarted)
        {
            IntArgs intArgs = (IntArgs)args;
            int choiceMade = intArgs.GetHeldInteger();

            if (!isDying)
            {
                if (stages[currentStageIndex].correctChoice == choiceMade)
                {
                    Enrage();
                }
                currentStageIndex++;
                currentState = States.Fighting;
            } else
            {
                currentState = States.Death;
                dialogueSystem.UnsubscribeToBoss(this);
            }

            dialogueSegmentStarted = false;

            dialogueSystem.OnDialogueFinish -= DialogueEnd;

        } else
        {
            currentState = States.Idle;
            bossFightStarted = true;

            bossHealth.ShowHealthBar();

            dialogueSegmentStarted = false;

            dialogueSystem.OnDialogueFinish -= DialogueEnd;
        }
    }

    protected override IEnumerator DeathSequence()
    {
        rb.velocity = Vector2.zero;

        currentState = States.Dialogue;
        yield return null;

        yield return new WaitUntil(() => !dialogueSegmentStarted);

        bossHealth.HideHealthBar();
        SceneManager.LoadScene("EndDemo");
    }

    public void WalkCameraShake()
    {
        if (cameraShake != null) cameraShake.ShakeCamera(walkingCameraShake);
    }
    public void ChargeCameraShake()
    {
        if (cameraShake != null) cameraShake.ShakeCamera(chargeCameraShake);
    }
}
