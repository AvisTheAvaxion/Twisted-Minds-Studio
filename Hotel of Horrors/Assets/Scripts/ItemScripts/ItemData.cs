using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    [SerializeField] Useables thisItemData;

    public Useables GetItemData()
    {
        return thisItemData;
    }
    public void SetItemData(Useables item)
    {
        thisItemData = item;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.sprite = item.GetSprite();
    }
}
