using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    Floor floor;
    public bool locked = false;
    public bool elevatorDoor = false;
    public Room associatedRoom;
    [HideInInspector] public Transform spawnLocation;
    [HideInInspector] public Door linkedDoor;

    [SerializeField] float doorTransitionLength = 0.5f;
    [SerializeField] Image fadeImage;


    public enum DoorLocations
    {
        North, East, South, West, Special
    }

    public DoorLocations doorLocation;
    DoorLocations targetOrientation;

    private void Start()
    {
        floor = FindObjectOfType<Floor>();
        spawnLocation = GetComponentInChildren<Transform>();
        associatedRoom = GetComponentInParent<Room>();
        fadeImage = GameObject.Find("Fade to Black Image").GetComponent<Image>();

        switch (doorLocation)
        {
            case DoorLocations.North:
                targetOrientation = DoorLocations.South;
                break;
            case DoorLocations.East:
                targetOrientation = DoorLocations.West;
                break;
            case DoorLocations.South:
                targetOrientation = DoorLocations.North;
                break;
            case DoorLocations.West:
                targetOrientation = DoorLocations.East;
                break;
            default:
                targetOrientation = DoorLocations.North;
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag.Equals("Player"))
        {
            GameObject player = collision.gameObject;
            if ((player.GetComponent<PlayerMovement>().Direction.Equals("North") && doorLocation.Equals(DoorLocations.North)) ||
                (player.GetComponent<PlayerMovement>().Direction.Equals("East") && doorLocation.Equals(DoorLocations.East)) ||
                (player.GetComponent<PlayerMovement>().Direction.Equals("South") && doorLocation.Equals(DoorLocations.South)) ||
                (player.GetComponent<PlayerMovement>().Direction.Equals("West") && doorLocation.Equals(DoorLocations.West)))
            {
                
                if (elevatorDoor)
                {
                    //take player to elevator room menu without moving them

                }
                else
                {
                    //if the door already does not have a link, give it one 
                    if (linkedDoor == null)
                    {
                        //make the floor run the logic for getting the next room
                        linkedDoor = floor.getDoorLink(targetOrientation);
                    }

                    StartCoroutine(TeleportPlayer(player));
                }
            }
        }
    }

    IEnumerator TeleportPlayer(GameObject player)
    {
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Animator>().SetBool("isWalking", false);

        player.GetComponent<PlayerMovement>().canMove = false;

        //player.GetComponent<NewAudioManager>().PlayEffect("DoorOpen");

        //fade to black
        for (float i = 0; i <= doorTransitionLength / 2f; i += Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }

        //takes the player to the next door
        player.transform.position = linkedDoor.spawnLocation.position;

        //fade to clear
        for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }
        player.GetComponent<PlayerMovement>().canMove = true;

        if (player.GetComponent<PlayerMovement>().MovementVector.x <= -0.1f || player.GetComponent<PlayerMovement>().MovementVector.x >= 0.1f || player.GetComponent<PlayerMovement>().MovementVector.y <= -0.1f || player.GetComponent<PlayerMovement>().MovementVector.y >= 0.1f)
            player.GetComponent<Animator>().SetBool("isWalking", true);
        else
            player.GetComponent<Animator>().SetBool("isWalking", false);
    }
}
