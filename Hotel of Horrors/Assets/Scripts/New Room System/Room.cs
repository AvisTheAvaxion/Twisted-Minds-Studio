using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public struct RoomModifierWeight
    {
        public float weight;
        public RoomModifiers modifier;

        public RoomModifierWeight(float weightIn, RoomModifiers modifierIn)
        {
            weight = weightIn;
            modifier = modifierIn;
        }
    }

    public string roomName = "Nameless Room";
    public RoomCategories category = RoomCategories.easy;
    public RoomModifiers currentModifier = RoomModifiers.none;
    [SerializeField] RoomModifierWeight[] roomModifierWeights = { 
        new RoomModifierWeight(8, RoomModifiers.none), 
        new RoomModifierWeight(1, RoomModifiers.increasedHealth),
        new RoomModifierWeight(1, RoomModifiers.increasedDamage),
        new RoomModifierWeight(1, RoomModifiers.increasedAttackSpeed),
        new RoomModifierWeight(1, RoomModifiers.increasedMovementSpeed),
        new RoomModifierWeight(1, RoomModifiers.moreEnemies)
    };

    [HideInInspector]
    public Door[] doors;
    [HideInInspector]
    public List<Door> doorsAvailable = new List<Door>();

    Floor currentFloor;

    public EnemySpawner enemySpawner { get; private set; }

    public enum RoomCategories
    {
        peaceful,
        easy, 
        medium, 
        hard,
        mindRoom
    }
    public enum RoomModifiers
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
        List<Door> otherDoors = new List<Door>();
        foreach (Door door in doorsAvailable)
        {
            if (door.doorLocation.Equals(targetLocation))
            {
                return door;
            }
            else if (!(targetLocation == Door.DoorLocations.North && door.doorLocation == Door.DoorLocations.South) &&
                !(targetLocation == Door.DoorLocations.South && door.doorLocation == Door.DoorLocations.North) &&
                !(targetLocation == Door.DoorLocations.East && door.doorLocation == Door.DoorLocations.West) &&
                !(targetLocation == Door.DoorLocations.West && door.doorLocation == Door.DoorLocations.East))
            {
                otherDoors.Add(door);
            }
        }
        return otherDoors.Count > 0 ? otherDoors[Random.Range(0, otherDoors.Count)] : null;
    }

    public void MakeElevatorDoor()
    {
        if(currentFloor.GetElevatorChance() > Random.Range(0, 100))
        {
            print(doorsAvailable.Count + " doors available for elevator");
            doorsAvailable[Random.Range(0, doorsAvailable.Count)].SetElevatorDoor();
            currentFloor.ClearElevatorChance();
        }
    }

    public void ChooseRoomModifier()
    {
        if (category == RoomCategories.easy || category == RoomCategories.medium || category == RoomCategories.hard)
        {
            float weightTotal = 0;
            for (int i = 0; i < roomModifierWeights.Length; i++)
            {
                if (roomModifierWeights[i].modifier != RoomModifiers.none)
                {
                    switch (category)
                    {
                        case RoomCategories.easy:
                            weightTotal += roomModifierWeights[i].weight;
                            break;
                        case RoomCategories.medium:
                            weightTotal += roomModifierWeights[i].weight * 1.25f;
                            break;
                        case RoomCategories.hard:
                            weightTotal += roomModifierWeights[i].weight * 1.5f;
                            break;
                    }
                } 
                else
                {
                    weightTotal += roomModifierWeights[i].weight;
                }
            }

            float rand = Random.Range(0, weightTotal);
            print($"{roomName}: {rand}");
            int e = 0;
            for (; e < roomModifierWeights.Length; e++)
            {
                float currentWeight = roomModifierWeights[e].weight;
                if (roomModifierWeights[e].modifier != RoomModifiers.none)
                {
                    switch (category)
                    {
                        case RoomCategories.medium:
                            currentWeight = roomModifierWeights[e].weight * 1.25f;
                            break;
                        case RoomCategories.hard:
                            currentWeight = roomModifierWeights[e].weight * 1.5f;
                            break;
                    }
                }

                if (rand < currentWeight)
                    break;
                rand -= currentWeight;
            }
            if (e < roomModifierWeights.Length)
            {
                currentModifier = roomModifierWeights[e].modifier;
            }
        }
    }
}
