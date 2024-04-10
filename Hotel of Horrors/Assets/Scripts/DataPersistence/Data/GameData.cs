using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int emotionalEnergy;

    //default values for when someone starts a fresh game
    public GameData()
    {
        //add the variables that we want to save here:
        emotionalEnergy = 0;
    }
}
