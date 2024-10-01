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
    public Transform spawnLocation;
    [HideInInspector] public Door linkedDoor;

    [SerializeField] float doorTransitionLength = 0.5f;
    [SerializeField] Image fadeImage;

    [Header("Locked Gate Settings")]
    [SerializeField] SpriteRenderer lockedGateRend;
    [SerializeField] int frameRate = 10;
    [SerializeField] Sprite[] lockedGateFrames;

    public enum DoorLocations
    {
        North, East, South, West, Special
    }

    public DoorLocations doorLocation;
    DoorLocations targetOrientation;

    private void Start()
    {
        floor = FindObjectOfType<Floor>();
        spawnLocation = transform.GetChild(0).transform;
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
            if (!locked)
            {
                if (elevatorDoor)
                {
                    //take player to elevator room menu without moving them
                    StartCoroutine(GoToElevator(player));
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

        //generates a potential elevator room spawn
        floor.IncreaseElevatorChance();
        linkedDoor.associatedRoom.MakeElevatorDoor();

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

    IEnumerator GoToElevator(GameObject player)
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
        
        floor.elevatorCanvas.SetActive(true);
        Cursor.visible = true;

        //fade to clear
        for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }
    }

    public void LockGate(bool close)
    {
        StartCoroutine(AnimateLockedGate(close));
    }
    IEnumerator AnimateLockedGate(bool close)
    {
        float frameLength = 1f / frameRate;
        WaitForSeconds wait = new WaitForSeconds(frameLength);

        lockedGateRend.enabled = true;

        for (int f = 0; f < lockedGateFrames.Length; f++)
        {
            lockedGateRend.sprite = close ? lockedGateFrames[f] : lockedGateFrames[lockedGateFrames.Length - 1 - f];
            yield return wait;
        }


        lockedGateRend.sprite = close ? lockedGateFrames[lockedGateFrames.Length - 1] : lockedGateFrames[0];

        yield return wait;

        lockedGateRend.enabled = close;
    }
}
