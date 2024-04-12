using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBoxInteractions : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]bool isInteractable;

    public event EventHandler OnDialogueAdvance;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click Attempt");
        if (eventData.button == PointerEventData.InputButton.Left && isInteractable)
        {
            Debug.Log("Click Success");
            OnDialogueAdvance?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ToggleInteraction(bool input)
    {
        isInteractable = input;
    }
}
