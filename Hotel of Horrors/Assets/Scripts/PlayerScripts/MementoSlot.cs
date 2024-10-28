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
    [SerializeField] float connectingTime = 0.5f;

    public bool connected { get; private set; }

    public GameObject ConnectingTail { get => connectingTail.gameObject; }


    private void Start()
    {
        mementoGUI = GetComponentInParent<MementoGUI>();
        connectingTail.gameObject.SetActive(false);
        connected = false;
    }

    public void UpdateImage(MementoInfo memento, bool equipped)
    {
        mementoImage.sprite = memento.GetSprite();

        //canvasGroup.alpha = equipped ? 1 : 0.35f;
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

    public void ConnectTail(bool connect)
    {
        if (connectTailCoroutine != null) StopCoroutine(connectTailCoroutine);

        connectTailCoroutine = StartCoroutine(ConnectTailSequence(connect));
    }

    Coroutine connectTailCoroutine;
    IEnumerator ConnectTailSequence(bool connect)
    {
        connectingTail.gameObject.SetActive(true);
        connectingTail.fillAmount = connect ? 0 : 1;
        float t = 0;
        while(t < connectingTime)
        {
            if(connect)
            {
                connectingTail.fillAmount = t / connectingTime;
            } 
            else
            {
                connectingTail.fillAmount = 1 - t / connectingTime;
            }
            yield return null;
            t += Time.deltaTime;
        }
        connectingTail.fillAmount = connect ? 1 : 0;
        connectTailCoroutine = null;

        connected = connect;

        if (!connect) connectingTail.gameObject.SetActive(false);
    }
}
