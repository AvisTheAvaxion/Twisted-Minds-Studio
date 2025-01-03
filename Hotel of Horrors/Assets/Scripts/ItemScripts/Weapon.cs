using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public int id { get; private set; }
    public int currentLevel { get; private set; }

    public bool autoAttack { get; private set; }
    public float attackSpeed { get; private set; }
    public int damage { get; private set; }
    public float deflectionStrength { get; private set; }
    public float knockback { get; private set; }

    public WeaponInfo GetInfo()
    {
        return UsableDatabase.weaponInfos[id];
    }

    public Weapon(WeaponInfo info)
    {
        id = info.id;
        currentLevel = info.GetBaseLevel();
        autoAttack = info.IsAutoAttack();
        attackSpeed = info.GetAttackSpeed() * info.CreationRandomFactor();
        damage = Mathf.RoundToInt(info.GetDamage() * info.CreationRandomFactor());
        deflectionStrength = info.GetDeflectionStrength() * info.CreationRandomFactor();
        knockback = info.GetKnockback() * info.CreationRandomFactor();
    }
    public Weapon(Weapon weapon)
    {
        id = weapon.id;
        autoAttack = weapon.autoAttack;
        attackSpeed = weapon.attackSpeed;
        damage = weapon.damage;
        deflectionStrength = weapon.deflectionStrength;
        knockback = weapon.knockback;
        currentLevel = weapon.currentLevel;
    }
    public Weapon(WeaponSave weaponSave)
    {
        id = weaponSave.id;
        autoAttack = weaponSave.autoAttack;
        attackSpeed = weaponSave.attackSpeed;
        damage = weaponSave.damage;
        deflectionStrength = weaponSave.deflectionStrength;
        knockback = weaponSave.knockback;
        currentLevel = weaponSave.currentLevel;
    }

    public float GetDamage(int attackNumber)
    {
        return (float)damage * (1f + GetInfo().GetAttack(attackNumber).damagePercIncrease);
    }
    public float GetKnockback(int attackNumber)
    {
        return (float)damage * (1f + GetInfo().GetAttack(attackNumber).knockbackPercIncrease);
    }

    public int GetUpgradeCost(Weapon otherWeapon)
    {
        return GetInfo().GetUpgradeInfo(currentLevel - 1).eeCost +
            Mathf.RoundToInt(otherWeapon.GetInfo().GetUpgradeInfo(otherWeapon.currentLevel - 1).eeCost *
            GetInfo().GetUpgradeInfo(currentLevel - 1).otherWeaponPercPerLevel * otherWeapon.currentLevel);
    }

    public virtual Weapon UpgradePreview(Weapon otherWeapon)
    {
        if (currentLevel >= GetInfo().GetMaxLevel()) return null;

        Weapon upgradedWeapon = new Weapon(this);
        float percentageIncrease = 1 + upgradedWeapon.GetInfo().GetUpgradeInfo(upgradedWeapon.currentLevel - 1).percentageIncrease +
            upgradedWeapon.GetInfo().GetUpgradeInfo(upgradedWeapon.currentLevel - 1).otherWeaponPercPerLevel * otherWeapon.currentLevel;

        upgradedWeapon.attackSpeed *= percentageIncrease;
        upgradedWeapon.damage = Mathf.RoundToInt(upgradedWeapon.damage * percentageIncrease);
        upgradedWeapon.deflectionStrength *= percentageIncrease;
        upgradedWeapon.knockback *= percentageIncrease;

        upgradedWeapon.currentLevel++;

        return upgradedWeapon;
    }
    public virtual void Upgrade(Weapon otherWeapon)
    {
        if (currentLevel >= GetInfo().GetMaxLevel()) return;

        float percentageIncrease = 1 + GetInfo().GetUpgradeInfo(currentLevel - 1).percentageIncrease +
            GetInfo().GetUpgradeInfo(currentLevel - 1).otherWeaponPercPerLevel * otherWeapon.currentLevel;

        attackSpeed *= percentageIncrease;
        damage = Mathf.RoundToInt(damage * percentageIncrease);
        deflectionStrength *= percentageIncrease;
        knockback *= percentageIncrease;

        currentLevel++;
    }

    public virtual string GetName()
    {
        string baseName = GetInfo().GetName();

        return baseName + $" Lv. {currentLevel}";
    }

    public virtual string GetDescription()
    {
        string[] words = GetInfo().GetDescription().Split(' ');

        string description = "";

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].StartsWith('$'))
            {
                string keyword = words[i].ToLower();
                string newWord = words[i].ToLower();

                if (keyword.Contains("damage"))
                {
                    keyword = "damage";
                    newWord = newWord.Replace("$" + keyword, $"<i>{string.Format("{0:0.0}", damage)}</i>");
                }
                else if (keyword.Contains("deflectionstrength"))
                {
                    keyword = "deflectionstrength";
                    newWord = newWord.Replace("$" + keyword, $"<i>{string.Format("{0:0.0}", deflectionStrength)}</i>");
                }
                else if (keyword.Contains("knockback"))
                {
                    keyword = "knockback";
                    newWord = newWord.Replace("$" + keyword, $"<i>{string.Format("{0:0.0}", knockback)}</i>");
                }
                else if (keyword.Contains("attackspeed"))
                {
                    keyword = "attackspeed";
                    newWord = newWord.Replace("$" + keyword, $"<i>{string.Format("{0:0.0}", attackSpeed)}</i>");
                }
                else if (keyword.Contains("range"))
                {
                    //keyword = "range";
                    //newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", range));
                }

                description += newWord + " ";
            }
            else
            {
                description += words[i] + " ";
            }
        }

        return description;
    }
}
