using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] Dialogue.Dialog npcDialog = Dialogue.Dialog.None;


    public Dialogue.Dialog GetNPCDialog()
    {
        return npcDialog;
    }
}
