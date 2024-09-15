using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UseableInfo : ScriptableObject
{
    public int id { get; private set; }

    [SerializeField] string useableName;
    [SerializeField, TextArea] string description;
    [SerializeField] int maxStackAmount = 1;
    [SerializeField] Sprite sprite;
    [SerializeField] Sprite displaySprite;

    public abstract void Use();

    public virtual string GetName()
    {
        return useableName;
    }
    public virtual string GetDescription()
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
    public Sprite GetDisplaySprite()
    {
        return displaySprite;
    }

    public void SetID(int id)
    {
        this.id = id;
    }
}
