using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] bool playerInRange;
    bool isNPCInteractable;
    int npcFlag;
    DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            isNPCInteractable = collision.GetComponent<NPCBehavior>().isInteractable;
            npcFlag = collision.GetComponent<NPCBehavior>().flag;
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            playerInRange = false;
        }
    }

    void OnInteract()
    {
        if (playerInRange && isNPCInteractable)
        {
            dialogueManager.SendMessage("RetrieveDialogue", npcFlag);
        }
    }
}
