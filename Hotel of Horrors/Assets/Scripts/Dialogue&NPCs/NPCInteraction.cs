using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] bool npcInRange;
    Dialogue.Dialog npcDialog;

    public event EventHandler OnPlayerTalk;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") && collision.GetComponent<NPCBehavior>() != null)
        {
            npcDialog = collision.GetComponent<NPCBehavior>().GetNPCDialog();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") && collision.GetComponent<NPCBehavior>() != null)
        {
            npcDialog = Dialogue.Dialog.None;
        }
    }

    void OnInteract()
    {
        if (npcDialog != Dialogue.Dialog.None)
        {
            FindObjectOfType<DialogueManager>().SetCutscene(npcDialog);
        }
    }
}
