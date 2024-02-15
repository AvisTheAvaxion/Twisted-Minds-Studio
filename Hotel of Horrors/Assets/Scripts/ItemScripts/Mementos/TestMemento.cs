using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMemento : Mementos
{
    public override void Use()
    {
        Debug.Log($"Memento {GetName()} Used");
    }
}
