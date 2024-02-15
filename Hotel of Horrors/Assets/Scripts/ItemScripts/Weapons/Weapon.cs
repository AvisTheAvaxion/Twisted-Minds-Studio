using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class Weapon : Useables
{
    
    [SerializeField] WeaponMode type;
    [SerializeField] int damage;
    [SerializeField] int attackSpeed;

    public virtual void WeaponSkill()
    {
        Debug.Log("WEAPON SKILL USED");
    }

    public string GetWeaponMode()
    {
        return $"{type}";
    }

    protected enum WeaponMode
    {
        Melee,
        Ranged,
        None
    }
}
