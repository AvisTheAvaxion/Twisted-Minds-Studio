using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attacks;

[CreateAssetMenu(fileName = "Weapon", menuName = "Useables/Weapon", order = 0)]
public class Weapon : Useables
{
    [Header("Weapon Settings")]
    [SerializeField] AttackModes mode;
    [SerializeField] AttackType type;
    [SerializeField] AnimationClip extraAttackAnim;
    [Space(20)]
    [SerializeField] int attackSpeed;
    [SerializeField] bool autoAttack;

    [Header("Melee Settings")]
    [SerializeField] int damage;
    [SerializeField] float deflectionStrength;
    [SerializeField] float knockback;
    [SerializeField] GameObject weaponStrike;

    [Header("Ranged Settings")]
    [SerializeField] GameObject projectile;

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
    public float GetKnockback()
    {
        return knockback;
    }
    public float GetDeflectionStrength()
    {
        return deflectionStrength;
    }
    public int GetAttackSpeed()
    {
        return attackSpeed;
    }
    public bool IsAutoAttack()
    {
        return autoAttack;
    }
    public AttackModes GetWeaponMode()
    {
        return mode;
    }
    public AttackType GetAttackType()
    {
        return type;
    }
    public AnimationClip GetWeaponAnimation()
    {
        return extraAttackAnim;
    }
    public GameObject GetProjectile()
    {
        return projectile;
    }
    public GameObject GetWeaponStrike()
    {
        return weaponStrike;
    }
}

