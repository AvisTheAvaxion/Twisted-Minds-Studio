using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindRoomInteraction : MonoBehaviour
{
    [SerializeField] GameObject mindRoomCanvas;
    bool inRangeOfChair;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("MindRoomChair"))
        {
            inRangeOfChair = true;

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("MindRoomChair"))
        {
            inRangeOfChair = false;
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
