using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AI))]
public class KarenBoss : BossStateMachine
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] AfterImage afterImage;
    [SerializeField] BasicShooter shooter;
    [SerializeField] AI ai;
    [SerializeField] Transform raycastOrigin;

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

        }

        switch (stages[currentStageIndex].stage)
        {
            case 1:
                break;
            case 2:
                break;
        }

        if (bossHealth.stats.GetHealthValue() <= stages[currentStageIndex].healthThresholdToNextStage)
        {
            OnDialogueStart(stages[currentStageIndex].cutscene);
            Disenrage();
        }
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

    protected override void Enrage()
    {
        enraged = true;
        //currentSettings = enragedAttackSettings;
    }

    protected override void Disenrage()
    {
        enraged = false;
        //currentSettings = normalAttackSettings;
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
        animator.SetBool("isWalking", false);

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
