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

        damage *= 1 + upgradeInfo.percentageIncrease;
        cooldown *= 1 - upgradeInfo.percentageIncrease;
        duration *= 1 + upgradeInfo.percentageIncrease;
        size *= 1 + upgradeInfo.percentageIncrease;
        range *= 1 + upgradeInfo.percentageIncrease;

        deflectionResistance *= 1 + upgradeInfo.percentageIncrease;

        currentLevel++;

        return true;
    }
}
