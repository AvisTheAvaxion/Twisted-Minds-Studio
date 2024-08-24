using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MementoSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    MementoGUI mementoGUI;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image mementoImage;
    [SerializeField] Image connectingTail;


    private void Start()
    {
        mementoGUI = GetComponentInParent<MementoGUI>();
    }

    public void UpdateImage(MementoInfo memento, bool equipped)
    {
        mementoImage.sprite = memento.GetSprite();

        canvasGroup.alpha = equipped ? 1 : 0.35f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        print("memento click");
        mementoGUI.EquipMemento(transform.GetSiblingIndex());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
