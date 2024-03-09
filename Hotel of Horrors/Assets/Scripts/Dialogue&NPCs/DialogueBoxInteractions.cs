using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBoxInteractions : MonoBehaviour
{
    int dialogueIncrement;
    [SerializeField] DialogueManager manager;

    private void Awake()
    {
        dialogueIncrement = 0;
        manager = FindAnyObjectByType<DialogueManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Click");
            manager.SendMessage("ChangeDialogue", dialogueIncrement);
            dialogueIncrement++;
        }
    }
}
