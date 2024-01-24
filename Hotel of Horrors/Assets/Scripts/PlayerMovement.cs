using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Object References")]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    private Transform playerTrans;

    [Header("Movement Modifiers")]
    public float movementSpeed;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    Vector2 movementVector = new Vector2();
    RoomManager roomManager;

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = player.transform;
        roomManager = GameObject.Find("Room Manager").GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(movementVector != Vector2.zero)
        {
            int count = rb.Cast(
                movementVector,
                movementFilter,
                castCollisions,
                movementSpeed * Time.deltaTime + collisionOffset);

            if(count == 0)
            {
                rb.MovePosition(rb.position + movementVector.normalized * movementSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void OnWASD(InputValue inputValue)
    {
        movementVector = inputValue.Get<Vector2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag.Equals("Door"))
        {

            Door door = obj.GetComponent<Door>();

            

            if (door.assignedRoom == null)
            {
                door.assignedRoom = roomManager.GetNextRoom();
                roomManager.availableRooms.RemoveAt(roomManager.availableRooms.IndexOf(door.assignedRoom));
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
                    this.transform.position = door.assignedRoom.GetComponent<Room>().WestDoor.position - new Vector3(-0.1f,0,0);
                    break;
                case Door.DoorLocations.West:
                    print("West");
                    this.transform.position = door.assignedRoom.GetComponent<Room>().EastDoor.position - new Vector3(1f, 0, 0);
                    break;
            }
        }
        
    }
}
