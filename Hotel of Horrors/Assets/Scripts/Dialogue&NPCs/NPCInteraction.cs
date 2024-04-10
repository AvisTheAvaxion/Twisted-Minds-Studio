using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] bool playerInRange;
    bool isNPCInteractable;
    int npcBlock;
    string npcFile;

    public event EventHandler OnPlayerTalk;

    DialogueSystem dialogueSystem;

    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>();
        dialogueSystem.OnDialogueFinish += DialogueSystem_OnDialogueFinish;
    }

    private void DialogueSystem_OnDialogueFinish(object sender, EventArgs e)
    {
        ToggleInteraction(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            isNPCInteractable = collision.GetComponent<NPCBehavior>().isInteractable;
            npcBlock = collision.GetComponent<NPCBehavior>().flag;
            npcFile = collision.GetComponent<NPCBehavior>().fileName;
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
            NPCArgs args = new NPCArgs(npcBlock, npcFile);
            OnPlayerTalk?.Invoke(this, args);
        }
    }

    public void ToggleInteraction(bool value)
    {
        isNPCInteractable = value;
    }
}

class NPCArgs : EventArgs
{
    int npcBlock;
    string npcFile;

    public NPCArgs(int npcBlock, string npcFile)
    {
        this.npcBlock = npcBlock;
        this.npcFile = npcFile;
    }

    public void SetBlock(int block)
    {
        npcBlock = block;
    }

    public int GetBlock()
    {
        return npcBlock;
    }

    public void SetFile(string file)
    {
        npcFile = file;
    }

    public string GetFile()
    {
        return npcFile;
    }
}
