using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Objectives
{
    public string getObjective(int floorNum, int objectiveNum)
    {
        
        switch (floorNum)
        {
            case 0:
                Debug.Log(objectiveNum + "vs" + Floor0.Length);
                return Floor0[objectiveNum];
            case 1:
                return Floor1[objectiveNum];

        }
        return null;
    }

    public int getObjectiveAmount(int floorNum, int objectiveNum)
    {
        switch (floorNum)
        {
            case 0:
                return Floor0[objectiveNum].Length;
            case 1:
                return Floor1[objectiveNum].Length;


        }
        return 0;
    }

    public enum ObjectiveTriggers
    {
        SameRoomTrigger,
        InventoryTutorialTrigger,
        CombatTutorialTrigger
    }
/*  Cheat sheet for string formatting
*   
*   return $"QuestTitle|{quest title}
*   return $"QuestDesc|{brief quest description}"
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
        "QuestTitle|Explore",
        "SetCutscene|WakeUpTutorial",
        "TripTrigger|SameRoomTrigger",
        "SetCutscene|SameRoomAgain",
        "TripTrigger|InventoryTutorialTrigger",
        "SetCutscene|InventoryTutorial",
        "TripTrigger|CombatTutorialTrigger",
        "SetCutscene|CombatTutorial"
    };

    string[] Floor1 = new string[]
    {
        "QuestTitle|Explore",
        "QuestDesc| ",
        "Traverse|Room3",
    };

}

