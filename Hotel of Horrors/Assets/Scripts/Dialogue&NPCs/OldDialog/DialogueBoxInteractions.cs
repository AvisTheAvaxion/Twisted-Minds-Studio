using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script is old and marked for deletion, please update your scripts accordingly.
/// </summary>
/// 


public class DialogueBoxInteractions : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]bool isInteractable;

    public event EventHandler OnDialogueAdvance;

    void Start() { Debug.Log("This script is gonna go soon, so pls update your scripts");  }

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
