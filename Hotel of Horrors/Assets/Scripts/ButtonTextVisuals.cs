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

    bool mouseOver;

    private void OnEnable()
    {
        if (textHandler == null) textHandler = GetComponent<TextMeshProUGUI>();
        textHandler.color = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        textHandler.color = mouseClickColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        textHandler.color = mouseHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        textHandler.color = defaultColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(mouseOver)
        {
            textHandler.color = mouseHoverColor;
        }
        else
        {
            textHandler.color = defaultColor;
        }
    }
}
