using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : MonoBehaviour
{
    [SerializeField] protected UseableInfo thisItemData;
    [SerializeField] protected int count = 1;
    [SerializeField] protected float activationTime = 0.7f;
    [SerializeField] protected int defaultLayer;
    [SerializeField] protected int collectableLayer;

    private void Awake()
    {
        gameObject.layer = defaultLayer;
        //customRB.Initialize(0.2f, )
        Invoke("ActivateCollider", activationTime);
    }

    public UseableInfo GetItemData()
    {
        return thisItemData;
    }
    public int GetCount()
    {
        return count;
    }
    public virtual void SetItemData(UseableInfo item, int count)
    {
        thisItemData = item;
        this.count = count;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.sprite = item.GetSprite();
    }

    protected void ActivateCollider()
    {
        gameObject.layer = collectableLayer;
        //GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {

    }
}
