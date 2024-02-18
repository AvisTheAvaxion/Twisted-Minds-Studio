using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Memento", menuName = "Useables/Memento", order = 1)]
public class Mementos : Useables
{
    public override void Use()
    {
        Debug.Log($"Memento {GetName()} Used");
    }

    //Honestly have no idea what to have in the Mementos base class
}
