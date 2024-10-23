using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoData : ItemData
{
    public int eeWorth { get; private set; }

    public virtual void SetItemData(UseableInfo item, int count, int eeWorth)
    {
        this.eeWorth = eeWorth;
        thisItemData = item;
        this.count = count;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.sprite = item.GetSprite();
    }
}
