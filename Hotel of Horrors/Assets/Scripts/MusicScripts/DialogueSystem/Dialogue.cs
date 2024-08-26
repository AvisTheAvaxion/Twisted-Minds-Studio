using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Dialogue
{
    public string[] getDialogue(string name)
    {
        switch (name)
        {
            case "Tutorial":
                return Tutorial;

            case "VarrenEncounter":
                return VarrenEncounter;
                
        }
        return null;
    }



    string[] Tutorial = new string[]
    {
        "???: Welcome to the game!",
        "Blaze: Who are you?",
        "???: Your mom!",
        "Blaze: You're not my mom",
        "???: you're adopted",
        "$Promt|Is it day or night|Day|Night",
        "Good morning|$StartCutScene(VarrenEncounter)",
        "$PlaySound(OpenDoor)",
        "???: I went to go get milk"

    };

    string[] VarrenEncounter = new string[]
    {

    };




}
