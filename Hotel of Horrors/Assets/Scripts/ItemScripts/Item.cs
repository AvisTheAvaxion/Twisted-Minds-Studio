using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IUse
{
    [SerializeField] string weaponName;
    [SerializeField] string description;
    [SerializeField] int effectValue;

    public virtual void Use()
    {
        Debug.Log("Item Used");
    }

    public string GetName()
    {
        return weaponName;
    }

    public string GetDescription()
    {
        return description;
    }
}
