using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attacks;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "Useables/Weapon", order = 0)]
public class WeaponInfo : UseableInfo
{
    [System.Serializable]
    public struct UpgradeInfo
    {
        public float percentageIncrease;
        public float otherWeaponPercPerLevel;
        public int eeCost;
    }

    [System.Serializable]
    public struct Attack
    {
        public float damagePercIncrease;
        public float knockbackPercIncrease;
        public GameObject weaponStrike;
        public AnimatorOverrideController overrideController;
    }

    [Header("Weapon Settings")]
    [SerializeField] AttackModes mode;
    [SerializeField] AttackType type;
    [SerializeField] GameObject weaponAbility;
    [SerializeField] float weaponAbilityDuration;
    [Space(20)]
    [SerializeField] int baseLevel = 1;
    [SerializeField] int maxLevel = 3;
    [SerializeField] float attackSpeed = 2;
    [SerializeField] bool autoAttack;

    [Header("Melee Settings")]
    [SerializeField] Attack[] attacks;
    [SerializeField] int damage = 5;
    [SerializeField] float deflectionStrength = 2;
    [SerializeField] float knockback = 1.5f;
    [SerializeField] float range;
    [SerializeField] [Range(0,1)] float chanceToInflictEffect;
    [SerializeField] EffectInfo[] effectsToInflict;
    [SerializeField] AnimationCurve creationRandomFactor;
    [SerializeField] UpgradeInfo[] upgradeInfos;

    public override void Use()
    {
        //Debug.Log($"WEAPON {this.GetName()} USED");
    }
    public GameObject GetWeaponAbility()
    {
        return weaponAbility;
    }
    public float GetWeaponAbilityDuration()
    {
        return weaponAbilityDuration;
    }
    /*public string GetWeaponAbilityName()
    {
        return weaponAbilityName;
    }
    public string GetWeaponAbilityDescription()
    {
        return weaponAbilityDescription;
    }
    public Sprite GetWeaponAbilitySprite()
    {
        return weaponAbilitySprite;
    }*/

    /*public override string GetDescription()
    {
        string[] words = base.GetDescription().Split(' ');

        string description = "";

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].StartsWith('$')) 
            {
                string keyword = words[i].ToLower();
                string newWord = words[i].ToLower();
                
                if(keyword.Contains("damage"))
                {
                    keyword = "damage";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", damage));
                }
                else if (keyword.Contains("deflectionstrength"))
                {
                    keyword = "deflectionstrength";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", deflectionStrength));
                }
                else if (keyword.Contains("knockback")) 
                {
                    keyword = "knockback";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", knockback));
                }
                else if(keyword.Contains("attackspeed"))
                {
                    keyword = "attackspeed";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", attackSpeed));
                }
                else if(keyword.Contains("range"))
                {
                    keyword = "range";
                    newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", range));
                }

                description += newWord + " ";
            }
            else
            {
                description += words[i] + " ";
            }
        }

        return description;
    }*/

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
    public GameObject GetWeaponStrike(int attackNumber)
    {
        return GetAttack(attackNumber).weaponStrike;
    }
    public float GetChanceToInflictEffect()
    {
        return chanceToInflictEffect;
    }
    public EffectInfo[] GetEffectsToInflict()
    {
        return effectsToInflict;
    } 
    public int GetBaseLevel()
    {
        return baseLevel;
    }
    public int GetMaxLevel()
    {
        return maxLevel;
    }
    public UpgradeInfo GetUpgradeInfo(int index)
    {
        return (index >= 0 && index < upgradeInfos.Length) ? upgradeInfos[index] : new UpgradeInfo();
    }
    public float CreationRandomFactor()
    {
        float rand = Random.Range(0f, 1f);
        return 1 + creationRandomFactor.Evaluate(rand);
    }
    public Attack GetAttack(int index)
    {
        return (index >= 0 && index < attacks.Length) ? attacks[index] : new Attack();
    }
    public string GetEffectsDescription()
    {
        string str = "";
        for (int i = 0; i < effectsToInflict.Length; i++)
        {
            str += $"<b>{string.Format("{0:0}", chanceToInflictEffect * 100)}%</b> chance to inflict " + effectsToInflict[i].ToString();
        }
        return str;
    }
}

