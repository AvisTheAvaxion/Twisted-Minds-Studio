using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Memento", menuName = "Useables/Memento", order = 1)]
public class MementoInfo : UseableInfo
{
    [SerializeField] GameObject specialAbility;
    [SerializeField] bool canMove = true;
    [SerializeField] bool canAttackToo = true;

    public bool CanMove()
    {
        return canMove;
    }
    public bool CanAttackToo()
    {
        return canAttackToo;
    }
    public override void Use()
    {
        Debug.Log($"Memento {GetName()} Used");
    }
    public GameObject GetSpecialAbility()
    {
        return specialAbility;
    }

    //Honestly have no idea what to have in the Mementos base class
}
