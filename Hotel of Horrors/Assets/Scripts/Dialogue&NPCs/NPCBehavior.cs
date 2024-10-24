using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] Dialogue.Dialog npcDialog = Dialogue.Dialog.None;
    [Header("Multiple Dialog Options")]
    [SerializeField] bool allowMultipleDialogs;
    [SerializeField] List<Dialogue.Dialog> dialogs = new List<Dialogue.Dialog>();

    public Dialogue.Dialog GetNPCDialog()
    {
        return npcDialog;
    }

    public List<Dialogue.Dialog> GetNPCDialogList()
    {
        return dialogs;
    }

    public bool CheckForMultipleDialogs()
    {
        return allowMultipleDialogs;
    }
}
