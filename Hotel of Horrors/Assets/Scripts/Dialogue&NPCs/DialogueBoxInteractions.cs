using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBoxInteractions : MonoBehaviour, IPointerClickHandler
{
    static int dialogueIncrement = 1;
    [SerializeField] DialogueManager manager;
    [SerializeField] bool isChoiceBox;
    [SerializeField] Choice currentChoice;
    bool isInteractable;
    

    private void Awake()
    {
        manager = FindAnyObjectByType<DialogueManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Current Incr: " + dialogueIncrement);
        if (eventData.button == PointerEventData.InputButton.Left && !isChoiceBox && isInteractable)
        {
            manager.SendMessage("ChangeDialogue", dialogueIncrement);
            dialogueIncrement++;
        }
        else if(eventData.button == PointerEventData.InputButton.Left && isChoiceBox)
        {
            if(currentChoice == Choice.One)
            {
                manager.TakePlayerChoice(Choice.One);
                manager.SendMessage("RetrieveChoiceDialogue", dialogueIncrement);
            }
            else if(currentChoice == Choice.Two)
            {
                manager.TakePlayerChoice(Choice.Two);
                manager.SendMessage("RetrieveChoiceDialogue", dialogueIncrement);
            }
            dialogueIncrement++;
        }
        
    }

    public void ToggleInteraction(bool input)
    {
        isInteractable = input;
    }

}
public enum Choice
{
    None,
    One,
    Two
}
