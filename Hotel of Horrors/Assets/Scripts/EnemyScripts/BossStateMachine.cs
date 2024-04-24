using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BossHealth))]
public abstract class BossStateMachine : MonoBehaviour
{
    public enum FaceDirection
    {
        Left, Right
    }

    protected FaceDirection currentFaceDir = FaceDirection.Left;

    public enum States
    {
        Idle, Dialogue, Fighting, Death
    }

    protected States currentState = States.Idle;

    protected GameObject player;
    protected Rigidbody2D rb;
    protected bool enraged;
    
    [System.Serializable]
    public struct Stage
    {
        public int stage;
        public int healthThresholdToNextStage;
        public int dialogueBlock;
        public int correctChoice;
    }

    protected BossHealth bossHealth;

    [SerializeField] protected bool debug;
    [SerializeField] protected UIDisplayContainer uiDisplay;
    [SerializeField] protected CameraShake cameraShake;
    [SerializeField] protected bool flipToRotate;
    [SerializeField] Transform transformToFlip;
    [Header("Dialogue Settings")]
    [SerializeField] protected DialogueSystem dialogueSystem;
    //[SerializeField] protected int npcBlock = 2;
    [SerializeField] protected int introBlock = 0;
    [SerializeField] protected string npcFile = "F0D.txt";

    [Header("Boss Stages")]
    [SerializeField] protected Stage[] stages;

    protected int currentStageIndex;

    protected bool onCooldown = false;
    protected bool isAttacking = false;

    protected bool bossFightStarted = false;
    protected bool isDying = false;

    protected bool dialogueSegmentStarted = false;
    public event EventHandler OnBossDialogue;

    private void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>();

        if (cameraShake == null) cameraShake = FindObjectOfType<CameraShake>();

        if (dialogueSystem == null)
            dialogueSystem = FindObjectOfType<DialogueSystem>();

        if (dialogueSystem != null)
            dialogueSystem.SubscribeToBoss(this);

        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (player == null)
            Debug.LogError("Player not found in scene");

        Init();
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
                Dialogue();
                break;
            case States.Fighting:
                Fight();
                break;
            case States.Death:
                Death();
                break;
        }
    }

    protected abstract void Init();

    protected abstract void Fight();
    protected abstract void Idle();
    protected virtual void Dialogue()
    {
        if (!dialogueSegmentStarted)
        {
            if (bossFightStarted)
            {
                NPCArgs bossArg = new NPCArgs(stages[currentStageIndex].dialogueBlock, npcFile);
                OnBossDialogue?.Invoke(this, bossArg);

                dialogueSystem.OnDialogueFinish += DialogueEnd;
            } else
            {
                NPCArgs bossArg = new NPCArgs(introBlock, npcFile);
                OnBossDialogue?.Invoke(this, bossArg);

                dialogueSystem.OnDialogueFinish += DialogueEnd;
            }
        }
    }
    protected abstract void DialogueEnd(object sender, System.EventArgs args);
    protected abstract void Death();
    protected abstract IEnumerator DeathSequence();
    protected virtual void OnDeath()
    {
        isDying = true;
        StartCoroutine(DeathSequence());
    }

    protected abstract void Enrage();
    protected abstract void Disenrage();

    /// <summary>
    /// Flips the sprite if necessary
    /// </summary>
    /// <param name="dir"></param>
    protected void CheckToRotate(Vector2 dir)
    {
        if(flipToRotate)
        {
            float angle = Vector2.SignedAngle(dir, Vector2.up);
            if(angle <= 0 && currentFaceDir == FaceDirection.Right)
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
                    if(psr != null) psr.flip = new Vector3(0, 0, 0);
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

    #region Helper Functions
    protected float GetDistanceToTarget(Vector2 target)
    {
        return Vector2.Distance(this.transform.position, target);
    }
    protected Vector2 GetPosBetweenTarget(Vector2 target, float speed)
    {
        return Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime); // calculate distance to move).normalized;
    }
    protected float GetDistanceToPlayer()
    {
        return Vector2.Distance(this.transform.position, player.transform.position);
    }
    protected Vector2 GetPosBetweenPlayer(float moveSpeed)
    {
        return Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime); // calculate distance to move).normalized;
    }
    protected Vector2 GetDirTowardsPlayer()
    {
        return (player.transform.position - transform.position).normalized;
    }
    protected Vector2 GetRandomDir()
    {
        return UnityEngine.Random.insideUnitCircle; //Get a random position around a unit circle.
    }
    #endregion

    protected IEnumerator WaitBeforeAttack(float minAttackCooldown, float maxAttackCooldown)
    {
        if(debug)
            print("Started cooldown");

        onCooldown = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(minAttackCooldown, maxAttackCooldown));

        if(debug)
            print("Served Cooldown");

        onCooldown = false;
    }
}
