using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> availableRooms;

    private void Start()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        
        foreach(GameObject room in rooms)
        {
            print(room.name);
            availableRooms.Add(room);
        }
        availableRooms.Remove(GameObject.Find("Test Room 1"));

    }
    /// <summary>
    /// Returns an available room to assign to a door
    /// </summary>

    public GameObject GetNextRoom()
    {
        return availableRooms[Random.Range(0, availableRooms.Count)];

    }
}
