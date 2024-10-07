using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    InventoryGUI inventoryGUI;

    [SerializeField] int tabNumber;
    [SerializeField] float moveSpeed = 10;
    [SerializeField] Vector2 hoverPosition;
    [SerializeField] Vector2 selectedPosition;

    Vector2 defaultPosition;

    Coroutine moveCoroutine;

    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        inventoryGUI = FindObjectOfType<InventoryGUI>();
        defaultPosition = transform.localPosition;

        if(tabNumber == 0)
        {
            inventoryGUI.SwitchTabs(tabNumber);
            transform.localPosition = selectedPosition;
        } else
        {
            transform.localPosition = defaultPosition;
        }

        initialized = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryGUI.SwitchTabs(tabNumber);

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveTab(selectedPosition));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryGUI.tab != tabNumber)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveTab(hoverPosition));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(inventoryGUI.tab != tabNumber)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveTab(defaultPosition));
        }
    }

    public void Unselect()
    {
        if (initialized && ((Vector2)transform.localPosition - defaultPosition).sqrMagnitude >= 1)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveTab(defaultPosition));
        }
    }

    IEnumerator MoveTab(Vector2 endPosition)
    {
        Vector2 startPos = transform.localPosition;
        //float duration = (endPosition - startPos).magnitude / moveSpeed
        float t = 0;
        while (t < 1)
        {
            transform.localPosition = Vector2.Lerp(startPos, endPosition, t);
            yield return new WaitForSecondsRealtime(1 / 60f);
            t += moveSpeed * (1 / 60f);
        }

        transform.localPosition = endPosition;
    }
}
