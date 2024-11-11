using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public enum State
    {
        Inventory,
        ElevatorRoom,
        MindRoom,
        InDialogue,
        None
    }
    static bool isPaused = false;
    public static bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
            if(value == false && currentState == State.None)
            {
                Time.timeScale = 1;
            }
            else
            {
                Cursor.visible = true;
            }
        }
    }
    static State currentState = State.None;
    public static State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
            if(!isPaused && value == State.None)
            {
                Cursor.visible = false;
                Time.timeScale = 1;
            }
            else
            {
                Cursor.visible = true;
                if (value == State.Inventory) Time.timeScale = 0;
            }
        }
    }
}
