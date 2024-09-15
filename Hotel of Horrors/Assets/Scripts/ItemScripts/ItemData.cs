using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : MonoBehaviour
{
    [SerializeField] UseableInfo thisItemData;
    [SerializeField] int count = 1;
    [SerializeField] float activationTime = 0.7f;
    [SerializeField] int defaultLayer;
    [SerializeField] int collectableLayer;

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
    public void SetItemData(UseableInfo item, int count)
    {
        thisItemData = item;
        this.count = count;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.sprite = item.GetSprite();
    }

    void ActivateCollider()
    {
        gameObject.layer = collectableLayer;
        //GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {

    }
}
