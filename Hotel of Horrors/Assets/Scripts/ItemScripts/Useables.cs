using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Useables : ScriptableObject
{
    [SerializeField] string useableName;
    [SerializeField, TextArea] string description;
    [SerializeField] int maxStackAmount = 1;
    [SerializeField] Sprite sprite;

    public abstract void Use();

    public string GetName()
    {
        return useableName;
    }
    public string GetDescription()
    {
        return description;
    }
    public int GetMaxStackAmount()
    {
        return maxStackAmount;
    }
    public Sprite GetSprite()
    {
        return sprite;
    }
}
