using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [SerializeField] int doorsToReset;


    List<GameObject> northDoors = new List<GameObject>();
    List<GameObject> eastDoors = new List<GameObject>();
    List<GameObject> southDoors = new List<GameObject>();
    List<GameObject> westDoors = new List<GameObject>();

    string currentRoom = "";
    int roomsTraversed = 0;
    int enemiesToKill = 0;
    

    private void Start()
    {
        GetAllDoors();
    }

    public void IncrementRoomsTraversed()
    {
        roomsTraversed++;
        if(roomsTraversed >= doorsToReset)
        {
            roomsTraversed = 0;
            GetAllDoors();
        }
    }

    /// <summary>
    /// Returns an available room to assign to a door
    /// </summary>
    public GameObject GetNextRoom(Door.DoorLocations orientation)
    {
        IncrementRoomsTraversed();

        GameObject door;
        switch (orientation)
        {
            case Door.DoorLocations.North:
                door = southDoors[Random.Range(0, southDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom;
                southDoors.Remove(door);
                return door;
            case Door.DoorLocations.South:
                door = northDoors[Random.Range(0, northDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom;
                northDoors.Remove(door);
                return door;
            case Door.DoorLocations.East:
                door = westDoors[Random.Range(0, westDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom;
                westDoors.Remove(door);
                return door;
            case Door.DoorLocations.West:
                door = eastDoors[Random.Range(0, eastDoors.Count)];
                currentRoom = door.GetComponent<Door>().associatedRoom;
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
            if (door) door.assignedDoor = null;
            westDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("EastDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.assignedDoor = null;
            eastDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("NorthDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.assignedDoor = null;
            northDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("SouthDoor");

        foreach (GameObject theDoor in doors)
        {
            Door door = theDoor.GetComponent<Door>();
            if (door) door.assignedDoor = null;
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
