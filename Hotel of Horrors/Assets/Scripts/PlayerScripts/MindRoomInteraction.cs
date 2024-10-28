using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindRoomInteraction : MonoBehaviour
{
    [SerializeField] GameObject buttonPrompt;
    [SerializeField] GameObject mindRoomCanvas;
    bool inRangeOfChair;
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
        if (inRangeOfChair)
        {
            mindRoomCanvas.SetActive(true);
            Cursor.visible = true;
        }
    }
}
