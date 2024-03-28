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

    int roomsTraversed = 0;
    

    private void Start()
    {
        GetAllDoors();
    }

    /// <summary>
    /// Returns an available room to assign to a door
    /// </summary>
    public GameObject GetNextRoom(Door.DoorLocations orientation)
    {
        if(roomsTraversed++ >= doorsToReset)
        {
            roomsTraversed = 0;
            GetAllDoors();
        }

        GameObject door;
        switch (orientation)
        {
            case Door.DoorLocations.North:
                door = southDoors[Random.Range(0, southDoors.Count)];
                southDoors.Remove(door);
                return door;
            case Door.DoorLocations.South:
                door = northDoors[Random.Range(0, northDoors.Count)];
                northDoors.Remove(door);
                return door;
            case Door.DoorLocations.East:
                door = westDoors[Random.Range(0, westDoors.Count)];
                westDoors.Remove(door);
                return door;
            case Door.DoorLocations.West:
                door = eastDoors[Random.Range(0, eastDoors.Count)];
                eastDoors.Remove(door);
                return door;
        }
        return eastDoors[0];
    }

    void GetAllDoors()
    {
        northDoors.Clear();
        eastDoors.Clear();
        southDoors.Clear();
        westDoors.Clear();

        GameObject[] doors = GameObject.FindGameObjectsWithTag("WestDoor");

        foreach (GameObject theDoor in doors)
        {
            westDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("EastDoor");

        foreach (GameObject theDoor in doors)
        {
            eastDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("NorthDoor");

        foreach (GameObject theDoor in doors)
        {
            northDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("SouthDoor");

        foreach (GameObject theDoor in doors)
        {
            southDoors.Add(theDoor);
        }
    }

}
