using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> westDoors;
    public List<GameObject> eastDoors;

    private void Start()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("WestDoor");
        
        foreach(GameObject theDoor in doors)
        {
            westDoors.Add(theDoor);
        }

        doors = GameObject.FindGameObjectsWithTag("EastDoor");

        foreach (GameObject theDoor in doors)
        {
            eastDoors.Add(theDoor);
        }
    }

    /// <summary>
    /// Returns an available room to assign to a door
    /// </summary>
    public GameObject GetNextRoom(Door.DoorLocations orientation)
    {
        GameObject door;
        switch (orientation)
        {
            case Door.DoorLocations.North:
                break;
            case Door.DoorLocations.South:
                break;
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
}
