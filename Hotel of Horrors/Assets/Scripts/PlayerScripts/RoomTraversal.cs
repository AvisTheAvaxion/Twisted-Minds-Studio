using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class RoomTraversal : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] RuntimeAnimatorController forwardController;
    [SerializeField] AnimatorOverrideController backController;
    [SerializeField] AnimatorOverrideController leftController;
    [SerializeField] AnimatorOverrideController rightController;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Image fadeImage;
    [SerializeField] GameObject elevatorCanvas;
    [SerializeField] NewAudioManager audioManager;

    RoomManager roomManager;
    QuestSystem questSys;

    [SerializeField] float doorTransitionLength = 0.5f;

    private void Awake()
    {
        questSys = FindObjectOfType<QuestSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = this.gameObject;
        if(rb == null)
            rb = player.GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = player.GetComponent<Animator>();

        roomManager = GameObject.Find("Room Manager")?.GetComponent<RoomManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if ((obj.tag.Equals("Door") || obj.tag.Equals("WestDoor") || obj.tag.Equals("EastDoor") || obj.tag.Equals("NorthDoor") || obj.tag.Equals("SouthDoor")) && !obj.GetComponent<Door>().locked)
        { 
            StartCoroutine(FadeImageOut(obj));
        }
    }


    IEnumerator FadeImageOut(GameObject obj)
    {
        Door door = obj.GetComponent<Door>();

        //Ensure player is facing the door
        /*if ((door.doorLocation == Door.DoorLocations.North && direction.Equals("North")) ||
            (door.doorLocation == Door.DoorLocations.South && direction.Equals("South")) ||
            (door.doorLocation == Door.DoorLocations.East && direction.Equals("East")) ||
            (door.doorLocation == Door.DoorLocations.West && direction.Equals("West")) ||
            (door.doorLocation == Door.DoorLocations.Special))
        {*/
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        playerMovement.canMove = false;
        //print("fading black");
        audioManager.PlayEffect("DoorOpen");
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
        else
        {
            roomManager.IncrementRoomsTraversed();
            if (door.assignedDoor == null)
            {
                door.assignedDoor = roomManager.GetNextRoom(door.doorLocation);
            }
        }


        switch (door.doorLocation)
        {
            case Door.DoorLocations.North:
                print("North");
                this.transform.position = door.assignedDoor.transform.position - new Vector3(0f, -0.6f, 0);
                playerMovement.SetDirection("North");
                animator.runtimeAnimatorController = backController;
                break;
            case Door.DoorLocations.South:
                print("South");
                this.transform.position = door.assignedDoor.transform.position - new Vector3(0, 0.6f, 0);
                playerMovement.SetDirection("South");
                animator.runtimeAnimatorController = forwardController;
                break;
            case Door.DoorLocations.East:
                print("East");
                this.transform.position = door.assignedDoor.transform.position - new Vector3(-0.6f, 0, 0);
                playerMovement.SetDirection("East");
                animator.runtimeAnimatorController = rightController;
                break;
            case Door.DoorLocations.West:
                print("West");
                this.transform.position = door.assignedDoor.transform.position - new Vector3(0.6f, 0, 0);
                playerMovement.SetDirection("West");
                animator.runtimeAnimatorController = leftController;
                break;
            case Door.DoorLocations.Special:
                print("special");
                this.transform.position = GetSpecialRoom(door);
                playerMovement.SetDirection("North");
                animator.runtimeAnimatorController = leftController;
                break;
        }

        print(roomManager.GetCurrentRoom());
        questSys.QuestEvent(QuestSystem.QuestEventType.RoomEnter, roomManager.GetCurrentRoom());

        //print("fading clear");
        // loop over 1 second backwards - fade to clear
        for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }
        playerMovement.canMove = true;

        if (playerMovement.MovementVector.x <= -0.1f || playerMovement.MovementVector.x >= 0.1f || playerMovement.MovementVector.y <= -0.1f || playerMovement.MovementVector.y >= 0.1f)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
        //}
    }

    Vector3 GetSpecialRoom(Door door)
    {
        switch (door.gameObject.name)
        {
            case "Elevator Room Door":
                print("Going to elevator");
                elevatorCanvas.SetActive(true);
                Cursor.visible = true;
                return new Vector3(-4.3f, -48.5f, 0f);
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
}
