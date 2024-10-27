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
        Idle, Dialogue, Fighting, Death, Stun, DialogueStart, DialogueEnd
    }

    protected States currentState = States.Idle;

    protected Floor floor;

    protected GameObject player;
    protected Rigidbody2D rb;
    protected bool enraged;
    
    [System.Serializable]
    public struct Stage
    {
        public int stage;
        public int healthThresholdToNextStage;
        public int[] attackSequence;
        public Dialogue.Dialog cutscene;
        public int correctChoice;
    }

    protected BossHealth bossHealth;

    [SerializeField] protected bool debug;
    [SerializeField] protected CameraShake cameraShake;
    [SerializeField] protected bool flipToRotate;
    [SerializeField] Transform transformToFlip;
    /*[Header("Drop Settings")]
    [SerializeField] MementoInfo mementoInfo;
    [SerializeField] int emotionalEnergyWorth = 500;
    [SerializeField] GameObject holderPrefab;
    [SerializeField] float dropRadius = 0.2f;
    [SerializeField] float startHeight = 0.2f;
    [SerializeField] float upwardsVelocity = 0.2f;*/
    [Header("Dialogue Settings")]
    [SerializeField] protected DialogueManager DialogueManager;
    [SerializeField] protected Dialogue.Dialog openCutscene;

    [Header("Boss Stages")]
    [SerializeField] protected Stage[] stages;
    [SerializeField] protected GameObject enrageSprite;

    protected int currentStageIndex;

    protected int currentAttack;

    protected bool onCooldown = false;
    protected bool canAttack = true;
    protected bool isAttacking = false;

    protected bool bossFightStarted = false;
    protected bool isDying = false;

    protected bool dialogueSegmentStarted = false;
    public event EventHandler OnBossDialogue;
    protected QuestSystem questSystem;

    private void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>();

        questSystem = FindObjectOfType<QuestSystem>();
        floor = FindObjectOfType<Floor>();

        if (cameraShake == null) cameraShake = FindObjectOfType<CameraShake>();


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

    public virtual void CheckHealth()
    {
        if (bossHealth.stats.GetHealthValue() <= stages[currentStageIndex].healthThresholdToNextStage && currentStageIndex < stages.Length - 1 && !dialogueSegmentStarted)
        {
            print(stages[currentStageIndex].cutscene);
            OnDialogueStart(stages[currentStageIndex].cutscene);
            Disenrage();
        }
    }

    protected abstract void Fight();
    protected abstract void Idle();
    public void OnDialogueStart(Dialogue.Dialog cutscene)
    {
        currentState = States.DialogueStart;
        StartCoroutine(DialogueStart(cutscene));
    }
    protected abstract IEnumerator DialogueStart(Dialogue.Dialog cutscene);
    protected virtual void Dialogue()
    {
        rb.velocity = Vector3.zero;

        if (DialogueManager.getCutsceneState() == DialogueManager.CutsceneState.None) 
        { 
            currentState = States.DialogueEnd;
            StartCoroutine(DialogueEnd());
        }
    }
    protected abstract IEnumerator DialogueEnd();
    protected abstract void OnDialogueEnd();
    public abstract void Stun(float stunLength, bool overrideCurrent);
    protected abstract void Death();
    protected abstract IEnumerator DeathSequence();
    public virtual void OnDeath()
    {
        if (Floor.maxFloorUnlocked <= Floor.currentFloor)
            Floor.maxFloorUnlocked = Floor.currentFloor + 1;

        if (currentState != States.Death)
        {
            isDying = true;
            StartCoroutine(DeathSequence());
        }
    }

    protected abstract void Enrage();
    protected abstract void Disenrage();

    /*protected virtual void SpawnMemento()
    {
        GameObject go = Instantiate(holderPrefab, transform.position, Quaternion.identity);
        MementoData data = go.GetComponent<MementoData>();
        if (data) data.SetItemData(mementoInfo, 1, emotionalEnergyWorth);
        CustomRigidbody2D rb = go.GetComponent<CustomRigidbody2D>();
        if (rb)
        {
            rb.Initialize(startHeight, UnityEngine.Random.Range(0f, upwardsVelocity));
            rb.AddForce(GetRandomDir() * dropRadius * 2, ForceMode2D.Impulse);
        }
    }*/

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
    protected RaycastHit2D RaycastPlayer(Vector2 origin, float distance, LayerMask layerMask)
    {
        Vector2 dir = GetDirTowardsPlayer();
        return Physics2D.Raycast(origin, dir, distance, layerMask);
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
