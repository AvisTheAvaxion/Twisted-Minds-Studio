using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : MonoBehaviour, IUse
{
    [SerializeField] string weaponName;
    [SerializeField] string description;
    [SerializeField] int effectValue;

    public void Use()
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
