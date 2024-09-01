using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class ItemToolTip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameTxt;
    [SerializeField] TextMeshProUGUI itemDescriptionTxt;
    [SerializeField] TextMeshProUGUI itemTypeTxt;
    [SerializeField] TextMeshProUGUI itemMainStatTxt;
    [SerializeField] Transform itemStatsListParent;
    [SerializeField] GameObject itemStatTxtPrefab;
    
    TextMeshProUGUI[] itemStatsTxt;

    public void AssignItem(Item item)
    {
        if (item != null)
        {
            ResetTertiaryItemStats();

            ItemInfo heldItem = item.GetInfo();

            itemNameTxt.text = heldItem.GetName();
            itemDescriptionTxt.text = heldItem.GetDescription();

            itemTypeTxt.text = "Consumable";
            EffectInfo[] heldItemEffects = heldItem.GetEffectInfos();
            if (heldItemEffects.Length > 0)
            {
                //itemMainStatTxt = Instantiate(itemStatTxtPrefab, itemStatsListParent).GetComponent<TextMeshProUGUI>();

                for (int i = 0; i < heldItemEffects.Length; i++)
                {
                    EffectInfo current = heldItemEffects[i];
                    if (current.Mode == EffectInfo.EffectMode.Permenant)
                    {
                        string str = "";
                        for (int e = 0; e < current.StatEffects.Length; e++)
                        {
                            EffectInfo.StatEffect currentSE = current.StatEffects[e];
                            if (currentSE.StatType == Stat.StatType.Health)
                            {
                                if (currentSE.EffectType == EffectInfo.EffectType.Buff)
                                    str += $"Heals {currentSE.Strength}\n";
                                else
                                    str += $"Damages for {currentSE.Strength}\n";
                            }
                            else
                            {
                                if (currentSE.EffectType == EffectInfo.EffectType.Buff)
                                    str += $"{(currentSE.IsPercentage ? "%" + (currentSE.Strength * 100).ToString("#") : "+" + currentSE.Strength.ToString("#"))} {currentSE.StatType.ToString()}\n";
                                else
                                    str += $"{(currentSE.IsPercentage ? "-%" + (currentSE.Strength * 100).ToString("#") : "-" + currentSE.Strength.ToString("#"))} {currentSE.StatType.ToString()}\n";
                            }
                        }
                        itemMainStatTxt.text = str;
                    }
                    else if (current.Mode == EffectInfo.EffectMode.Overtime)
                    {
                        string str = "";
                        for (int e = 0; e < current.StatEffects.Length; e++)
                        {
                            EffectInfo.StatEffect currentSE = current.StatEffects[e];
                            float strength = Mathf.Round(currentSE.Strength * (current.Duration / current.Interval));
                            if (currentSE.EffectType == EffectInfo.EffectType.Buff)
                                str += $"{(currentSE.IsPercentage ? "+%" + (strength * 100).ToString("#") : "+" + strength.ToString("#"))} {currentSE.StatType.ToString()} over {current.Duration}\n";
                            else
                                str += $"{(currentSE.IsPercentage ? "-%" + (strength * 100).ToString("#") : "-" + strength.ToString("#"))} {currentSE.StatType.ToString()} over {current.Duration}\n";
                        }
                        itemMainStatTxt.text = str;
                    }
                    else
                    {
                        string str = "";
                        for (int e = 0; e < current.StatEffects.Length; e++)
                        {
                            EffectInfo.StatEffect currentSE = current.StatEffects[e];
                            if (currentSE.EffectType == EffectInfo.EffectType.Buff)
                                str += $"{(currentSE.IsPercentage ? "+%" + (currentSE.Strength * 100).ToString("#") : "+" + currentSE.Strength.ToString("#"))} {currentSE.StatType.ToString()} for {current.Duration}\n";
                            else
                                str += $"{(currentSE.IsPercentage ? "-%" + (currentSE.Strength * 100).ToString("#") : "-" + currentSE.Strength.ToString("#"))} {currentSE.StatType.ToString()} for {current.Duration}";
                        }
                        itemMainStatTxt.text = str;
                    }
                }
            }
        }
    }

    public void AssignWeapon(Weapon weapon)
    {
        if (weapon != null)
        {
            ResetTertiaryItemStats();

            Weapon heldWeapon = weapon;

            itemNameTxt.text = heldWeapon.GetInfo().GetName();
            itemDescriptionTxt.text = heldWeapon.GetInfo().GetDescription();

            itemTypeTxt.text = $"{heldWeapon.GetInfo().GetWeaponMode()}";
            itemMainStatTxt.text = $"Damage: {heldWeapon.damage}";

            itemStatsTxt = new TextMeshProUGUI[4];
            for (int i = 0; i < 3; i++)
            {
                itemStatsTxt[i] = Instantiate(itemStatTxtPrefab, itemStatsListParent).GetComponent<TextMeshProUGUI>();
            }
            itemStatsTxt[0].text = $"Attack Speed: {heldWeapon.attackSpeed}";
            itemStatsTxt[1].text = $"Knockback: {heldWeapon.knockback}";
            itemStatsTxt[2].text = $"Deflection Strength: {heldWeapon.deflectionStrength}";

            EffectInfo[] heldWeaponEffects = heldWeapon.GetInfo().GetEffectsToInflict();
            if (heldWeaponEffects.Length > 0)
            {
                itemStatsTxt[3] = Instantiate(itemStatTxtPrefab, itemStatsListParent).GetComponent<TextMeshProUGUI>();

                itemStatsTxt[3].text = $"{heldWeapon.GetInfo().GetChanceToInflictEffect() * 100}% chance to inflict ";
                for (int i = 0; i < heldWeaponEffects.Length - 1; i++)
                {
                    itemStatsTxt[3].text += heldWeaponEffects[i].Name;
                    if (heldWeaponEffects.Length > 2 && i < heldWeaponEffects.Length)
                        itemStatsTxt[3].text += ", ";
                }
                if (heldWeaponEffects.Length > 1)
                    itemStatsTxt[3].text += $"and {heldWeaponEffects[heldWeaponEffects.Length - 1].Name}";
                else
                    itemStatsTxt[3].text += $"{heldWeaponEffects[heldWeaponEffects.Length - 1].Name}";
            }
        } 
    }

    void ResetTertiaryItemStats()
    {
        if (itemStatsTxt != null)
        {
            for (int i = 0; i < itemStatsTxt.Length; i++)
            {
                if(itemStatsTxt[i] != null)
                    Destroy(itemStatsTxt[i].gameObject);
                itemStatsTxt[i] = null;
            }
        }
        itemStatsTxt = null;
    }
}
