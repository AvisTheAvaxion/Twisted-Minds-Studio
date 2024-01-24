using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorLocations
    {
        North, East, South, West
    }

    public DoorLocations doorLocation;
    private RoomManager roomManager;
    [HideInInspector]public GameObject assignedRoom;

    private void Start()
    {
        roomManager = GameObject.Find("Room Manager").GetComponent<RoomManager>();
    }

    
}
