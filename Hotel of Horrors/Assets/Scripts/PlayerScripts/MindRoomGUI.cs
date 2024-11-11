using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindRoomGUI : MonoBehaviour
{
    [SerializeField] GameObject mindRoomObj;

    public void OpenMindRoom()
    {
        mindRoomObj.SetActive(true);
        GameState.CurrentState = GameState.State.MindRoom;
    }

    public void CloseMindRoom()
    {
        mindRoomObj.SetActive(false);
        GameState.CurrentState = GameState.State.None;
    }
}
