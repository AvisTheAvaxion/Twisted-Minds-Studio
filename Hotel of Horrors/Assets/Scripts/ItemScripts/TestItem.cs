using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : Item
{
    public override void Use()
    {
        Debug.Log($"Item {GetName()} Used");
    }
}
