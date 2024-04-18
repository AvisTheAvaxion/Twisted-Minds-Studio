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

    [Header ("Object References")]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] StatsController stats;
    [SerializeField] AfterImage afterImage;
    [SerializeField] Image fadeImage;
    [SerializeField] GameObject elevatorCanvas;
    [SerializeField] GameObject mindRoomCanvas;
    private Transform playerTrans;

    [Header("Movement Modifiers")]
    public float movementSpeed;
    [SerializeField] [Range(0,1)] float drag;
    
    //public float collisionOffset = 0.05f;
    //public ContactFilter2D movementFilter;
    //List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    [HideInInspector]public bool canMove;
    [HideInInspector]public bool canDash;

    [Header("Dashing Modifiers")]
    [SerializeField] float dashCooldown;
    public float dashDistance;
    public float dashSpeed;
    [SerializeField] float iFrameDistForDash;
    bool isDashing = false;
    float currentDashLength;

    string direction = "South";

    bool isAttackMode = false;
    float attackModeTimer;

    Vector2 movementVector = new Vector2();
    Vector2 dashMovementVector = new Vector2();

    [Header("Room Traversal")]
    [SerializeField] float doorTransitionLength = 0.5f;
    RoomManager roomManager;
    bool inRangeOfChair;

    AttackController attackController;

    #endregion

    public bool CanMove { get => canMove; }
    public bool IsDashing { get => isDashing; }
    public string Direction { get => direction; }

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = player.transform;
        roomManager = GameObject.Find("Room Manager")?.GetComponent<RoomManager>();

        if (stats == null) stats = GetComponent<StatsController>();

        attackController = GetComponent<AttackController>();
        playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetBool("isWalking", false);
        animator.SetBool("Dashing", false);

        canMove = canDash = true;
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
                    animator.runtimeAnimatorController = backController;
                }
                else if (angle <= -45 && angle > -135 && !direction.Equals("West"))
                {
                    //left
                    direction = "West";
                    animator.runtimeAnimatorController = leftController;
                }
                else if (angle <= -135 || angle > 135 && !direction.Equals("South"))
                {
                    //forward
                    direction = "South";
                    animator.runtimeAnimatorController = forwardController;
                }
                else if (angle <= 135 && angle > 45 && !direction.Equals("East"))
                {
                    //right
                    direction = "East";
                    animator.runtimeAnimatorController = rightController;
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
                    rb.MovePosition(rb.position + dashMovementVector * (dashSpeed + stats.GetCurrentValue(Stat.StatType.MovementSpeed)) * Time.fixedDeltaTime);
                    currentDashLength += Time.fixedDeltaTime;
                }
                else
                {
                    if (afterImage != null) afterImage.StopEffect();
                    animator.SetBool("Dashing", false);
                    isDashing = false;
                    StartCoroutine(DashCooldown());
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
            animator.runtimeAnimatorController = backController;
        }
        else if (movementVector.y <= -0.1f)
        {
            direction = "South";
            animator.runtimeAnimatorController = forwardController;
        }
        else if (movementVector.x <= -0.1f)
        {
            direction = "West";
            animator.runtimeAnimatorController = leftController;
        }
        else if (movementVector.x >= 0.1f)
        {
            direction = "East";
            animator.runtimeAnimatorController = rightController;
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
                direction = "North";
                animator.runtimeAnimatorController = backController;
            }
            else if (movementVector.y <= -0.1f)
            {
                direction = "South";
                animator.runtimeAnimatorController = forwardController;
            }
            else if (movementVector.x <= -0.1f)
            {
                direction = "West";
                animator.runtimeAnimatorController = leftController;
            }
            else if (movementVector.x >= 0.1f)
            {
                direction = "East";
                animator.runtimeAnimatorController = rightController;
            } else
            {
                return; 
            }

            dashMovementVector = movementVector.normalized;

            isDashing = true;
            currentDashLength = 0;

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

        canMove = true;

        if (!isAttackMode)
            AnimateMovement();
    }

    #region RoomTraversal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        GameObject obj = collision.gameObject;
        if (obj.tag.Equals("Door") || obj.tag.Equals("WestDoor")|| obj.tag.Equals("EastDoor") || obj.tag.Equals("NorthDoor")|| obj.tag.Equals("SouthDoor"))
        {
            
            StartCoroutine(FadeImageOut(obj));
        }
    }

    IEnumerator FadeImageOut(GameObject obj)
    {
        Door door = obj.GetComponent<Door>();

        //Ensure player is facing the door
        if ((door.doorLocation == Door.DoorLocations.North && direction.Equals("North")) ||
            (door.doorLocation == Door.DoorLocations.South && direction.Equals("South")) ||
            (door.doorLocation == Door.DoorLocations.East && direction.Equals("East")) ||
            (door.doorLocation == Door.DoorLocations.West && direction.Equals("West")) ||
            (door.doorLocation == Door.DoorLocations.Special))
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            canMove = false;
            print("fading black");
            AudioManager.Play("Door");
            // loop over 1 second - fade to black
            for (float i = 0; i <= doorTransitionLength / 2f; i += Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
                yield return null;
            }

            if (door.assignedDoor == null)
            {
                door.assignedDoor = roomManager.GetNextRoom(door.doorLocation);
            }


            switch (door.doorLocation)
            {
                case Door.DoorLocations.North:
                    print("North");
                    this.transform.position = door.assignedDoor.transform.position - new Vector3(0f, -0.4f, 0);
                    direction = "North";
                    animator.runtimeAnimatorController = backController;
                    break;
                case Door.DoorLocations.South:
                    print("South");
                    this.transform.position = door.assignedDoor.transform.position - new Vector3(0, 0.4f, 0);
                    direction = "South";
                    animator.runtimeAnimatorController = forwardController;
                    break;
                case Door.DoorLocations.East:
                    print("East");
                    this.transform.position = door.assignedDoor.transform.position - new Vector3(-0.4f, 0, 0);
                    direction = "East";
                    animator.runtimeAnimatorController = rightController;
                    break;
                case Door.DoorLocations.West:
                    print("West");
                    this.transform.position = door.assignedDoor.transform.position - new Vector3(0.4f, 0, 0);
                    direction = "West";
                    animator.runtimeAnimatorController = leftController;
                    break;
                case Door.DoorLocations.Special:
                    print("special");
                    this.transform.position = GetSpecialRoom(door);
                    direction = "North";
                    animator.runtimeAnimatorController = leftController;
                    break;
            }

            print("fading clear");
            // loop over 1 second backwards - fade to clear
            for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
                yield return null;
            }
            canMove = true;

            if (movementVector.x <= -0.1f || movementVector.x >= 0.1f || movementVector.y <= -0.1f || movementVector.y >= 0.1f)
                animator.SetBool("isWalking", true);
            else
                animator.SetBool("isWalking", false);
        }
    }

    Vector3 GetSpecialRoom(Door door)
    {
        switch (door.gameObject.name)
        {
            case "Elevator Room Door":
                print("Going to elevator");
                elevatorCanvas.SetActive(true);
                Cursor.visible = true;
                return this.transform.position;
            case "Boss Room Door":
                return GameObject.Find("Boss Room Spawn").transform.position;
            case "Mind Room Door":
                return GameObject.Find("Mind Room Spawn").transform.position;
            case "Return Mind Room Door":
                return GameObject.Find("Mind Room Return").transform.position;
        }

        print("No Dice");
        return this.transform.position;
    }
    #endregion

    #region MindRoomInteraction
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("MindRoomChair"))
        {
            inRangeOfChair = true;
            
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("MindRoomChair"))
        {
            inRangeOfChair = false;
        }
    }

    void OnInteract()
    {
        print("interacting");
        if (inRangeOfChair)
        {
            mindRoomCanvas.SetActive(true);
            Cursor.visible = true;
        }
    }

    #endregion
}
