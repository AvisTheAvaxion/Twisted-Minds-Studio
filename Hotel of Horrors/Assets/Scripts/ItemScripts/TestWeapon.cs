using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TestWeapon : MonoBehaviour, IUse, IWeaponSkill
{
    [SerializeField] string weaponName;
    [SerializeField] string description;
    [SerializeField] WeaponMode type;
    [SerializeField] int damage;
    [SerializeField] int attackSpeed;


    public void Use()
    {
        Debug.Log("Weapon Used");
    }

    public void WeaponSkill()
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
