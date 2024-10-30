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

    NPCBehavior activeBehavior;


    List<Dialogue.Dialog> npcDialogList;
    [SerializeField]int lastRandomInt = -1;

    public event EventHandler OnPlayerTalk;

    private void Awake()
    {
        questSys = FindObjectOfType<QuestSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") && collision.GetComponent<NPCBehavior>() != null)
        {
            activeBehavior = collision.GetComponent<NPCBehavior>();
            if (activeBehavior.CheckForMultipleDialogs())
            {
                npcDialogList = activeBehavior.GetNPCDialogList();
            }
            else
            {
                npcDialog = activeBehavior.GetNPCDialog();
            }
            npcName = collision.gameObject.name;
            activeBehavior.ToggleButtonPrompt(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") && collision.GetComponent<NPCBehavior>() != null)
        {
            NPCBehavior behavior = collision.GetComponent<NPCBehavior>();
            if (behavior.CheckForMultipleDialogs())
            {
                npcDialogList = null;
            }
            else
            {
                npcDialog = Dialogue.Dialog.None;
            }
            if(activeBehavior != null)
                activeBehavior.ToggleButtonPrompt(false);
            npcName = null;
            activeBehavior = null;
        }
    }

    void OnInteract()
    {
        if (npcDialog != Dialogue.Dialog.None)
        {
            FindObjectOfType<DialogueManager>().SetCutscene(npcDialog);
            questSys.QuestEvent(QuestSystem.QuestEventType.NpcInteraction, npcName);

            activeBehavior.IncrementInteractionCount();
            activeBehavior.ToggleButtonPrompt(false);
        }
        else if(npcDialogList != null)
        {
            int randomInt = UnityEngine.Random.Range(0, npcDialogList.Count);
            while (randomInt == lastRandomInt)
            {
                randomInt = UnityEngine.Random.Range(0, npcDialogList.Count);
            }
            lastRandomInt = randomInt;
            Dialogue.Dialog randomDialog = npcDialogList[randomInt];
            FindObjectOfType<DialogueManager>().SetCutscene(randomDialog);
            questSys.QuestEvent(QuestSystem.QuestEventType.NpcInteraction, npcName);
        }
    }
}
