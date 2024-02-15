using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAbilites : Abilities
{
    public override void Use()
    {
        Debug.Log($"Abilities {GetName()} Used");
    }
}
