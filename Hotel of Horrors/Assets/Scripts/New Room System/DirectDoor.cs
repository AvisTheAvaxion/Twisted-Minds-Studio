using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DirectDoor : MonoBehaviour
{
    Floor floor;
    public bool sceneTransition = false;
    public int sceneToLoad = 0;
    public bool locked = false;
    public Room associatedRoom;
    public Transform spawnLocation;

    [SerializeField] bool lockAfterEntering;
    [SerializeField] Animator animator;
    [SerializeField] Animator lockedGateAnimator;
    [SerializeField] float doorTransitionLength = 0.5f;
    [SerializeField] Image fadeImage;
    [SerializeField] bool canUnlockAfterLocking = true;

    QuestSystem questSystem;
    SerializationManager serialization;

    public enum DoorOrientations
    {
        North, East, South, West, Special
    }

    public DoorOrientations doorLocation;
    DoorOrientations targetOrientation;
    DirectDoor correspondingDoor;
    Room correspondingRoom;

    private void Start()
    {
        questSystem = FindObjectOfType<QuestSystem>();
        serialization = FindObjectOfType<SerializationManager>();
        floor = FindObjectOfType<Floor>();
        associatedRoom = GetComponentInParent<Room>();

        animator = GetComponent<Animator>();

        if (locked) LockDoor();
        else UnlockDoor();

        if (spawnLocation)
        {
            correspondingDoor = spawnLocation.parent.GetComponent<DirectDoor>();
            if(correspondingDoor)
                correspondingRoom = correspondingDoor.transform.parent.GetComponent<Room>();
        }
        
        if (fadeImage == null)
            fadeImage = GameObject.Find("Fade to Black Image").GetComponent<Image>();

        switch (doorLocation)
        {
            case DoorOrientations.North:
                targetOrientation = DoorOrientations.South;
                break;
            case DoorOrientations.East:
                targetOrientation = DoorOrientations.West;
                break;
            case DoorOrientations.South:
                targetOrientation = DoorOrientations.North;
                break;
            case DoorOrientations.West:
                targetOrientation = DoorOrientations.East;
                break;
            default:
                targetOrientation = DoorOrientations.North;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!locked && collision.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(TeleportPlayer(collision.gameObject));
            if (!sceneTransition && correspondingRoom != null)
            {
                if(lockAfterEntering && correspondingDoor)
                {
                    correspondingDoor.LockDoor();
                }
                floor.SetCurrentRoom(correspondingRoom.roomName);
            }
        }
    }

    IEnumerator TeleportPlayer(GameObject player)
    {
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Animator>().SetBool("isWalking", false);

        player.GetComponent<PlayerMovement>().canMove = false;
        //print("fading black");
        //player.GetComponent<NewAudioManager>().PlayEffect("DoorOpen");

        // loop over 1 second - fade to black
        for (float i = 0; i <= doorTransitionLength / 2f; i += Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }

        //takes the player to the next door
        if (sceneTransition)
        {
            questSystem.NextFloor();
            serialization.SaveData();
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            player.transform.position = spawnLocation.position;
        }

        // loop over 1 second backwards - fade to clear
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

    public void LockDoor()
    {
        locked = true;
        if (lockedGateAnimator)
        {
            lockedGateAnimator.gameObject.SetActive(true);
            lockedGateAnimator.SetBool("Close", true);
        }
        if (animator)
        {
            animator.SetBool("Locked", true);
        }
    }
    public void UnlockDoor()
    {
        if (locked && !canUnlockAfterLocking) return;

        locked = false;
        if (lockedGateAnimator)
            lockedGateAnimator.SetBool("Close", false);

        if (animator)
        {
            animator.SetBool("Locked", false); ;
        }
    }
}
