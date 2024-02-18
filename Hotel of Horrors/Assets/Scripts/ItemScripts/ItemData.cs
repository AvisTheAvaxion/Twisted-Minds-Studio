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
}
