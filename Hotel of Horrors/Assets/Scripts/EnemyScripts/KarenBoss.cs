using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
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
        public float slamMinAttackRange;

        [Header("Summon Settings")]
        [Range(0,2)]public int summonPattern;
        public float patternSpeed;
        public float summonShootForce;
        public float summonShootDelay;
        public float summonCooldownMin;
        public float summonCooldownMax;
    }

    [Header("References")]
    [SerializeField] GameObject sprite;
    [SerializeField] Animator animator;
    [SerializeField] AfterImage afterImage;
    [SerializeField] BasicShooter shooter;
    [SerializeField] AI ai;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform jumpPoint;
    [SerializeField] GameObject karenShadow;
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] Transform summonPlatform;
    [SerializeField] Transform jawbreakerPlatform;
    [SerializeField] GameObject summonProjectile;
    [SerializeField] GameObject jawbreaker;

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

        if (stages[currentStageIndex].attackSequence[currentAttack] == 0 && distToPlayer <= currentSettings.slamMinAttackRange)
        {
            SlashBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 0 && distToPlayer >= currentSettings.slamMinAttackRange)
        {
            SlamBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 1)
        {
            SummonBegin();
            currentAttack++;
        }
        else
        {
            currentAttack++;
        }

        currentAttack = currentAttack % stages[currentStageIndex].attackSequence.Length;
    }
    protected override void Idle()
    {
        float distToPlayer = GetDistanceToPlayer();
        if (distToPlayer <= 3.5f)
        {
            //OnDialogueStart(openCutscene);
        }

        if (bossFightStarted)
            currentState = States.Fighting;
    }

    void SlashBegin()
    {
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Slash");
        rb.velocity = Vector3.zero;
        isAttacking = true;
    }

    void Slash()
    {
        shooter.Attack();
        SlamEnd();
    }

    void SlashEnd()
    {
        animator.SetBool("isWalking", true);
        isAttacking = false;
        StartCoroutine(WaitBeforeAttack(currentSettings.slashCooldownMin, currentSettings.slashCooldownMax));
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

        //if (afterImage) afterImage.StartEffect();
        if (collider) collider.enabled = false;
        float timer = 0f;
        while (timer < 1)
        {
            Debug.Log($"Karen:{transform.position} | timer {timer}");
            sprite.transform.position += (Vector3)Vector2.up;
            timer += Time.deltaTime;
        }
        sprite.SetActive(false);
        //Have shadow following player and shrink

        karenShadow.SetActive(true);

        timer = 0f;
        Vector2 shadowMoveDir = (player.transform.position - karenShadow.transform.position).normalized;
        Rigidbody2D shadowRigidbody = karenShadow.GetComponent<Rigidbody2D>();
        while (timer < currentSettings.fallLength)
        {
            //Debug.Log($"Timer {timer}/{currentSettings.fallLength}");
            shadowMoveDir = Vector2.Lerp(shadowMoveDir, (player.transform.position - karenShadow.transform.position).normalized, currentSettings.shadowChaseIntensity * Time.deltaTime * 10);
            shadowRigidbody.velocity = shadowMoveDir * currentSettings.shadowSpeed * Time.fixedDeltaTime * 10;

            timer += Time.deltaTime;
        }
        shadowRigidbody.velocity = Vector2.zero;

        //Have Karen slam down at where the shadow stopped
        sprite.SetActive(true);
        while (sprite.transform.position.y != 0)
        {
            //Debug.Log($"{interpol} | Karen:{transform.position} | target: {karenShadow.transform.position}");
            sprite.transform.position += (Vector3)Vector2.down;
            yield return new WaitForSeconds(.2f);
        }
        sprite.transform.position = karenShadow.transform.position;
        karenShadow.SetActive(false);


        if (cameraShake != null) cameraShake.ShakeCamera(slamCameraShake);
        //if (afterImage) afterImage.StopEffect();
        collider.enabled = true;

        //Detect things around monster at given radius
        //Deal damage and knockback
        //Possibly spawn ring of projectiles

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentSettings.slamRadius);
        animator.SetTrigger("Slam");
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

    void SummonBegin()
    {
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        isAttacking = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("Summon", true);
    }

    IEnumerator Summon()
    {
        List<Transform> positionsToUse = new List<Transform>();
        switch (currentSettings.summonPattern)
        {
            case 0:
                //Horizontal Vertical Patttern
                positionsToUse.Add(summonPlatform.GetChild(0));
                positionsToUse.Add(summonPlatform.GetChild(2));

                foreach (Transform t in positionsToUse)
                {
                    summonPlatform.position = player.transform.position;
                    t.gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay);
                    GameObject summon = Instantiate(summonProjectile, t.position, t.rotation);
                    summon.GetComponent<Rigidbody2D>().AddForce(summon.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    t.gameObject.SetActive(false);
                }
                break;
            case 1:
                //Cross Pattern
                positionsToUse.Add(summonPlatform.GetChild(1));
                positionsToUse.Add(summonPlatform.GetChild(3));

                foreach (Transform t in positionsToUse)
                {
                    summonPlatform.position = player.transform.position;
                    t.gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay);
                    GameObject summon = Instantiate(summonProjectile, t.position, t.rotation);
                    summon.GetComponent<Rigidbody2D>().AddForce(summon.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    t.gameObject.SetActive(false);
                }
                break;
            case 2:
                //Circle Pattern
                foreach (Transform t in summonPlatform)
                {
                    summonPlatform.position = player.transform.position;
                    t.gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay);
                    GameObject summon = Instantiate(summonProjectile, t.position, t.rotation);
                    summon.GetComponent<Rigidbody2D>().AddForce(summon.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    t.gameObject.SetActive(false);
                }
                break;
            case 3:
                //Jawbreaker Summon
                foreach (Transform t in jawbreakerPlatform)
                {
                    jawbreakerPlatform.position = player.transform.position;
                    t.gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay);
                    GameObject summon = Instantiate(jawbreaker, t.position, t.rotation);
                    t.gameObject.SetActive(false);
                }
                break;
        }
        positionsToUse.Clear();
        SummonEnd();
    }

    void SummonEnd()
    {
        animator.SetBool("Summon", false);
        animator.SetBool("isWalking", true);
        isAttacking = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        StartCoroutine(WaitBeforeAttack(currentSettings.summonCooldownMin, currentSettings.summonCooldownMax));
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

        if (!dialogueSegmentStarted)
        {
            print(stages[stages.Length - 1].cutscene);
            OnDialogueStart(stages[stages.Length - 1].cutscene);
        }
        //yield return new WaitUntil(() => !dialogueSegmentStarted);

        bossHealth.HideHealthBar();
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
