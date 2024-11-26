using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AI))]
public class KarenBoss : BossStateMachine
{
    [System.Serializable]
    public struct AttackSettings
    {
        public float walkSpeed;
        public float normalKnockback;
        public float normalContactDamage;

        [Header("Slash Settings")]
        public float slashDamage;
        public float slashKnockback;
        public float slashStun;
        public float slashCooldownMin;
        public float slashCooldownMax;
        public float cancelStunLength;

        [Header("Slam Settings")]
        public float fallLength;
        [Range(0, 1)] public float shadowChaseIntensity;
        public float shadowSpeed;
        public float slamRadius;
        public float slamDamage;
        public float slamKnockback;
        public float slamCooldownMin;
        public float slamCooldownMax;

        [Header("Summon Settings")]
        public int patternNumber;
        public float patternSpeed;
    }

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] AfterImage afterImage;
    [SerializeField] BasicShooter shooter;
    [SerializeField] AI ai;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform jumpPoint;
    [SerializeField] GameObject karenShadow;
    [SerializeField] GameObject shockwavePrefab;

    [Header("Attack Settings")]
    [SerializeField] AttackSettings normalAttackSettings;
    [SerializeField] AttackSettings enragedAttackSettings;
    [SerializeField] float slamCameraShake = 0.9f;
    [SerializeField] float walkingCameraShake = 0.4f;
    [SerializeField] float slamHitStopLength = 0.1f;
    [SerializeField] LayerMask wallMask;
    AttackSettings currentSettings;

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
        if (!isAttacking && canAttack)
        {
            Move();
        }

        switch (stages[currentStageIndex].stage)
        {
            case 1:
                FightStageOne();
                break;
            case 2:
                break;
            case 3:
                break;
        }

        if (bossHealth.stats.GetHealthValue() <= stages[currentStageIndex].healthThresholdToNextStage)
        {
            OnDialogueStart(stages[currentStageIndex].cutscene);
            Disenrage();
        }
    }

    void FightStageOne()
    {
        if (isAttacking || onCooldown || !canAttack)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if (stages[currentStageIndex].attackSequence[currentAttack] == 0)
        {
            SlamBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 1)
        {

        }

        currentAttack = currentAttack % stages[currentStageIndex].attackSequence.Length;
    }
    protected override void Idle()
    {
        float distToPlayer = GetDistanceToPlayer();
        if (distToPlayer <= 3.5f)
        {
            OnDialogueStart(openCutscene);
        }

        if (bossFightStarted)
            currentState = States.Fighting;
    }

    void SlamBegin()
    {
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Jump");
        rb.velocity = Vector3.zero;
        isAttacking = true;
    }

    IEnumerator StandardSlam()
    {
        karenShadow.transform.position = transform.position;
        //Jump Up

        if (afterImage) afterImage.StartEffect();
        if (collider) collider.enabled = false;
        float interpol = 0;
        while (transform.position.y != jumpPoint.position.y)
        {
            //Debug.Log($"{interpol} | Karen:{transform.position} | target: {jumpPoint.position}");
            transform.position = Vector2.Lerp(transform.position, jumpPoint.position, interpol);

            interpol += 0.05f;
            yield return new WaitForSeconds(.2f);
        }
        transform.position = jumpPoint.position;
        rb.velocity = Vector3.zero;
        //Have shadow following player and shrink

        karenShadow.SetActive(true);

        float timer = 0f;
        Vector2 shadowMoveDir = (player.transform.position - karenShadow.transform.position).normalized;
        Rigidbody2D shadowRigidbody = karenShadow.GetComponent<Rigidbody2D>();
        while (timer < currentSettings.fallLength)
        {
            //Debug.Log($"Timer {timer}/{currentSettings.fallLength}");
            shadowMoveDir = Vector2.Lerp(shadowMoveDir, (player.transform.position - karenShadow.transform.position).normalized, currentSettings.shadowChaseIntensity * Time.deltaTime * 10);
            shadowRigidbody.velocity = shadowMoveDir * currentSettings.shadowSpeed * Time.fixedDeltaTime * 10;

            yield return null;

            timer += Time.deltaTime;
        }
        shadowRigidbody.velocity = Vector2.zero;

        //Have Karen slam down at where the shadow stopped
        animator.SetTrigger("Slam");
        interpol = 0;
        while ((transform.position - karenShadow.transform.position).sqrMagnitude > 0.01f)
        {
            //Debug.Log($"{interpol} | Karen:{transform.position} | target: {karenShadow.transform.position}");
            transform.position = Vector2.Lerp(transform.position, karenShadow.transform.position, interpol);

            interpol += 0.05f;
            yield return new WaitForSeconds(.2f);
        }
        transform.position = karenShadow.transform.position;
        karenShadow.SetActive(false);

        if (cameraShake != null) cameraShake.ShakeCamera(slamCameraShake);
        if (afterImage) afterImage.StopEffect();
        collider.enabled = true;

        //Detect things around monster at given radius
        //Deal damage and knockback
        //Possibly spawn ring of projectiles

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentSettings.slamRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag.Equals("Player"))
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
        SlamEnd();
    }
    public void SlamEnd()
    {
        animator.SetTrigger("SlamEnd");
        isAttacking = false;
        StartCoroutine(WaitBeforeAttack(currentSettings.slamCooldownMin, currentSettings.slamCooldownMax));
    }

    void Move()
    {
        //Thinking of repurposing this to go towards any Vector2 instead of just the player
        //This is mainly so that the boss can go to certain points in the room and go crazy and is less reliant on player position to do stuff
        //This comment is mainly to remind myself that im thinking this stuff.
        animator.SetBool("isWalking", true);

        if (RaycastPlayer(raycastOrigin.position, GetDistanceToPlayer(), wallMask).collider != null)
        {
            if (!routing || arrived)
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

        if (afterImage == null)
            CheckToRotate(ai.CurrentDir, null);
        else
            CheckToRotate(ai.CurrentDir);
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

    protected override void OnDialogueEnd()
    {
        if (bossFightStarted)
        {
            int choiceMade = (int)FindObjectOfType<DialogueManager>().GetLastPlayerChoice();

            if (!isDying)
            {
                if (stages[currentStageIndex].correctChoice == choiceMade)
                {
                    Enrage();
                }
                currentAttack = 0;
                currentStageIndex++;

                Stun(1f, true);
                //currentState = States.Fighting;
            }
            else
            {
                currentState = States.Death;
            }

            dialogueSegmentStarted = false;

        }
        else
        {
            Stun(1f, true);
            bossFightStarted = true;

            bossHealth.ShowHealthBar();

            //dialogueSegmentStarted = false;
        }
    }

    protected override IEnumerator DeathSequence()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        currentState = States.Dialogue;
        yield return null;

        yield return new WaitUntil(() => !dialogueSegmentStarted);

        bossHealth.HideHealthBar();
        SceneManager.LoadScene("EndDemo");
    }

    Coroutine stunCoroutine;
    protected IEnumerator BossStun(float stunLength)
    {
        canAttack = false;

        rb.velocity = Vector2.zero;
        //animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(stunLength);

        if (currentState == States.Stun)
        {
            canAttack = true;
            currentState = States.Fighting;
        }

        stunCoroutine = null;
    }

    public override void Stun(float stunLength, bool overrideCurrent)
    {
        if (overrideCurrent && stunCoroutine != null) StopCoroutine(stunCoroutine);
        if (overrideCurrent || stunCoroutine == null)
        {
            currentState = States.Stun;
            stunCoroutine = StartCoroutine(BossStun(stunLength));
        }
    }

    protected override IEnumerator DialogueStart(Dialogue.Dialog cutscene)
    {
        yield return new WaitForSeconds(1f);
        DialogueManager.SetCutscene(cutscene);
        currentState = States.Dialogue;
    }

    protected override IEnumerator DialogueEnd()
    {
        yield return new WaitForSeconds(1f);
        OnDialogueEnd();
    }
}
