using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    [SerializeField] GameObject buttonPrompt;
    [Header("State Fields")]
    [SerializeField] Dialogue.Dialog npcDialog = Dialogue.Dialog.None;
    [SerializeField] Dialogue.Dialog replacementDialog = Dialogue.Dialog.None;
    int interactionCount = 0;
    [Header("Multiple Dialog Options")]
    [SerializeField] bool allowMultipleDialogs;
    [SerializeField] List<Dialogue.Dialog> dialogs = new List<Dialogue.Dialog>();

    public Dialogue.Dialog GetNPCDialog()
    {
        if (interactionCount == 0)
        {
            return npcDialog;
        }
        else
        {
            return replacementDialog;
        }
    }

    public void IncrementInteractionCount()
    {
        interactionCount++;
    }

    public List<Dialogue.Dialog> GetNPCDialogList()
    {
        return dialogs;
    }

    public bool CheckForMultipleDialogs()
    {
        return allowMultipleDialogs;
    }

    public void ToggleButtonPrompt(bool toggle)
    {
        if (buttonPrompt) buttonPrompt.SetActive(toggle);
    }
}
