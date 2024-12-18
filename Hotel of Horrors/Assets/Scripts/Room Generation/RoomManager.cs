using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [SerializeField] int doorsToReset;
    [SerializeField] GameObject elevatorDoor;
    [SerializeField] GameObject mindRoomDoor;

    List<GameObject> northDoors = new List<GameObject>();
    List<GameObject> eastDoors = new List<GameObject>();
    List<GameObject> southDoors = new List<GameObject>();
    List<GameObject> westDoors = new List<GameObject>();

    ElevatorMenuManager elevator;

    string currentRoom = "";
    int roomsTraversed = 0;
    int enemiesToKill = 0;

    float mindRoomChance = 0;
    float elevatorRoomChance = 0;

    [SerializeField][Range(0,1)] float mindRoomPercentGain;
    [SerializeField] [Range(0, 1)] float elevatorRoomPercentGain;

    private void Start()
    {
        GetAllDoors();
        elevator = FindObjectOfType<ElevatorMenuManager>();
    }

    public void IncrementRoomsTraversed()
    {
        roomsTraversed++;
        print("rooms traversed " + roomsTraversed);
        mindRoomChance += mindRoomPercentGain;
        print("Mind room chance " + mindRoomChance);
        elevatorRoomChance += elevatorRoomPercentGain;

        if(roomsTraversed >= doorsToReset)
        {
            roomsTraversed = 0;
            elevator.ResetShop();
            GetAllDoors();
        }
    }

    /// <summary>
    /// Returns an available room to assign to a door
    /// </summary>
    public GameObject GetNextRoom(Door.DoorLocations orientation)
    {
        IncrementRoomsTraversed();
        float random = Random.value;
        //attempt to get either mind or elevator room
        if (mindRoomChance > random)
        {
            print("pitty mind room " + random);
            mindRoomChance = 0;
            return mindRoomDoor;
        }
        if (elevatorRoomChance > Random.value)
        {
            print("pitty elevator room");
            elevatorRoomChance = 0;
            return elevatorDoor;
        }

        GameObject door;
        switch (orientation)
        {
            case Door.DoorLocations.North:
                door = southDoors[Random.Range(0, southDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom.roomName;
                southDoors.Remove(door);
                return door;
            case Door.DoorLocations.South:
                door = northDoors[Random.Range(0, northDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom.roomName;
                northDoors.Remove(door);
                return door;
            case Door.DoorLocations.East:
                door = westDoors[Random.Range(0, westDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom.roomName;
                westDoors.Remove(door);
                return door;
            case Door.DoorLocations.West:
                door = eastDoors[Random.Range(0, eastDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom.roomName;
                eastDoors.Remove(door);
                return door;
        }
        return eastDoors[0];
    }

    void GetAllDoors()
    {
       /* for (int i = 0; i < northDoors.Count; i++)
        {
            Door door = northDoors[i].GetComponent<Door>();
            if (door) door.assignedDoor = null;
        }
        for (int i = 0; i < southDoors.Count; i++)
        {
            Door door = southDoors[i].GetComponent<Door>();
            if (door) door.assignedDoor = null;
        }
        for (int i = 0; i < westDoors.Count; i++)
        {
            Door door = westDoors[i].GetComponent<Door>();
            if (door) door.assignedDoor = null;
        }
        for (int i = 0; i < eastDoors.Count; i++)
        {
            Door door = eastDoors[i].GetComponent<Door>();
            if (door) door.assignedDoor = null;
        }*/

        northDoors.Clear();
        eastDoors.Clear();
        southDoors.Clear();
        westDoors.Clear();

        GameObject[] doors = GameObject.FindGameObjectsWithTag("WestDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.linkedDoor = null;
            westDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("EastDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.linkedDoor = null;
            eastDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("NorthDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.linkedDoor = null;
            northDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("SouthDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.linkedDoor = null;
            southDoors.Add(theDoor);
        }
    }

    public void AddEnemy()
    {
        if(enemiesToKill <= 0)
        {
            enemiesToKill = 0;

            Door[] allDoors = FindObjectsOfType<Door>();

            foreach (Door d in allDoors)
            {
                d.locked = true;
            }
            
        }

        enemiesToKill++;
    }

    public void KillEnemy()
    {
        enemiesToKill--;
        
        if(enemiesToKill <= 0)
        {
            Door[] allDoors = FindObjectsOfType<Door>();

            foreach (Door d in allDoors)
            {
                d.locked = false;
            }
        }
    }

    public string GetCurrentRoom()
    {
        return currentRoom;
    }
}
