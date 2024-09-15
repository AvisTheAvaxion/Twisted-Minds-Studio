using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int id { get; private set; }
    public int currentAmount { get; private set; }
    public bool isFull { get; private set; }
    public int CurrentAmount { get => currentAmount; }
    public bool IsFull { get => isFull; }

    public virtual ItemInfo GetInfo()
    {
        return UsableDatabase.itemInfos[id];
    }

    public Item(ItemInfo item, int amount)
    {
        currentAmount = amount;

        id = item.id;
    }
    public Item(ItemSave itemSave)
    {
        id = itemSave.id;
        currentAmount = itemSave.currentAmount;
        isFull = itemSave.isFull;
    }

    public int AddAmount(int amount)
    {
        currentAmount += amount;
        isFull = currentAmount >= GetInfo().GetMaxStackAmount();
        if (isFull)
        {
            int amountOver = currentAmount - GetInfo().GetMaxStackAmount();
            currentAmount = GetInfo().GetMaxStackAmount();
            return amountOver;
        }
        else
            return 0;
    }
    public int RemoveAmount(int amount)
    {
        currentAmount -= amount;
        isFull = false;
        currentAmount = currentAmount < 0 ? 0 : currentAmount;
        return currentAmount;
    }

    public void SetCurrentAmount(int amount)
    {
        currentAmount = amount;
    }

    public virtual string GetName()
    {
        string baseName = GetInfo().GetName();

        return baseName;
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
                    //keyword = "damage";
                    //newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", damage));
                }
                else if (keyword.Contains("deflectionstrength"))
                {
                    //keyword = "deflectionstrength";
                    //newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", deflectionStrength));
                }
                else if (keyword.Contains("knockback"))
                {
                    //keyword = "knockback";
                    //newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", knockback));
                }
                else if (keyword.Contains("attackspeed"))
                {
                    //keyword = "attackspeed";
                    //newWord = newWord.Replace("$" + keyword, string.Format("{0:0.0}", attackSpeed));
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

        return GetInfo().GetDescription();
    }
}
