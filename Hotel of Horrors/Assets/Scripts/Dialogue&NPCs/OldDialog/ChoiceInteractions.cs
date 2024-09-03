using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceInteractions : MonoBehaviour, IPointerClickHandler
{
    int choiceIndex;

    public event EventHandler OnChoiceInteract;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            IntArgs args = new IntArgs(choiceIndex);
            OnChoiceInteract?.Invoke(this, args);
        }
    }

    public void SetChoiceIndex(int index)
    {
        choiceIndex = index;
    }
}

class IntArgs : EventArgs
{
    int heldInteger;

    public IntArgs(int heldInteger)
    {
        this.heldInteger = heldInteger;
    }

    public void SetHeldInteger(int heldInteger)
    {
        this.heldInteger = heldInteger;
    }

    public int GetHeldInteger()
    {
        return this.heldInteger;
    }
}
