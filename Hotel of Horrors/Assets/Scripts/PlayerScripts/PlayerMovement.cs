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
    [SerializeField] AfterImage afterImage;
    [SerializeField] Image fadeImage;
    [SerializeField] GameObject elevatorCanvas;
    private Transform playerTrans;

    [Header("Movement Modifiers")]
    public float movementSpeed;
    [SerializeField] [Range(0,1)] float drag;
    
    //public float collisionOffset = 0.05f;
    //public ContactFilter2D movementFilter;
    //List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    [HideInInspector]public bool canMove;

    [Header("Dashing Modifiers")]
    public float dashDistance;
    public float dashSpeed;
    [SerializeField] float iFrameDistForDash;
    bool isDashing = false;

    string direction = "South";

    bool isAttackMode = false;
    float attackModeTimer;

    Vector2 movementVector = new Vector2();
    Vector2 dashStartVector = new Vector2();

    [Header("Room Traversal")]
    [SerializeField] float chanceForElevator;
    [SerializeField] float doorTransitionLength = 0.5f;
    RoomManager roomManager;

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

        attackController = GetComponent<AttackController>();
        playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetBool("isWalking", false);
        animator.SetBool("Dashing", false);

        canMove = true;
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
            if (movementVector != Vector2.zero)
            {
                float velMag = rb.velocity.magnitude;
                movementVector.Normalize();

                if (isDashing)
                {
                    if (Vector2.Distance(dashStartVector, rb.position) <= dashDistance)
                    {
                        rb.MovePosition(rb.position + movementVector.normalized * dashSpeed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        animator.SetBool("Dashing", false);
                        isDashing = false;
                    }
                }
                else
                {
                    rb.velocity = movementVector * movementSpeed;
                }
            }
            else
            {
                animator.SetBool("Dashing", false);
                isDashing = false;
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

        if (!isAttackMode && canMove)
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
        if (!attackController.IsAttacking && inputValue.isPressed)
        {
            dashStartVector = rb.position;
            isDashing = true;

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

            animator.SetTrigger("Dash");
            animator.SetBool("Dashing", true);

            if (afterImage != null)
                afterImage.StartEffect(dashDistance / dashSpeed);

            playerHealth.GiveIFrames(iFrameDistForDash / dashSpeed);
        }
    }
    #endregion

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

        //for now, random chance to enter elevator
        if (Random.Range(0,1) < chanceForElevator)
        {
            print("Going to elevator");
            transform.position = new Vector3(-1000, 0, -1000);
            elevatorCanvas.SetActive(true);
            Cursor.visible = true;



        }

        Door door = obj.GetComponent<Door>();

        //Ensure player is facing the door
        if ((door.doorLocation == Door.DoorLocations.North && direction.Equals("North")) ||
            (door.doorLocation == Door.DoorLocations.South && direction.Equals("South")) ||
            (door.doorLocation == Door.DoorLocations.East && direction.Equals("East")) ||
            (door.doorLocation == Door.DoorLocations.West && direction.Equals("West")))
        {

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
    #endregion
}
