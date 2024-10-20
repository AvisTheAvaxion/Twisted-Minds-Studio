using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Useables/Item", order = 2)]
public class ItemInfo : UseableInfo
{
    [SerializeField] float cooldown;
    [SerializeField] EffectInfo[] effects;


    public EffectInfo[] GetEffectInfos()
    {
        return effects;
    }

    public override void Use()
    {
        Debug.Log($"Item {GetName()} Used");
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public string GetEffectsDescription()
    {
        string str = "";
        for (int i = 0; i < effects.Length; i++)
        {
            str += effects[i].ToString();
        }
        return str;
    }
}
