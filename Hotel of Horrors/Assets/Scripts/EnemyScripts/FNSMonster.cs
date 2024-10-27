using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AI))]
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
        public float cancelStunLength;

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
    [SerializeField] AI ai;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] ParticleSystem stompPSLeft;
    [SerializeField] ParticleSystem stompPSRight;

    [Header("Attack Settings")]
    [SerializeField] AttackSettings normalAttackSettings;
    [SerializeField] AttackSettings enragedAttackSettings;
    [SerializeField] float chargeCameraShake = 0.65f;
    [SerializeField] float chargeCancelCamShake = 0.8f;
    [SerializeField] float slamCameraShake = 0.9f;
    [SerializeField] float walkingCameraShake = 0.4f;
    [SerializeField] float slamHitStopLength = 0.1f;
    [SerializeField] float chargeHitStopLength = 0.1f;
    [SerializeField] LayerMask wallMask;
    [SerializeField] GameObject shockwavePrefab;

    AttackSettings currentSettings;

    Vector2 moveDir;
    bool isCharging;

    Vector2 playerPos;
    bool routing = false;
    bool arrived = false;
    
    protected override void Init()
    {
        Disenrage();

        ai = GetComponent<AI>();

        currentStageIndex = 0;
    }


    protected override void Death()
    {
    }

    protected override void Fight()
    {
        if (isDying) return;

        if(!isAttacking && canAttack)
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

        /*if(bossHealth.stats.GetHealthValue() <= stages[currentStageIndex].healthThresholdToNextStage && currentStageIndex < stages.Length - 1)
        {
            print(stages[currentStageIndex].cutscene);
            OnDialogueStart(stages[currentStageIndex].cutscene);
            Disenrage();
        }*/
    }

    void Fight_Stage1()
    {
        if (isAttacking || onCooldown || !canAttack)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if(stages[currentStageIndex].attackSequence[currentAttack] == 0 && distToPlayer <= currentSettings.slamMinAttackRange)
        {
            SlamBegin();
            currentAttack++;
        }
        else if(stages[currentStageIndex].attackSequence[currentAttack] == 1 && distToPlayer <= currentSettings.chargeMaxAttackRange)
        {
            Charge();
            currentAttack++;
        }

        currentAttack = currentAttack % stages[currentStageIndex].attackSequence.Length;
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
            OnDialogueStart(openCutscene);
        }

        if (bossFightStarted)
            currentState = States.Fighting;
    }

    void Move()
    {
        animator.SetBool("isWalking", true);

        if(RaycastPlayer(raycastOrigin.position, GetDistanceToPlayer(), wallMask).collider != null)
        {
            if(!routing || arrived)
            {
                playerPos = player.transform.position;
            }
            routing = true;
        }
        else
        {
            routing = false;
            playerPos = player.transform.position;
        }

        ai.SetSpeed(currentSettings.walkSpeed);
        arrived = !ai.MoveTowardsTarget(playerPos);

        CheckToRotate(ai.CurrentDir, afterImage);
    }

    Coroutine chargeCoroutine;
    void Charge()
    {
        isAttacking = true;
        animator.SetBool("isCharging", true);
        animator.SetBool("isWalking", false);
        chargeCoroutine = StartCoroutine(ChargeSequence());
    }
    bool cancelCharge;
    IEnumerator ChargeSequence()
    {
        isCharging = true;
        cancelCharge = false;

        moveDir = GetDirTowardsPlayer();
        float timer = 0;

        if(afterImage) afterImage.StartEffect();

        while(!cancelCharge && timer < currentSettings.chargeLength)
        {
            moveDir = Vector2.Lerp(moveDir, GetDirTowardsPlayer(), currentSettings.chargeSensitivity * Time.deltaTime * 10);
            rb.velocity = moveDir * currentSettings.chargeSpeed * Time.fixedDeltaTime * 10;

            CheckToRotate(moveDir, afterImage);

            yield return null;

            timer += Time.deltaTime;
        }

        if (afterImage) afterImage.StopEffect();

        isAttacking = false;
        animator.SetBool("isCharging", false);

        isCharging = false;

        rb.velocity = Vector3.zero;

        if(cancelCharge)
        {
            animator.SetBool("ChargeStun", true);
            if (cameraShake != null) cameraShake.ShakeCamera(chargeCancelCamShake, 0.6f, false);
            Stun(currentSettings.cancelStunLength, true);
        }

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

                if (health.TakeDamage(currentSettings.slamDamage))
                {
                    health.Knockback(dir, currentSettings.slamKnockback);
                    GameTime.AddHitStop(slamHitStopLength);
                }

                break;
            }
        }
        Destroy(Instantiate(shockwavePrefab, raycastOrigin.transform.position, Quaternion.identity), 1.5f);
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

        if (enrageSprite) enrageSprite.SetActive(true);
    }

    protected override void Disenrage()
    {
        enraged = false;
        currentSettings = normalAttackSettings;

        if (enrageSprite) enrageSprite.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Do damage and knockback to player for touching the monster
        //If charging, do even more damage and knockback

        if (isCharging && collision.gameObject.tag.Equals("Wall"))
        {
            cancelCharge = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (currentState == States.Fighting && collision.gameObject.tag.Equals("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;

            if (isCharging)
            {
                if (health.TakeDamage(currentSettings.chargeDamage, currentSettings.chargeStun))
                {
                    health.Knockback(dir, currentSettings.chargeKnockback);
                    GameTime.AddHitStop(chargeHitStopLength);
                }
            }
            else if (!isAttacking && currentSettings.normalContactDamage > 0)
            {
                if(health.TakeDamage(currentSettings.normalContactDamage))
                    health.Knockback(dir, currentSettings.normalKnockback);
            }
        }
    }

    protected override void OnDialogueEnd()
    {
        if (bossFightStarted)
        {
            int choiceMade = (int)FindObjectOfType<DialogueManager>().GetLastPlayerChoice();
            //print("Choice Made: " + choiceMade);
            if (!isDying)
            {
                if (stages[currentStageIndex].correctChoice != choiceMade)
                {
                    Enrage();
                }
                currentAttack = 0;
                currentStageIndex++;

                Stun(0.5f, true);

                isAttacking = false;
                isCharging = false;

                //currentState = States.Fighting;
            } else
            {
                currentState = States.Death;
            }

            dialogueSegmentStarted = false;

        } else
        {
            Stun(0.5f, true);
            bossFightStarted = true;

            isAttacking = false;
            isCharging = false;

            bossHealth.ShowHealthBar();

            dialogueSegmentStarted = false;
        }
    }

    protected override IEnumerator DeathSequence()
    {
        currentState = States.Death;

        cancelCharge = true;
        yield return null;
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        animator.SetBool("ChargeStun", false);
        animator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
        animator.ResetTrigger("Slam");
        animator.SetTrigger("Death");
        Disenrage();

        if (!dialogueSegmentStarted)
        {
            print(stages[stages.Length - 1].cutscene);
            OnDialogueStart(stages[stages.Length - 1].cutscene);
        }
        bossHealth.HideHealthBar();

        //yield return new WaitUntil(() => !dialogueSegmentStarted);

        //SceneManager.LoadScene("EndDemo");
    }

    Coroutine stunCoroutine;
    protected IEnumerator BossStun(float stunLength)
    {
        canAttack = false;

        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(stunLength);

        if (currentState == States.Stun)
        {
            canAttack = true;
            currentState = States.Fighting;
        }

        animator.SetBool("ChargeStun", false);

        stunCoroutine = null;
    }

    public void WalkCameraShake()
    {
        if (cameraShake != null) cameraShake.ShakeCamera(walkingCameraShake);
    }
    public void ChargeCameraShake()
    {
        if (cameraShake != null) cameraShake.ShakeCamera(chargeCameraShake);
    }

    public override void Stun(float stunLength, bool overrideCurrent)
    {
        if (overrideCurrent && stunCoroutine != null) StopCoroutine(stunCoroutine);
        if(overrideCurrent || stunCoroutine == null)
        {
            currentState = States.Stun;
            stunCoroutine = StartCoroutine(BossStun(stunLength));
        }
    }

    protected override IEnumerator DialogueStart(Dialogue.Dialog cutscene)
    {
        dialogueSegmentStarted = true;

        //Cancel anything happening already
        cancelCharge = true;
        yield return null;
        if(stunCoroutine != null) StopCoroutine(stunCoroutine);
        animator.SetBool("ChargeStun", false);
        animator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.1f);
        DialogueManager.SetCutscene(cutscene);
        currentState = States.Dialogue;
    }

    protected override IEnumerator DialogueEnd()
    {
        yield return new WaitForSeconds(0.1f);
        OnDialogueEnd();
    }

    public void EmitStompParticlesLeft()
    {
        if(stompPSLeft != null)
        {
            stompPSLeft.Stop();
            stompPSLeft.Play();
        }
    }
    public void EmitStompParticlesRight()
    {
        if (stompPSRight != null)
        {
            stompPSRight.Stop();
            stompPSRight.Play();
        }
    }
}
