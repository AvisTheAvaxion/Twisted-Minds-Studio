using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    Room[] allRooms;
    PlayerInventory playerInventoryRef;
    QuestSystem questSys;
    Dictionary<string, Room> roomsByName = new Dictionary<string, Room>();
    Dictionary<string, List<Room>> roomsByCategory = new Dictionary<string, List<Room>>();
    int emotionalEnergyGained;

    [SerializeField] int traversalsForReset = 5;
    int roomTraversals = 0;

    //chance gained to spawn each room for every EE picked up
    [SerializeField] float mediumChanceGained = 0.02f;
    [SerializeField] float hardChanceGained = 0.01f;

    //chance gained every time the player enters a new room
    [SerializeField] float elevatorChanceGained = 10f;
    [SerializeField] float mindRoomChanceGained = 7.5f;
    [SerializeField] float hallwayChanceGained = 5f;

    //starting chances for each room type to spawn
    [SerializeField] float mediumChance = 15f;
    [SerializeField] float hardChance = 5f;
    [SerializeField] float elevatorChance = 0f;
    [SerializeField] float mindRoomChance = 0f;
    [SerializeField] float hallwayChance = 0f;

    string currentRoom;
    int enemiesToKill = 0;

    [SerializeField]ElevatorMenuManager elevator;
    public GameObject elevatorCanvas;
    [SerializeField] bool debug;

    System.Random prng;

    private void Start()
    {
        allRooms = FindObjectsOfType<Room>();

        roomsByCategory.Add("Peaceful", new List<Room>());
        roomsByCategory.Add("Easy", new List<Room>());
        roomsByCategory.Add("Medium", new List<Room>());
        roomsByCategory.Add("Hard", new List<Room>());
        roomsByCategory.Add("Mind Room", new List<Room>());

        elevator = FindObjectOfType<ElevatorMenuManager>();
        

        foreach (Room room in allRooms)
        {
            roomsByName.Add(room.roomName, room);

            switch (room.category)
            {
                case Room.roomCategories.peaceful:
                    roomsByCategory["Peaceful"].Add(room);
                    break;
                case Room.roomCategories.easy:
                    roomsByCategory["Easy"].Add(room);
                    break;
                case Room.roomCategories.medium:
                    roomsByCategory["Medium"].Add(room);
                    break;
                case Room.roomCategories.hard:
                    roomsByCategory["Hard"].Add(room);
                    break;
                case Room.roomCategories.mindRoom:
                    roomsByCategory["Mind Room"].Add(room);
                    break;
            }
        }

        prng = new System.Random(Random.Range(int.MinValue, int.MaxValue));

        playerInventoryRef = FindObjectOfType<PlayerInventory>();
        questSys = FindObjectOfType<QuestSystem>();

        //waiting to set the EE gains because it takes some time for the inventory to load
        StartCoroutine(WaitToAddEE());
    }

    public void SetEEGains(int amount)
    {
        emotionalEnergyGained = amount;

        mediumChance += (mediumChanceGained * amount);
        hardChance += (hardChanceGained * amount);
    }

    public void AddEmotionalEnergy(int amountGained)
    {
        emotionalEnergyGained += amountGained;

        mediumChance += (mediumChanceGained * amountGained);
        hardChance += (hardChanceGained * amountGained);
    }

    IEnumerator WaitToAddEE()
    {
        yield return new WaitForSeconds(0.5f);

        mediumChance = 15f;
        hardChance = 5f;
        elevatorChance = 0f;
        mindRoomChance = 0f;

        emotionalEnergyGained = 0;
        SetEEGains(playerInventoryRef.emotionalEnergyGained);
        elevator.ResetShop();
    }

    /// <summary>
    /// Runs the logic for finding the next room to take the player
    /// </summary>
    /// <returns>The next room's spawn location to teleport the player to</returns>
    public Door getDoorLink(Door.DoorLocations targetOrientation)
    {
        Door spawnDoor;
        float difficultyRoll = Random.Range(0, 100);
        List<Room> possibleRooms = new List<Room>();

        roomTraversals++;

        mindRoomChance += mindRoomChanceGained;
        hallwayChance += hallwayChanceGained;

        if(roomTraversals >= traversalsForReset)
        {
            ResetDoorLinks();
            roomTraversals = 0;
        }

        if (mindRoomChance > Random.Range(0, 100f))
        {
            possibleRooms.Clear();
            possibleRooms.Add(roomsByName["Mind Room"]);
            mindRoomChance = 0;

            if (debug) print("Travelling to Mind Room");
        }
        else if (hallwayChance > Random.Range(0,100f))
        {
            possibleRooms.Clear();
            possibleRooms.Add(roomsByName["Hallway"]);
            hallwayChance = 0;

            if (debug) print("Travelling to Hallway");
        }
        else if (hardChance > difficultyRoll)
        {
            possibleRooms = roomsByCategory["Hard"];

            if (debug) print("Travelling to Hard Room");
        }
        else if (mediumChance > difficultyRoll)
        {
            possibleRooms = roomsByCategory["Medium"];

            if (debug) print("Travelling to Medium Room");
        }
        else
        {
            possibleRooms = roomsByCategory["Easy"];

            if (debug) print("Travelling to Easy Room");
        }

        int findAttempts = 0;
        do
        {
            findAttempts++;
            print("possible rooms: " + possibleRooms.Count);
            
            //if there are no rooms left of the desired difficulty
            if(possibleRooms.Count < 1 || findAttempts > 100)
            {
                possibleRooms.Clear();

                foreach (Room room in allRooms)
                    possibleRooms.Add(room);
            }

            int next = prng.Next();
            int i = next % possibleRooms.Count;
            if (debug) print("Chosen room index: " + i);
            spawnDoor = possibleRooms[i].GetTargetDoor(targetOrientation);

        } while (spawnDoor == null);

        spawnDoor.associatedRoom.doorsAvailable.Remove(spawnDoor);
        currentRoom = spawnDoor.associatedRoom.roomName;
        //questSys.QuestEvent(QuestSystem.QuestEventType.RoomEnter, currentRoom);

        return spawnDoor;
    }

    /// <summary>
    /// Resets all the doors in each room to the default state with no links and makes them all available again.
    /// Also Changes the shop's inventory
    /// </summary>
    void ResetDoorLinks()
    {
        foreach(Room room in allRooms)
        {
            if(room.enemySpawner != null)
                room.enemySpawner.ResetSpawner();

            foreach(Door door in room.doors)
            {
                door.linkedDoor = null;
            }

            room.doorsAvailable.Clear();

            foreach (Door door in room.doors)
            {
                room.doorsAvailable.Add(door);
            }
        }

        elevator.ResetShop();
    }

    public void AddEnemy()
    {
        if (enemiesToKill <= 0)
        {
            enemiesToKill = 0;

            Door[] allDoors = FindObjectsOfType<Door>();

            foreach (Door d in allDoors)
            {
                d.locked = true;
                d.LockGate(d.locked);
            }

        }

        enemiesToKill++;
    }

    public void KillEnemy()
    {
        enemiesToKill--;

        if (enemiesToKill <= 0)
        {
            Door[] allDoors = FindObjectsOfType<Door>();

            foreach (Door d in allDoors)
            {
                d.locked = false;
                d.LockGate(d.locked);
            }
        }
    }

    public float GetElevatorChance()
    {
        return elevatorChance;
    }
    
    public void IncreaseElevatorChance()
    {
        elevatorChance += elevatorChanceGained;
    }

    public void ClearElevatorChance()
    {
        elevatorChance = 0;
    }
}
