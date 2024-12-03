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
            case 2:
                return Floor2[objectiveNum];
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
            case 2:
                return Floor2.Length;

        }
        return 0;
    }

    public enum ObjectiveTriggers
    {
        SlimeLadyMeeting,
        SlimeLadyInventory,
        SlimeLadyCombat,
        VarrenEncounter
    }

    /*  Cheat sheet for string formatting
    * 
    *   return "QuestTitle|{Quest Title}"
    *   return "QuestDesc|{Breif Quest Description}"
    *   return "SetCutscene|{Cutscene name}"
    *   return "Traverse|{roomName}";
        return "Collect|{total}";
        return "KillSpecific|{total}|{enemyName}";
        return "Kill|{total}";
        return "Talk|{npc}|{roomName}";
        return "FindMultiple|{s}"; //s is a string of objects seperated by "|" (obj1|ob2|ob3)
        return "FindObject|{objectName}";
    */

    //Test for the tutorial floor. Uses a lot of TripTrigger Quest since the room detection doesn't work like it does in a normal floor.
    string[] Floor0 = new string[] {
        "SetCutscene|WakeUpTutorial",
        "TripTrigger|SlimeLadyMeeting",
        "SetCutscene|MeetingTheSlimeLady",
        "TripTrigger|SlimeLadyInventory",
        "SetCutscene|InventoryIntro",
        "Talk|SlimeLady_InventoryExplanation",
        "OpenInventory",
        "SetCutscene|InventoryExplaination",
        "TripTrigger|SlimeLadyCombat",
        "SetCutscene|CombatIntro",
        "Talk|SlimeLady_CombatExplanation",
        "BreakLootable|3",
        "SetCutscene|CombatEnemyInbound",
        "Kill|3",
        "SetCutscene|TutorialOver",
        "TripTrigger|VarrenEncounter",
        "SetCutscene|VarrenEncounter",
        //Gotta have enemies spawn and have the player kill them.
        //After that play cutscene TutorialOver and let the leave to the varren room.
        //VarrenEncounter wont be an objective for this. dont really see a point to it.
        "ClearFloor"
    };

    string[] Floor1 = new string[]
    {
        "Talk|DrHarris_Intro|HarrisRoom",
        "Collect|200",
        "SetCutscene|DrHarrisQuest1",
        "Talk|DrHarris_Q2|HarrisRoom",
        "Kill|20",
        "SetCutscene|DrHarrisQuest3",
        "Talk|DrHarris_Q4|HarrisRoom",
        "Talk|FrankJersey(Quest)|DrFrankOffice",
        "KillSpecific|1|FrankNSteinMonster",
        "ClearFloor"
    };

    string[] Floor2 = new string[]
    {
        "SetCutscene|Floor2Intro",
        "Talk|TwinEncounter1|Twins Bedroom",
        "FindObject|Candy",
        "SetCutscene|TryCandy",
        "FindMultiple|Candy|Candy|Candy",
        "SetCutscene|EnoughCandy",
        "Talk|TwinEncounter2|Twins Bedroom",
        "Talk|Evidence1|Lawyers Office",
        "Talk|Evidence2|Lawyers Office (1)",
        "Talk|Evidence3|Lawyers Office (2)",
        "Talk|TwinEncounter3|TwinsBedroom",
        "KillSpecific|1|KarenWerewolf",
        "ClearFloor"
    };









    string[] VarrenIntro = new string[]
    {
        "Varren: You’re here. Hand over the mementos",
        "Blaze: No.",
        "Varren: Excuse me?",
        "Blaze: I won’t let you leave.",
        "Varren: I’m not joking, hand them over.",
        "Varren: We wouldn’t want things to get messy now.",
        "Blaze: No.",
        "Varren: Fine. Have it your way!",
    };



    string[] Varren1 = new string[]
    {


        "Varren: If you stop now, your death will be quick.|Fine.|No. I won’t give up! $Prompt",
        "Varren: You fool!",
        "Varren: Why won’t you give up!",
    };

    string[] Varren2 = new string[]
    {

        "Varren: You have no chance of winning. Fighting is pointless.|Your wounds say otherwise. |You don’t have to do this. $Prompt",
        "Varren: This? This is nothing!",
        "Varen: You’re right.",
        "Varren: We can be friends.",
        "Varren:When you’re just a corpse!",
    };

}

