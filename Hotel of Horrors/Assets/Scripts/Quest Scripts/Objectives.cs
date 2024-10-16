using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Objectives
{
    public string getObjective(int floorNum, int objectiveNum)
    {
        switch (floorNum)
        {
            case 0:
                return Floor0[objectiveNum];

        }
        return null;
    }

/*  Cheat sheet for string formatting
* 
*   return $"Traverse|{roomName}";
    return $"Collect|{amountCollected}|{total}";
    return $"KillSpecific|{amountKilled}|{total}|{enemyName}";
    return $"Kill|{amountKilled}|{total}";
    return $"Talk|{npc}|{dialouge.ToString()}";
    return $"FindMultiple|{s}"; //s is a string of objects seperated by "|" (obj1|ob2|ob3)
    return $"FindObject|{objectName}";
*/

    //Test for the tutorial floor. Uses a lot of TripTrigger Quest since the room detection doesn't work like it does in a normal floor.
    string[] Floor0 = new string[] {
        "SetCutscene|WakeUpTutorial",
        "TripTrigger|SameRoomTrigger",
        "SetCutscene|SameRoomAgain",
        "TripTrigger|InventoryTutorialTrigger",
        "SetCutscene|InventoryTutorial",
        "Traverse|Room3",
        "SetCutscene|Nope",
    };

    string[] Floor1 = new string[]
    {

    };

}

