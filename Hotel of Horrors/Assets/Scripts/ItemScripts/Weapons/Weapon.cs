using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attacks;

[CreateAssetMenu(fileName = "Weapon", menuName = "Useables/Weapon", order = 0)]
public class Weapon : Useables
{
    
    [SerializeField] AttackModes type;
    [SerializeField] AnimationClip attackAnim;
    [SerializeField] int damage;
    [SerializeField] int attackSpeed;

    public override void Use()
    {
        //Debug.Log($"WEAPON {this.GetName()} USED");
    }
    public void WeaponSkill()
    {
        Debug.Log("WEAPON SKILL USED");
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetAttackSpeed()
    {
        return attackSpeed;
    }

    public AttackModes GetWeaponMode()
    {
        return type;
    }

    public AnimationClip GetWeaponAnimation()
    {
        return attackAnim;
    }
}

