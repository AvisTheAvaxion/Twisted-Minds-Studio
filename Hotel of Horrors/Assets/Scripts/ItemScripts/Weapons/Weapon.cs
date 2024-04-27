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
    [SerializeField] GameObject weaponAbility;
    [Space(20)]
    [SerializeField] float attackSpeed;
    [SerializeField] bool autoAttack;

    [Header("Melee Settings")]
    [SerializeField] int damage;
    [SerializeField] float deflectionStrength;
    [SerializeField] float knockback;
    [SerializeField] float range;
    [SerializeField] GameObject weaponStrike;
    [SerializeField] [Range(0,1)] float chanceToInflictEffect;
    [SerializeField] EffectInfo[] effectsToInflict;

    [Header("Ranged Settings")]
    [SerializeField] GameObject projectile;
    [SerializeField] float fireRate;
    [SerializeField] int maxAmmoCount;
    [SerializeField] float reloadTime;


    public override void Use()
    {
        //Debug.Log($"WEAPON {this.GetName()} USED");
    }
    public GameObject WeaponAbility()
    {
        return weaponAbility;
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
    public float GetAttackSpeed()
    {
        return attackSpeed / 10f;
    }
    public float GetRange()
    {
        return range / 10f;
    }
    public float GetFireRate()
    {
        return fireRate;
    }
    public int GetMaxAmmoCount()
    {
        return maxAmmoCount;
    }
    public float GetReloadTime()
    {
        return reloadTime;
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
    public GameObject GetProjectile()
    {
        return projectile;
    }
    public GameObject GetWeaponStrike()
    {
        return weaponStrike;
    }
    public float GetChanceToInflictEffect()
    {
        return chanceToInflictEffect;
    }
    public EffectInfo[] GetEffectsToInflict()
    {
        return effectsToInflict;
    } 
}

