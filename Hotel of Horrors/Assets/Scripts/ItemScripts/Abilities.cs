using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilities", menuName = "Useables/Abilities", order = 3)]

public class Abilities : Useables
{
    [SerializeField] GameObject playerAbility;
    public override void Use()
    {
        Debug.Log($"Abilities {GetName()} Used");
    }
    public GameObject GetPlayerAbility()
    {
        return playerAbility;
    }
}
