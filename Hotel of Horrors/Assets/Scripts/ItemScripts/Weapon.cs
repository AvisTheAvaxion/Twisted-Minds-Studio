using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Weapon : MonoBehaviour, IAttack, IWeaponSkill
{
    [SerializeField] string weaponName;
    [SerializeField] string description;
    [SerializeField] WeaponMode type;
    [SerializeField] int damage;
    [SerializeField] int attackSpeed;


    public virtual void Attack()
    {
        Debug.Log("Weapon Used");
    }

    public virtual void WeaponSkill()
    {
        Debug.Log("WEAPON SKILL USED");
    }

    public string GetName()
    {
        return weaponName;
    }

    public string GetDescription()
    {
        return description;
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
