using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBoxInteractions : MonoBehaviour, IPointerClickHandler
{
    int dialogueIncrement;
    [SerializeField] DialogueManager manager;

    private void Awake()
    {
        dialogueIncrement = 1;
        manager = FindAnyObjectByType<DialogueManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            manager.SendMessage("ChangeDialogue", dialogueIncrement);
            dialogueIncrement++;
        }
    }
}
