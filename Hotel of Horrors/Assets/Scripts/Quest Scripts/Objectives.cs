using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Objectives
{
    //Test for the tutorial floor
    string[] Floor0 = new string[] {
        "Talk|Protag|WakeUpTutorial",
        "Traverse|Room2",
        "SetCutscene|VarrenEncounter",
        "Traverse|Room3",
        "SetCutscene|Nope",

    };

}

/* Cheat sheet for string formatting
 * 
 *         return $"Traverse|{roomName}";
        return $"Collect|{amountCollected}|{total}";
        return $"KillSpecific|{amountKilled}|{total}|{enemyName}";
        return $"Kill|{amountKilled}|{total}";
        return $"Talk|{npc}|{dialouge.ToString()}";
            return $"FindMultiple|{s}"; //s is a string of objects seperated by "|" (obj1|ob2|ob3)
        return $"FindObject|{objectName}";
*/