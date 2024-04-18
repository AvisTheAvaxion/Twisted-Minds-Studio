using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    [SerializeField] Useables thisItemData;
    [SerializeField] int count = 1;

    public Useables GetItemData()
    {
        return thisItemData;
    }
    public int GetCount()
    {
        return count;
    }
    public void SetItemData(Useables item, int count)
    {
        thisItemData = item;
        this.count = count;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.sprite = item.GetSprite();
    }
}
