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
            case 1:
                return Floor1[objectiveNum];
        }
        return null;
    }

    public int getObjectiveAmount(int floorNum)
    {
        switch (floorNum)
        {
            case 0:
                Debug.Log("Floor Obj Total: " + Floor0.Length);
                return Floor0.Length;
            case 1:
                return Floor1.Length;

        }
        return 0;
    }

    public enum ObjectiveTriggers
    {
        VarrenTrigger,
        InventoryTutorialTrigger,
        CombatTutorialTrigger,
        MindRoomTrigger
    }

    /*  Cheat sheet for string formatting
    * 
    *   return "QuestTitle|{Quest Title}"
    *   return "QuestDesc|{Breif Quest Description}"
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
        "TripTrigger|VarrenTrigger",
        "SetCutscene|VarrenEncounter",
        "TripTrigger|InventoryTutorialTrigger",
        "SetCutscene|InventoryTutorial",
        "TripTrigger|MindRoomTrigger",
        "SetCutscene|MindRoom",
        "TripTrigger|CombatTutorialTrigger",
        "SetCutscene|CombatTutorial",
        "ClearFloor"
    };

    string[] Floor1 = new string[]
    {
        "QuestTitle|Explore",
        "Traverse|Room3"
    };

}

