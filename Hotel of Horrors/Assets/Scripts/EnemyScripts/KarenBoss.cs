using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

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
        public int slashDamage;
        public float slashKnockback;
        public float slashStun;
        public float projectileLifetime;
        public float slashCooldownMin;
        public float slashCooldownMax;
        public float cancelStunLength;

        [Header("Slam Settings")]
        public float fallLength;
        [Range(0, 1)] public float shadowChaseIntensity;
        public float shadowSpeed;
        public float slamRadius;
        public float slamDamage;
        public int projectileDamage;
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
    [SerializeField] GameObject defaultProjectile;
    [SerializeField] GameObject gavelProjectile;
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

        defaultProjectile.GetComponent<Projectile>().ModifyDamage(currentSettings.slashDamage);
        gavelProjectile.GetComponent<Projectile>().ModifyDamage(currentSettings.projectileDamage);

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
                FightStage1();
                break;
            case 2:
                FightStage2();
                break;
            case 3:
                FightStage3();
                break;
        }

        if (bossHealth.stats.GetHealthValue() <= stages[currentStageIndex].healthThresholdToNextStage)
        {
            OnDialogueStart(stages[currentStageIndex].cutscene);
            Disenrage();
        }
    }

    void FightStage1()
    {
        if (isAttacking || onCooldown || !canAttack)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if (stages[currentStageIndex].attackSequence[currentAttack] == 0)
        {
            SlashBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 1)
        {
            SlamBegin();
            currentAttack++;
        }

        currentAttack = currentAttack % stages[currentStageIndex].attackSequence.Length;
    }
    void FightStage2()
    {
        if (isAttacking || onCooldown || !canAttack)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if (stages[currentStageIndex].attackSequence[currentAttack] == 0)
        {
            SlashBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 1)
        {
            SlamBegin();
            SlamBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 2)
        {
            if(enraged)
            {
                currentSettings.summonPattern = 0;
                SummonBegin();
                currentSettings.summonPattern = 1;
                SummonBegin();
            }
            currentSettings.summonPattern = Random.Range(0, 1);
            SummonBegin();
            currentAttack++;
        }

        currentAttack = currentAttack % stages[currentStageIndex].attackSequence.Length;
    }
    void FightStage3()
    {
        if (isAttacking || onCooldown || !canAttack)
            return;

        float distToPlayer = GetDistanceToPlayer();

        if (debug) print(distToPlayer);

        if (stages[currentStageIndex].attackSequence[currentAttack] == 0 && !enraged)
        {
            
            SlashBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 1)
        {
            SlamBegin();
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 2)
        {
            if (enraged)
            {
                currentSettings.summonPattern = 2;
                SummonBegin();
            }
            else
            {
                currentSettings.summonPattern = 0;
                SummonBegin();
                currentSettings.summonPattern = 1;
                SummonBegin();
            }
            currentAttack++;
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 3)
        {
            if (enraged)
            {
                jawbreakerPlatform.transform.localScale = new Vector3(2, 2, 2);
                currentSettings.summonPattern = 3;
                SummonBegin();
            }
            else
            {
                jawbreakerPlatform.transform.localScale = new Vector3(1, 1, 1);
                currentSettings.summonPattern = 3;
                SummonBegin();
            }
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
        if (stages[currentStageIndex].stage == 3)
        {
            shooter.ModifyAngleSpread(41);
        }
        shooter.SetBulletPrefab(defaultProjectile);
    }

    void Slash()
    {
        if (stages[currentStageIndex].stage == 1)
        {
            shooter.GetBulletPrefab().GetComponent<Projectile>().destroyTimer = currentSettings.projectileLifetime;
        }
        else if (stages[currentStageIndex].stage == 2)
        {
            shooter.GetBulletPrefab().GetComponent<Projectile>().destroyTimer = currentSettings.projectileLifetime * 1.5f;
        }
        else if (stages[currentStageIndex].stage == 3)
        {
            shooter.GetBulletPrefab().GetComponent<Projectile>().destroyTimer = currentSettings.projectileLifetime * 2f;
        }
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
        if(afterImage) afterImage.StartEffect();
        animator.SetTrigger("Jump");
        rb.velocity = Vector3.zero;
        isAttacking = true;
        if (stages[currentStageIndex].stage == 3)
        {
            shooter.ModifyAngleSpread(341);
        }
        shooter.SetBulletPrefab(gavelProjectile);
    }

    void StandardSlam()
    {
        karenShadow.transform.position = transform.position;
        //Jump Up

        if (collider) collider.enabled = false;
        sprite.SetActive(false);
        if (afterImage) afterImage.StopEffect();
        //Have shadow following player and shrink
        karenShadow.SetActive(true);

        float timer = 0;
        Vector2 shadowMoveDir = (player.transform.position - karenShadow.transform.position).normalized;
        Rigidbody2D shadowRigidbody = karenShadow.GetComponent<Rigidbody2D>();
        while (timer < currentSettings.fallLength)
        {
            Debug.Log($"Timer {timer}/{currentSettings.fallLength} | DeltaTime {Time.deltaTime}");
            shadowMoveDir = Vector2.Lerp(shadowMoveDir, (player.transform.position - karenShadow.transform.position).normalized, currentSettings.shadowChaseIntensity * Time.deltaTime * 10);
            shadowRigidbody.velocity = shadowMoveDir * currentSettings.shadowSpeed * Time.fixedDeltaTime * 10;

            timer += Time.fixedDeltaTime;
        }
        shadowRigidbody.velocity = Vector2.zero;
        transform.position = karenShadow.transform.position;
        //Have Karen slam down at where the shadow stopped
        sprite.SetActive(true);

        if (afterImage) afterImage.StartEffect();
        animator.SetTrigger("Slam");
    }

    void StandardSlamPartTwo()
    {
        if (afterImage) afterImage.StopEffect();
        if (cameraShake != null) cameraShake.ShakeCamera(slamCameraShake);
        karenShadow.SetActive(false);
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
        if (stages[currentStageIndex].stage == 3)
        {
            if (enraged)
            {
                shooter.ModiftProjectilesPerBurst(22);
            }
            else
            {
                shooter.ModiftProjectilesPerBurst(11);
            }
            shooter.Attack();
        }

        SlamEnd();
    }
    void SlamEnd()
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
                positionsToUse.Add(summonPlatform.GetChild(4));
                positionsToUse.Add(summonPlatform.GetChild(6));

                if (stages[currentStageIndex].stage == 2)
                {
                    foreach (Transform t in positionsToUse)
                    {
                        summonPlatform.position = player.transform.position;
                        t.gameObject.SetActive(true);
                        yield return new WaitForSeconds(currentSettings.summonShootDelay);
                        GameObject summon = Instantiate(summonProjectile, t.position, t.rotation);
                        summon.GetComponent<Rigidbody2D>().AddForce(summon.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                        t.gameObject.SetActive(false);
                    }
                }
                else if(stages[currentStageIndex].stage == 3)
                {
                    summonPlatform.position = player.transform.position;
                    positionsToUse[0].gameObject.SetActive(true);
                    positionsToUse[1].gameObject.SetActive(true);
                    positionsToUse[2].gameObject.SetActive(true);
                    positionsToUse[3].gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay * .5f);
                    GameObject summon1 = Instantiate(summonProjectile, positionsToUse[0].position, positionsToUse[0].rotation);
                    GameObject summon2 = Instantiate(summonProjectile, positionsToUse[1].position, positionsToUse[1].rotation);
                    GameObject summon3 = Instantiate(summonProjectile, positionsToUse[2].position, positionsToUse[2].rotation);
                    GameObject summon4 = Instantiate(summonProjectile, positionsToUse[3].position, positionsToUse[3].rotation);
                    summon1.GetComponent<Rigidbody2D>().AddForce(summon1.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    summon2.GetComponent<Rigidbody2D>().AddForce(summon2.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    summon3.GetComponent<Rigidbody2D>().AddForce(summon3.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    summon4.GetComponent<Rigidbody2D>().AddForce(summon4.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    positionsToUse[0].gameObject.SetActive(false);
                    positionsToUse[1].gameObject.SetActive(false);
                    positionsToUse[2].gameObject.SetActive(false);
                    positionsToUse[3].gameObject.SetActive(false);
                }
                break;
            case 1:
                //Cross Pattern
                positionsToUse.Add(summonPlatform.GetChild(1));
                positionsToUse.Add(summonPlatform.GetChild(3));
                positionsToUse.Add(summonPlatform.GetChild(5));
                positionsToUse.Add(summonPlatform.GetChild(7));

                if (stages[currentStageIndex].stage == 2)
                {
                    foreach (Transform t in positionsToUse)
                    {

                        summonPlatform.position = player.transform.position;
                        t.gameObject.SetActive(true);
                        yield return new WaitForSeconds(currentSettings.summonShootDelay);
                        GameObject summon = Instantiate(summonProjectile, t.position, t.rotation);
                        summon.GetComponent<Rigidbody2D>().AddForce(summon.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                        t.gameObject.SetActive(false);
                    }
                }
                else if (stages[currentStageIndex].stage == 3)
                {
                    summonPlatform.position = player.transform.position;
                    positionsToUse[0].gameObject.SetActive(true);
                    positionsToUse[1].gameObject.SetActive(true);
                    positionsToUse[2].gameObject.SetActive(true);
                    positionsToUse[3].gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay * .5f);
                    GameObject summon1 = Instantiate(summonProjectile, positionsToUse[0].position, positionsToUse[0].rotation);
                    GameObject summon2 = Instantiate(summonProjectile, positionsToUse[1].position, positionsToUse[1].rotation);
                    GameObject summon3 = Instantiate(summonProjectile, positionsToUse[2].position, positionsToUse[2].rotation);
                    GameObject summon4 = Instantiate(summonProjectile, positionsToUse[3].position, positionsToUse[3].rotation);
                    summon1.GetComponent<Rigidbody2D>().AddForce(summon1.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    summon2.GetComponent<Rigidbody2D>().AddForce(summon2.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    summon3.GetComponent<Rigidbody2D>().AddForce(summon3.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    summon4.GetComponent<Rigidbody2D>().AddForce(summon4.transform.up * currentSettings.summonShootForce, ForceMode2D.Impulse);
                    positionsToUse[0].gameObject.SetActive(false);
                    positionsToUse[1].gameObject.SetActive(false);
                    positionsToUse[2].gameObject.SetActive(false);
                    positionsToUse[3].gameObject.SetActive(false);
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
                    
                    if(Physics2D.OverlapCircle(t.position, .3f, 6))
                    {
                        continue;
                    }

                    t.gameObject.SetActive(true);
                    yield return new WaitForSeconds(currentSettings.summonShootDelay);
                    GameObject summon = Instantiate(jawbreaker, t.position, quaternion.identity);
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

        if (enrageSprite) enrageSprite.SetActive(true);
    }

    protected override void Disenrage()
    {
        enraged = false;
        currentSettings = normalAttackSettings;

        if (enrageSprite) enrageSprite.SetActive(false);
    }

    protected override void OnDialogueEnd()
    {
        if (bossFightStarted)
        {
            int choiceMade = (int)FindObjectOfType<DialogueManager>().GetLastPlayerChoice();

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
            Stun(0.5f, true);
            bossFightStarted = true;

            isAttacking = false;

            bossHealth.ShowHealthBar();

            dialogueSegmentStarted = false;
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
