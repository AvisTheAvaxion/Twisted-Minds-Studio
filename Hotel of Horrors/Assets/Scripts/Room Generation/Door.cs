using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    public bool locked = false;
    public string associatedRoom;

    public enum DoorLocations
    {
        North, East, South, West, Special
    }

    public DoorLocations doorLocation;
    private RoomManager roomManager;
    [HideInInspector]public GameObject assignedDoor;

    private void Start()
    {
        roomManager = GameObject.Find("Room Manager").GetComponent<RoomManager>();
    }

    
}
