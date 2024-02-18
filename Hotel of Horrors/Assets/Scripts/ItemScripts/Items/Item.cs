using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Useables/Item", order = 2)]
public class Item : Useables
{
    [SerializeField] int potency;

    public override void Use()
    {
        Debug.Log($"Item {GetName()} Used");
    }
}
