using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public static int maxFloorUnlocked = 1;
    public static int maxFloorTraveledTo = 1;
    public static int currentFloor { get; private set; }

    Room[] allRooms;
    PlayerInventory playerInventoryRef;
    QuestSystem questSys;
    Dictionary<string, Room> roomsByName = new Dictionary<string, Room>();
    Dictionary<string, List<Room>> roomsByCategory = new Dictionary<string, List<Room>>();
    int emotionalEnergyGained;

    [SerializeField] int floorNumber = 1;

    [SerializeField] int traversalsForReset = 5;
    int roomTraversals = 0;

    //chance gained to spawn each room for every EE picked up
    [SerializeField] float mediumChanceGained = 0.02f;
    [SerializeField] float hardChanceGained = 0.01f;

    //chance gained every time the player enters a new room
    [SerializeField] float elevatorChanceGained = 10f;
    [SerializeField] float mindRoomChanceGained = 7.5f;
    [SerializeField] float hallwayChanceGained = 5f;
    [SerializeField] float peacefulRoomChanceGained = 5f;

    //starting chances for each room type to spawn
    [SerializeField] float mediumChance = 15f;
    [SerializeField] float hardChance = 5f;
    [SerializeField] float elevatorChance = 0f;
    [SerializeField] float mindRoomChance = 0f;
    [SerializeField] float hallwayChance = 0f;
    [SerializeField] float peacefulRoomChance = 0f;
    [SerializeField] float guaranteeRoomChance = 80;

    [Header("Boss Room")]
    [SerializeField] DirectDoor toBossDoor;
    [SerializeField] DirectDoor fromBossDoor;

    public float MediumChance { get => mediumChance; }
    public float HardChance { get => hardChance; }

    string currentRoom;
    int enemiesToKill = 0;

    Queue<Room> roomGuarantees = new Queue<Room>();

    [SerializeField]ElevatorMenuManager elevator;
    public GameObject elevatorCanvas;
    [SerializeField] bool debug;

    System.Random prng;

    private void Start()
    {
        allRooms = FindObjectsOfType<Room>();

        currentFloor = floorNumber;
        if (currentFloor > maxFloorTraveledTo) maxFloorTraveledTo = currentFloor;

        if (toBossDoor && fromBossDoor)
        {
            toBossDoor.LockDoor();
            fromBossDoor.LockDoor();
        }

        roomsByCategory.Add("Peaceful", new List<Room>());
        roomsByCategory.Add("Easy", new List<Room>());
        roomsByCategory.Add("Medium", new List<Room>());
        roomsByCategory.Add("Hard", new List<Room>());
        roomsByCategory.Add("Mind Room", new List<Room>());
        roomsByCategory.Add("Hallway", new List<Room>());

        elevator = FindObjectOfType<ElevatorMenuManager>();

        Random.InitState((int)System.DateTime.Now.Ticks);

        foreach (Room room in allRooms)
        {
            roomsByName.Add(room.roomName, room);

            switch (room.category)
            {
                case Room.RoomCategories.peaceful:
                    roomsByCategory["Peaceful"].Add(room);
                    break;
                case Room.RoomCategories.easy:
                    roomsByCategory["Easy"].Add(room);
                    break;
                case Room.RoomCategories.medium:
                    roomsByCategory["Medium"].Add(room);
                    break;
                case Room.RoomCategories.hard:
                    roomsByCategory["Hard"].Add(room);
                    break;
                case Room.RoomCategories.mindRoom:
                    roomsByCategory["Mind Room"].Add(room);
                    break;
                case Room.RoomCategories.hallway:
                    roomsByCategory["Hallway"].Add(room);
                    break;
            }

            room.ChooseRoomModifier();
            if(room.enemySpawner != null)
                room.enemySpawner.ApplyRoomModifier(room.currentModifier);
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
        if(elevator != null)
            elevator.ResetShop();
    }

    /// <summary>
    /// Runs the logic for finding the next room to take the player
    /// </summary>
    /// <returns>The next room's spawn location to teleport the player to</returns>
    public Door getDoorLink(Door.DoorLocations targetOrientation)
    {
        Door doorLink = null;
        float difficultyRoll = Random.Range(0f, 100f);
        print("Difficulty Roll: " + difficultyRoll);
        List<Room> possibleRooms = new List<Room>();


       /* if(roomTraversals >= traversalsForReset)
        {
            ResetDoorLinks();
            roomTraversals = 0;
        }*/

        if(roomGuarantees.Count > 0 && roomGuarantees.Peek().CheckAvailableDoors(targetOrientation) && guaranteeRoomChance > Random.Range(0, 100f))
        {
            possibleRooms.Clear();
            
            possibleRooms.Add(roomGuarantees.Dequeue());

            if (debug) print("Travelling to Guaranteed Room");
        }
        else if (mindRoomChance > Random.Range(0, 100f))
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
        else if(peacefulRoomChance > Random.Range(0, 100f))
        {
            possibleRooms.Clear();
            possibleRooms = roomsByCategory["Peaceful"];
            peacefulRoomChance = 0;

            if (debug) print("Travelling to Peaceful Room");
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

        mindRoomChance += mindRoomChanceGained;
        hallwayChance += hallwayChanceGained;
        peacefulRoomChance += peacefulRoomChanceGained;

        int findAttempts = 0;
        do
        {
            findAttempts++;
            //print("possible rooms: " + possibleRooms.Count);
            
            //if there are no rooms left of the desired difficulty
            if(possibleRooms.Count < 1 || findAttempts > 100)
            {
                possibleRooms.Clear();

                foreach (Room room in allRooms)
                    possibleRooms.Add(room);
            }

            int next = prng.Next();
            int i = next % possibleRooms.Count;
            //if (debug) print("Chosen room index: " + i);
            if (i < possibleRooms.Count)
            {
                doorLink = possibleRooms[i].GetTargetDoor(targetOrientation);
                if (doorLink == null)
                    possibleRooms.RemoveAt(i);
            }

        } while (doorLink == null);

        if(doorLink.associatedRoom.removeDoorsUponEntering)
            doorLink.associatedRoom.doorsAvailable.Remove(doorLink);
        currentRoom = doorLink.associatedRoom.roomName;

        //questSys.QuestEvent(QuestSystem.QuestEventType.RoomEnter, currentRoom);

        return doorLink;
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

            room.ChooseRoomModifier();

            if (room.enemySpawner != null)
                room.enemySpawner.ApplyRoomModifier(room.currentModifier);

            foreach (Door door in room.doors)
            {
                door.linkedDoor = null;
                door.ResetDoor();
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
            if (roomsByName[currentRoom].enemySpawner.AreEnemiesRemaining())
            {
                roomsByName[currentRoom].enemySpawner.waveActive = false;
            }
            else
            {
                Door[] allDoors = FindObjectsOfType<Door>();

                foreach (Door d in allDoors)
                {
                    d.locked = false;
                    d.LockGate(d.locked);
                }
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

    public void AddTraversal()
    {
        roomTraversals++;

        if (roomTraversals >= traversalsForReset)
        {
            ResetDoorLinks();
            roomTraversals = 0;
        }
    }

    public void AddToGuaranteeRooms(string roomName)
    {
        roomGuarantees.Enqueue(roomsByName[roomName]);

        if (debug) print($"Added {roomName} to guaranteed rooms queue (Count {roomGuarantees.Count})");
    }
    public void ClearGuaranteeRooms()
    {
        roomGuarantees.Clear();
    }

    public void UnlockToBossDoor()
    {
        toBossDoor.UnlockDoor();
    }
    public void UnlockFromBossDoor()
    {
        fromBossDoor.UnlockDoor();
    }

    public void LoadData(SerializedClass save)
    {
        currentFloor = floorNumber;
        maxFloorUnlocked = save.maxLevelAchieved;
        maxFloorTraveledTo = save.maxLevelTravelledTo;
        mediumChance = save.mediumRoomChance;
        hardChance = save.hardRoomChance;
    }

    public void SetCurrentRoom(string roomName)
    {
        currentRoom = roomName;
    }
}
