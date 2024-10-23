using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerMovement : MonoBehaviour
{
    #region VariableDeclarations
    [Header("Animators/Controllers")]
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController forwardController;
    [SerializeField] AnimatorOverrideController backController;
    [SerializeField] AnimatorOverrideController leftController;
    [SerializeField] AnimatorOverrideController rightController;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header ("Object References")]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] StatsController stats;
    [SerializeField] AfterImage afterImage;
    [SerializeField] PlayerAudio playerAudio;

    [Header("Movement Modifiers")]
    public float movementSpeed;
    [SerializeField] [Range(0,1)] float drag;
    [SerializeField] ParticleSystem walkParticles;
    [SerializeField] int normalLayer = 13;
    
    //public float collisionOffset = 0.05f;
    //public ContactFilter2D movementFilter;
    //List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    [HideInInspector]public bool canMove;
    [HideInInspector]public bool canDash;
    bool inCutscene;

    [Header("Dashing Modifiers")]
    [SerializeField] float dashCooldown;
    public float dashDistance;
    public float dashSpeed;
    [SerializeField] float iFrameDistForDash;
    [SerializeField] Transform dashPartcileSpawn;
    [SerializeField] GameObject[] dashParticles;
    [SerializeField] int dashLayer = 17;
    bool isDashing = false;
    float currentDashLength;

    string direction = "South";

    bool isAttackMode = false;
    float attackModeTimer;

    Vector2 movementVector = new Vector2();
    Vector2 dashMovementVector = new Vector2();

    ActionController attackController;

    #endregion

    public bool IsDashing { get => isDashing; }
    public string Direction { get => direction; }
    public Vector2 MovementVector { get => movementVector; }

    // Start is called before the first frame update
    void Start()
    {

        if (stats == null) stats = GetComponent<StatsController>();

        attackController = GetComponent<ActionController>();
        playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetBool("isWalking", false);
        animator.SetBool("Dashing", false);

        canMove = canDash = true;

        gameObject.layer = normalLayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing && isAttackMode)
        {
            if (!attackController.IsAttacking || attackController.CurrentAttackMode == Attacks.AttackModes.Ranged)
            {
                Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
                float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                if (angle > -45 && angle <= 45 && !direction.Equals("North"))
                {
                    //back
                    direction = "North";
                    //animator.runtimeAnimatorController = backController;
                }
                else if (angle <= -45 && angle > -135 && !direction.Equals("West"))
                {
                    //left
                    direction = "West";
                    //animator.runtimeAnimatorController = leftController;
                }
                else if (angle <= -135 || angle > 135 && !direction.Equals("South"))
                {
                    //forward
                    direction = "South";
                    //animator.runtimeAnimatorController = forwardController;
                }
                else if (angle <= 135 && angle > 45 && !direction.Equals("East"))
                {
                    //right
                    direction = "East";
                    //animator.runtimeAnimatorController = rightController;
                }

                if(angle >= 0 && angle <= 180)
                {
                    direction = "East";
                    spriteRenderer.flipX = true;

                    Vector3 scale = afterImage.transform.localScale;
                    scale.x *= 1;
                    afterImage.transform.localScale = scale;
                }
                else
                {
                    direction = "West";
                    spriteRenderer.flipX = false;

                    Vector3 scale = afterImage.transform.localScale;
                    scale.x = -1;
                    afterImage.transform.localScale = scale;
                }
            }

            if (movementVector.x <= -0.1f || movementVector.x >= 0.1f || movementVector.y <= -0.1f || movementVector.y >= 0.1f)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

            attackModeTimer -= Time.deltaTime;

            if (attackModeTimer <= 0)
                isAttackMode = false;
        }
    }

    #region MovementLogic
    private void FixedUpdate()
    {
        if (canMove)
        {
            if (isDashing)
            {
                if (currentDashLength <= dashDistance / dashSpeed)
                {
                    gameObject.layer = dashLayer;

                    rb.MovePosition(rb.position + dashMovementVector * (dashSpeed + stats.GetCurrentValue(Stat.StatType.MovementSpeed)) * Time.fixedDeltaTime);
                    currentDashLength += Time.fixedDeltaTime;
                }
                else
                {
                    if (afterImage != null) afterImage.StopEffect();
                    animator.SetBool("Dashing", false);
                    isDashing = false;
                    StartCoroutine(DashCooldown());

                    gameObject.layer = normalLayer;
                }
            }
            else if (movementVector != Vector2.zero)
            {
                float velMag = rb.velocity.magnitude;
                movementVector.Normalize();

                rb.velocity = movementVector * (movementSpeed + stats.GetCurrentValue(Stat.StatType.MovementSpeed));
            }
            else
            {
/*                if (afterImage != null) afterImage.StopEffect();
                animator.SetBool("Dashing", false);
                isDashing = false;*/
            }

            if (Mathf.Approximately(movementVector.x, 0))
            {
                Vector2 newVelocity = rb.velocity;
                newVelocity.x *= (1 - drag) * Time.fixedDeltaTime * 10;
                rb.velocity = newVelocity;
            }
            if (Mathf.Approximately(movementVector.y, 0))
            {
                Vector2 newVelocity = rb.velocity;
                newVelocity.y *= (1 - drag) * Time.fixedDeltaTime * 10;
                rb.velocity = newVelocity;
            }
        }
    }

    public void OnWASD(InputValue inputValue)
    {
        movementVector = inputValue.Get<Vector2>();

        if (!isDashing && !isAttackMode && canMove)
        {
            AnimateMovement();
        }
    }

    private void AnimateMovement()
    {
        if (movementVector.y >= 0.1f)
        {
            direction = "North";
            //animator.runtimeAnimatorController = backController;
        }
        else if (movementVector.y <= -0.1f)
        {
            direction = "South";
            //animator.runtimeAnimatorController = forwardController;
        }
        
        if (movementVector.x <= -0.1f)
        {
            direction = "West";
            spriteRenderer.flipX = false;
            //animator.runtimeAnimatorController = leftController;
        }
        else if (movementVector.x >= 0.1f)
        {
            direction = "East";
            spriteRenderer.flipX = true;
            //animator.runtimeAnimatorController = rightController;
        }

        if (movementVector.x <= -0.1f || movementVector.x >= 0.1f || movementVector.y <= -0.1f || movementVector.y >= 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void OnShift(InputValue inputValue)
    {
        if (!isDashing && canDash && !attackController.IsAttacking && inputValue.isPressed)
        {
            if (movementVector.y >= 0.1f)
            {
                if (dashPartcileSpawn && dashParticles[0]) Destroy(Instantiate(dashParticles[0], dashPartcileSpawn.position, dashPartcileSpawn.rotation), 1f);

                direction = "North";
                //animator.runtimeAnimatorController = backController;
            }
            else if (movementVector.y <= -0.1f)
            {
                if (dashPartcileSpawn && dashParticles[1]) Destroy(Instantiate(dashParticles[1], dashPartcileSpawn.position, dashPartcileSpawn.rotation), 1f);

                direction = "South";
                //animator.runtimeAnimatorController = forwardController;
            }
            else if (movementVector.x <= -0.1f)
            {
                if (dashPartcileSpawn && dashParticles[2]) Destroy(Instantiate(dashParticles[2], dashPartcileSpawn.position, dashPartcileSpawn.rotation), 1f);

                direction = "West";
                //animator.runtimeAnimatorController = leftController;
            }
            else if (movementVector.x >= 0.1f)
            {
                if (dashPartcileSpawn && dashParticles[3]) Destroy(Instantiate(dashParticles[3], dashPartcileSpawn.position, dashPartcileSpawn.rotation), 1f);

                direction = "East";
                //animator.runtimeAnimatorController = rightController;
            } else
            {
                return; 
            }

            dashMovementVector = movementVector.normalized;

            isDashing = true;
            currentDashLength = 0;

            playerAudio.Dash();

            animator.SetTrigger("Dash");
            animator.SetBool("Dashing", true);

            if (afterImage != null)
                afterImage.StartEffect(dashDistance / dashSpeed / 1.25f);

            playerHealth.GiveIFrames(iFrameDistForDash / dashSpeed);
        }
    }
    #endregion

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //Player attacked so switch to attack mode
    public void Attacked()
    {
        isAttackMode = true;
        attackModeTimer = 5;
    }

    public void Knockback(Vector2 dir, float strength, float stunLength)
    {
        rb.AddForce(dir * strength, ForceMode2D.Impulse);
        Stun(stunLength);
    }
    public void Knockback(Vector2 dir, float strength)
    {
        rb.AddForce(dir * strength, ForceMode2D.Impulse);
    }

    Coroutine stunCoroutine;
    public void Stun(float length)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunSequence(length));
    }
    IEnumerator StunSequence(float length)
    {
        canMove = false;

        isDashing = false;
        animator.SetBool("Dashing", false);

        yield return new WaitForSeconds(length);

        if(!inCutscene)
            canMove = true;

        if (!isAttackMode)
            AnimateMovement();
    }

    public void MeleeLunge(Vector2 dir, float strength)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * strength, ForceMode2D.Impulse);
    }

    public void EmitWalkParticles()
    {
        if (walkParticles != null)
        {
            walkParticles.Stop();
            walkParticles.Play();
        }
    }

    public void TogglePlayerControls(bool toggle)
    {
        canMove = toggle;

        if(attackController == null) attackController = GetComponent<ActionController>();
        attackController.ToggleAttackControls(toggle);
    }

    public void SetDirection(string newDirection)
    {
        this.direction = newDirection;
    }

    public void StartCutscene()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        animator.SetBool("Dashing", false);

        TogglePlayerControls(false);
        inCutscene = true;
    }

    public void EndCutscene()
    {
        rb.velocity = Vector2.zero;

        TogglePlayerControls(true);
        inCutscene = false;
    }
}
