using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBoxInteractions : MonoBehaviour, IPointerClickHandler
{
    bool isInteractable;

    public event EventHandler OnDialogueAdvance;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isInteractable)
        {
            OnDialogueAdvance?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ToggleInteraction(bool input)
    {
        isInteractable = input;
    }
}
