using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] bool npcInRange;
    QuestSystem questSys;
    Dialogue.Dialog npcDialog;
    string npcName;

    public event EventHandler OnPlayerTalk;

    private void Awake()
    {
        questSys = FindObjectOfType<QuestSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") && collision.GetComponent<NPCBehavior>() != null)
        {
            npcDialog = collision.GetComponent<NPCBehavior>().GetNPCDialog();
            npcName = collision.gameObject.name;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") && collision.GetComponent<NPCBehavior>() != null)
        {
            npcDialog = Dialogue.Dialog.None;
            npcName = null;
        }
    }

    void OnInteract()
    {
        if (npcDialog != Dialogue.Dialog.None)
        {
            FindObjectOfType<DialogueManager>().SetCutscene(npcDialog);
            questSys.QuestEvent(QuestSystem.QuestEventType.NpcInteraction, npcName);
        }
    }
}
