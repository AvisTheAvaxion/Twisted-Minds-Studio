using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region VariableDeclarations
    [Header("Animators/Controllers")]
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController frontController;
    [SerializeField] AnimatorOverrideController backController;
    [SerializeField] AnimatorOverrideController leftController;
    [SerializeField] AnimatorOverrideController rightController;

    [Header ("Object References")]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    private Transform playerTrans;

    [Header("Movement Modifiers")]
    public float movementSpeed;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    [Header("Dashing Modifiers")]
    public int dashDistance;
    public float dashSpeed;
    bool isDashing = false;

    Vector2 movementVector = new Vector2();
    Vector2 dashStartVector = new Vector2();
    RoomManager roomManager;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = player.transform;
        roomManager = GameObject.Find("Room Manager").GetComponent<RoomManager>();

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isHolding", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region MovementLogic
    private void FixedUpdate()
    {
        if (movementVector != Vector2.zero)
        {
            int count = rb.Cast(
                movementVector,
                movementFilter,
                castCollisions,
                movementSpeed * Time.deltaTime + collisionOffset);

            if(count == 0)
            {
                if (isDashing)
                {
                    if (Vector2.Distance(dashStartVector, rb.position) <= dashDistance)
                    {
                        rb.MovePosition(rb.position + movementVector.normalized * dashSpeed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        isDashing = false;
                    }
                }
                else
                {
                    rb.MovePosition(rb.position + movementVector.normalized * movementSpeed * Time.fixedDeltaTime);
                }
            }
        }
        else
        {
            isDashing = false;
        }
    }

    public void OnWASD(InputValue inputValue)
    {
        movementVector = inputValue.Get<Vector2>();

        if (movementVector.y >= 0.1f)
        {
            animator.runtimeAnimatorController = backController;
        }
        else if (movementVector.y <= -0.1f)
        {
            animator.runtimeAnimatorController = frontController;
        }
        else if (movementVector.x <= -0.1f)
        {
            animator.runtimeAnimatorController = leftController;
        }
        else if (movementVector.x >= 0.1f)
        {
            animator.runtimeAnimatorController = rightController;
        }

        if (movementVector.x <= -0.1f || movementVector.x >= 0.1f || movementVector.y <= -0.1f || movementVector.y >= 0.1f)
        {
            animator.SetBool("isWalking", true);
        } else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void OnShift(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            dashStartVector = rb.position;
            isDashing = true;

            animator.SetTrigger("Dash");
        }
    }
    #endregion

    #region RoomTraversal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag.Equals("Door") || obj.tag.Equals("WestDoor")|| obj.tag.Equals("EastDoor") || obj.tag.Equals("NorthDoor")|| obj.tag.Equals("SouthDoor"))
        {

            Door door = obj.GetComponent<Door>();

            

            if (door.assignedDoor == null)
            {
                door.assignedDoor = roomManager.GetNextRoom(door.doorLocation);
            }


            switch (door.doorLocation)
            {
                case Door.DoorLocations.North:
                    print("North");
                    break;
                case Door.DoorLocations.South:
                    print("South");
                    break;
                case Door.DoorLocations.East:
                    print("East");
                    this.transform.position = door.assignedDoor.transform.position - new Vector3(-0.1f,0,0.2f);
                    break;
                case Door.DoorLocations.West:
                    print("West");
                    this.transform.position = door.assignedDoor.transform.position - new Vector3(1f, 0, 0.2f);
                    break;
            }
        }
    }
    #endregion
}
