using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        public float slamAngleOffsetDifference;
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
    [SerializeField] Transform[] roomPositions;

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
            StandardSlamBegin();
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
    void StandardSlamBegin()
    {
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Slam");
        
        rb.velocity = Vector3.zero;
        isAttacking = true;
        //Jump into the air for certain amount of time
    }
    void StandardSlam()
    {
        //Slam down on the player
        Vector2 lastPlayerPos = playerPos;
        transform.position = (lastPlayerPos * (Vector2.up * 2));
        Wait(3f);


        if (cameraShake != null) cameraShake.ShakeCamera(slamCameraShake);
            
    }
    public void StandardSlamEnd()
    {
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
