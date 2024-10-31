using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    public int id { get; private set; }

    public int currentLevel { get; private set; }

    public float cooldown { get; private set; }
    public float duration { get; private set; }
    public float damage { get; private set; }
    public float size { get; private set; }
    public float range { get; private set; }

    public int numberOfProjectiles { get; private set; }
    public float deflectionResistance { get; private set; }
    public int maxTargets { get; private set; }
    public bool goThroughWalls { get; private set; }

    public AbilityInfo GetInfo()
    {
        return UsableDatabase.abilityInfos[id];
    }

    public Ability(AbilityInfo info)
    {
        id = info.id;
        currentLevel = info.GetBaseLevel();

        cooldown = info.GetCooldown();
        duration = info.GetDuration();
        damage = info.GetDamage();
        size = info.GetSize();
        range = info.GetRange();

        numberOfProjectiles = info.GetNumOfProjectiles();
        deflectionResistance = info.GetDeflectionResistance();
        maxTargets = info.GetMaxTargets();
        goThroughWalls = info.CanGoThroughWalls();
    }

    public Ability(AbilitySave abilitySave)
    {
        id = abilitySave.id;
        currentLevel = abilitySave.currentLevel;
        cooldown = abilitySave.cooldown;
        duration = abilitySave.duration;
        damage = abilitySave.damage;
        size = abilitySave.size;
        range = abilitySave.range;

        numberOfProjectiles = abilitySave.numberOfProjectiles;
        deflectionResistance = abilitySave.deflectionResistance;
        maxTargets = abilitySave.maxTargets;
        goThroughWalls = abilitySave.goThroughWalls;
    }

    public int GetUnlockEECost()
    {
        return GetInfo().GetUnlockEECost();
    }
    public int GetUpgradeEECost()
    {
        return GetInfo().GetUpgradeInfo(currentLevel - 1).eeCost;
    }
    public bool CanUpgrade()
    {
        return currentLevel < GetInfo().GetMaxLevel();
    }

    public bool HasEnoughEEToUnlock(int eeAmount)
    {
        return eeAmount >= GetUnlockEECost();
    }

    public virtual bool Upgrade(int eeAmount)
    {
        if (!CanUpgrade() || !HasEnoughEEToUnlock(eeAmount)) return false;

        AbilityInfo.UpgradeInfo upgradeInfo = GetInfo().GetUpgradeInfo(currentLevel - 1);

        damage *= 1 + (upgradeInfo.percentageIncrease / 100f);
        cooldown *= 1 - (upgradeInfo.percentageIncrease / 100f);
        duration *= 1 + (upgradeInfo.percentageIncrease / 100f);
        size *= 1 + (upgradeInfo.percentageIncrease / 100f);
        range *= 1 + (upgradeInfo.percentageIncrease / 100f);

        deflectionResistance *= 1 + (upgradeInfo.percentageIncrease / 100f);

        currentLevel++;

        return true;
    }

    public virtual string GetName()
    {
        string baseName = GetInfo().GetName();

        string level = "";
        for (int i = 0; i < currentLevel; i++)
        {
            level += "I";
        }

        return baseName + " " + level;
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
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", damage));
                }
                else if (keyword.Contains("duration"))
                {
                    keyword = "duration";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", duration));
                }
                else if (keyword.Contains("size"))
                {
                    keyword = "size";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", size));
                }
                else if (keyword.Contains("range"))
                {
                    keyword = "range";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", range));
                }
                else if (keyword.Contains("deflectionresistance"))
                {
                    keyword = "deflectionresistance";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", deflectionResistance));
                }
                else if (keyword.Contains("maxtargets"))
                {
                    keyword = "maxtargets";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", maxTargets));
                }
                else if (keyword.Contains("numberofprojectiles"))
                {
                    keyword = "numberofprojectiles";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", numberOfProjectiles));
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
