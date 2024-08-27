using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public int currentLevel { get; private set; }
    public WeaponInfo info { get; private set; }

    public bool autoAttack { get; private set; }
    public float attackSpeed { get; private set; }
    public int damage { get; private set; }
    public float deflectionStrength { get; private set; }
    public float knockback { get; private set; }

    public WeaponInfo GetInfo()
    {
        return info;
    }

    public Weapon(WeaponInfo info)
    {
        this.info = info;
        currentLevel = info.GetBaseLevel();

        autoAttack = info.IsAutoAttack();
        attackSpeed = info.GetAttackSpeed() * info.CreationRandomFactor();
        damage = Mathf.RoundToInt(info.GetDamage() * info.CreationRandomFactor());
        deflectionStrength = info.GetDeflectionStrength() * info.CreationRandomFactor();
        knockback = info.GetKnockback() * info.CreationRandomFactor();
    }
    public Weapon(Weapon weapon)
    {
        info = weapon.info;
        autoAttack = weapon.autoAttack;
        attackSpeed = weapon.attackSpeed;
        damage = weapon.damage;
        deflectionStrength = weapon.deflectionStrength;
        knockback = weapon.knockback;
        currentLevel = weapon.currentLevel;
    }

    public int GetUpgradeCost(Weapon otherWeapon)
    {
        return info.GetUpgradeInfo(currentLevel - 1).eeCost +
            Mathf.RoundToInt(otherWeapon.info.GetUpgradeInfo(otherWeapon.currentLevel - 1).eeCost * 
            info.GetUpgradeInfo(currentLevel - 1).otherWeaponPercPerLevel * otherWeapon.currentLevel);
    }

    public Weapon UpgradePreview(Weapon otherWeapon)
    {
        if (currentLevel >= info.GetMaxLevel()) return null;

        Weapon upgradedWeapon = new Weapon(this);
        float percentageIncrease = 1 + upgradedWeapon.info.GetUpgradeInfo(upgradedWeapon.currentLevel - 1).percentageIncrease +
            upgradedWeapon.info.GetUpgradeInfo(upgradedWeapon.currentLevel - 1).otherWeaponPercPerLevel * otherWeapon.currentLevel;

        upgradedWeapon.attackSpeed *= percentageIncrease;
        upgradedWeapon.damage = Mathf.RoundToInt(upgradedWeapon.damage * percentageIncrease);
        upgradedWeapon.deflectionStrength *= percentageIncrease;
        upgradedWeapon.knockback *= percentageIncrease;

        upgradedWeapon.currentLevel++;

        return upgradedWeapon;
    }
    public void Upgrade(Weapon otherWeapon)
    {
        if (currentLevel >= info.GetMaxLevel()) return;

        float percentageIncrease = 1 + info.GetUpgradeInfo(currentLevel - 1).percentageIncrease +
            info.GetUpgradeInfo(currentLevel - 1).otherWeaponPercPerLevel * otherWeapon.currentLevel;

        attackSpeed *= percentageIncrease;
        damage = Mathf.RoundToInt(damage * percentageIncrease);
        deflectionStrength *= percentageIncrease;
        knockback *= percentageIncrease;

        currentLevel++;
    }
}
