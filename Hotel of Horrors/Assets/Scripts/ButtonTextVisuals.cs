using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonTextVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI textHandler;
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color mouseHoverColor = Color.gray;
    [SerializeField] Color mouseClickColor = Color.black;
    [SerializeField] Transform selectionIcon;
    [SerializeField] Animator animator;
    [SerializeField] bool startSelected = false;

    bool mouseOver;

    private void OnEnable()
    {
        if (textHandler == null) textHandler = GetComponent<TextMeshProUGUI>();
        textHandler.color = defaultColor;

         //print(name + " " + textHandler.textInfo.lineInfo[0].characterCount);

        if (startSelected && animator != null)
        {
            animator.SetBool("Highlighted", true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        textHandler.color = mouseClickColor;

        if(animator != null)
        {
            animator.SetBool("Pressed", true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        textHandler.color = mouseHoverColor;

        if (animator != null)
        {
            animator.SetBool("Highlighted", true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        textHandler.color = defaultColor;

        if (animator != null)
        {
            animator.SetBool("Highlighted", false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetBool("Pressed", false);
            animator.SetBool("Highlighted", mouseOver);
        }

        if (mouseOver)
        {
            textHandler.color = mouseHoverColor;
        }
        else
        {
            textHandler.color = defaultColor;
        }
    }
}
