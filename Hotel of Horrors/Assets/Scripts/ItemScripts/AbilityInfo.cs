using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Abilities", menuName = "Useables/Abilities", order = 3)]
public class AbilityInfo : UseableInfo
{
    [System.Serializable]
    public struct UpgradeInfo
    {
        public float percentageIncrease;
        public int eeCost;
    }


    [SerializeField] int baseLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField] int unlockEECost = 200;
    [SerializeField] int unlockFloor = 1;
    [SerializeField] GameObject playerAbility;
    [SerializeField] UpgradeInfo[] upgradeInfos;
    [SerializeField] AnimatorOverrideController animatorOverride;
    [SerializeField] bool canMove = true;

    [Header("Ability Settings (Not all Settings apply to every Ability)")]
    [SerializeField] float cooldown;
    [SerializeField] float duration;
    [SerializeField] float damage;
    [SerializeField] float size;
    [SerializeField] float range;

    [Header("Projectile Settings (Not all may apply)")]
    [SerializeField] int numberOfProjectiles;
    [SerializeField] float deflectionResistance;
    [SerializeField] [Range(0, 1)] float chanceToInflictEffect;
    [SerializeField] EffectInfo[] effectsToInflict;
    [SerializeField] int maxTargets = 1;
    [SerializeField] bool goThroughWalls = false;

    public override void Use()
    {
        Debug.Log($"Abilities {GetName()} Used");
    }
    public GameObject GetPlayerAbility()
    {
        return playerAbility;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public float GetCooldown()
    {
        return cooldown;
    }
    public float GetDuration()
    {
        return duration;
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetSize()
    {
        return size;
    }
    public float GetRange()
    {
        return range;
    }
    public int GetNumOfProjectiles()
    {
        return numberOfProjectiles;
    }
    public float GetDeflectionResistance()
    {
        return deflectionResistance;
    }
    public float GetChanceToInflicEffects()
    {
        return chanceToInflictEffect;
    }
    public EffectInfo[] GetEffectsToInflict()
    {
        return effectsToInflict;
    }
    public int GetMaxTargets()
    {
        return maxTargets;
    }
    public bool CanGoThroughWalls()
    {
        return goThroughWalls;
    }
    public int GetBaseLevel()
    {
        return baseLevel;
    }
    public int GetMaxLevel()
    {
        return maxLevel;
    }
    public int GetUnlockEECost()
    {
        return unlockEECost;
    }
    public int GetUnlockFloor()
    {
        return unlockFloor;
    }
    public UpgradeInfo GetUpgradeInfo(int index)
    {
        return (index < upgradeInfos.Length && index >= 0) ? upgradeInfos[index] : new UpgradeInfo();
    }
    public AnimatorOverrideController GetOverrideController()
    {
        return animatorOverride;
    }
}
