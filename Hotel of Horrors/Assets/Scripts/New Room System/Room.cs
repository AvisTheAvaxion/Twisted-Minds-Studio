using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string roomName = "Nameless Room";
    public roomCategories category;
    public enemyModifiers currentModifier;
    public Door[] doors;
    public List<Door> doorsAvailable = new List<Door>();
    Floor currentFloor;

    public EnemySpawner enemySpawner { get; private set; }

    public enum roomCategories
    {
        peaceful,
        easy, 
        medium, 
        hard,
        mindRoom
    }
    public enum enemyModifiers
    {
        none,
        increasedHealth, 
        increasedDamage, 
        moreEnemies, 
        increasedMovementSpeed, 
        increasedAttackSpeed
    }

    private void Start()
    {
        enemySpawner = this.gameObject.GetComponentInChildren<EnemySpawner>();
        doors = GetComponentsInChildren<Door>();
        currentFloor = GetComponentInParent<Floor>();

        foreach(Door door in doors)
        {
            doorsAvailable.Add(door);
        }
    }

    public Door GetTargetDoor(Door.DoorLocations targetLocation) 
    {
        foreach (Door door in doorsAvailable)
        {
            if (door.doorLocation.Equals(targetLocation))
            {
                return door;
            }
        }
        return null;
    }

    public void MakeElevatorDoor()
    {
        if(currentFloor.GetElevatorChance() > Random.Range(0, 100))
        {
            print(doorsAvailable.Count + " doors available for elevator");
            doorsAvailable[Random.Range(0, doorsAvailable.Count)].elevatorDoor = true;
            currentFloor.ClearElevatorChance();
        }
    }
}
