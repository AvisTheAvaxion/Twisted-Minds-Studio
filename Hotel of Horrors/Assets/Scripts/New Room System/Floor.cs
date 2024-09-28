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
    [SerializeField] float easyChanceGained = -0.02f;
    [SerializeField] float mediumChanceGained = 0.02f;
    [SerializeField] float hardChanceGained = 0.01f;

    //chance gained every time the player enters a new room
    [SerializeField] float elevatorChanceGained = 10f;
    [SerializeField] float mindRoomChanceGained = 7.5f;

    //starting chances for each room type to spawn
    [SerializeField] float easyChance = 80f;
    [SerializeField] float mediumChance = 15f;
    [SerializeField] float hardChance = 5f;
    [SerializeField] float elevatorChance = 0f;
    [SerializeField] float mindRoomChance = 0f;

    string currentRoom;
    int enemiesToKill = 0;

    ElevatorMenuManager elevator;

    private void Start()
    {
        allRooms = FindObjectsOfType<Room>();

        roomsByCategory.Add("Peaceful", new List<Room>());
        roomsByCategory.Add("Easy", new List<Room>());
        roomsByCategory.Add("Medium", new List<Room>());
        roomsByCategory.Add("Hard", new List<Room>());
        roomsByCategory.Add("Mind Room", new List<Room>());

        elevator = FindObjectOfType<ElevatorMenuManager>();
        elevator.ResetShop();

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



        playerInventoryRef = FindObjectOfType<PlayerInventory>();
        questSys = FindObjectOfType<QuestSystem>();

        //waiting to set the EE gains because it takes some time for the inventory to load
        StartCoroutine(WaitToAddEE());
    }

    public void SetEEGains(int amount)
    {
        emotionalEnergyGained = amount;

        easyChance += (easyChanceGained * amount);
        mediumChance += (mediumChanceGained * amount);
        hardChance += (hardChanceGained * amount);
    }

    public void AddEmotionalEnergy(int amountGained)
    {
        emotionalEnergyGained += amountGained;

        easyChance += (easyChanceGained * amountGained);
        mediumChance += (mediumChanceGained * amountGained);
        hardChance += (hardChanceGained * amountGained);
    }

    IEnumerator WaitToAddEE()
    {
        yield return new WaitForSeconds(0.5f);

        SetEEGains(playerInventoryRef.emotionalEnergyGained);
    }

    /// <summary>
    /// Runs the logic for finding the next room to take the player
    /// </summary>
    /// <returns>The next room's spawn location to teleport the player to</returns>
    public Door getDoorLink(Door.DoorLocations targetOrientation)
    {
        Room nextRoom = null;
        Door spawnDoor;
        float difficultyRoll = Random.Range(0, 100);
        List<Room> possibleRooms = new List<Room>();

        roomTraversals++;

        mindRoomChance += mindRoomChanceGained;

        if(roomTraversals >= traversalsForReset)
        {
            ResetDoorLinks();
            roomTraversals = 0;
        }

        if (mindRoomChance > Random.Range(0, 100f))
        {
            nextRoom = roomsByName["Mind Room"];
        }
        else if (hardChance > difficultyRoll)
        {
            possibleRooms = roomsByCategory["Hard"];
        }
        else if (mediumChance > difficultyRoll)
        {
            possibleRooms = roomsByCategory["Medium"];
        }
        else
        {
            possibleRooms = roomsByCategory["Easy"];
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

            spawnDoor = possibleRooms[Random.Range(0, possibleRooms.Count)].GetTargetDoor(targetOrientation);

        } while (spawnDoor == null);

        spawnDoor.associatedRoom.doorsAvailable.Remove(spawnDoor);
        currentRoom = spawnDoor.associatedRoom.roomName;
        questSys.QuestEvent(QuestSystem.QuestEventType.RoomEnter, currentRoom);

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
            }
        }
    }
}
