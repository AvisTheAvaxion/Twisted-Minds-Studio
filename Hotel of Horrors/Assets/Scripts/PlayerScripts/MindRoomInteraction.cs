using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindRoomInteraction : MonoBehaviour
{
    [SerializeField] GameObject buttonPrompt;
    [SerializeField] MindRoomGUI mindRoomGUI;
    bool inRangeOfChair;

    private void Start()
    {
        mindRoomGUI = FindObjectOfType<MindRoomGUI>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("MindRoomChair"))
        {
            inRangeOfChair = true;

            if (buttonPrompt) buttonPrompt.SetActive(true);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("MindRoomChair"))
        {
            inRangeOfChair = false;

            if (buttonPrompt) buttonPrompt.SetActive(false);
        }
    }

    void OnInteract()
    {
        if (!GameState.IsPaused && GameState.CurrentState == GameState.State.None && inRangeOfChair)
        {
            mindRoomGUI.OpenMindRoom();
        }
    }
}
