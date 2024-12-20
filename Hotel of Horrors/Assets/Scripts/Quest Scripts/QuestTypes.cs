using UnityEngine;

public abstract class QuestType
{
    public abstract string ToSaveString();
}

public class FindObject : QuestType
{
    public FindObject(string objName)
    {
        objectName = objName;
    }

    string objectName;

    #region Find Object Methods
    public string GetObjectName()
    {
        return objectName;
    }

    public override string ToSaveString()
    {
        return $"FindObject|{objectName}";
    }
    #endregion
}

/*public class FindMultiple : QuestType
{
    public FindMultiple(string[] objs)
    {
        objects = objs;
        ownedObjs = new bool[objs.Length];
    }

    string[] objects;
    bool[] ownedObjs;

    #region Find Multiple Methods
    public string[] GetObjectNames()
    {
        return objects;
    }

    public bool[] CheckItems(int index)
    {
        if (index > -1)
        {
            ownedObjs[index] = true;
        }
        return ownedObjs;
    }

    public override string ToSaveString()
    {
        string s = "";

        foreach (string s1 in objects)
        {
            s += s1;
            s += "|";
        }

        return $"FindMultiple|{s}";
    }
    #endregion
}*/
public class FindMultiple : QuestType
{
    public FindMultiple (int total, string itemName)
    {
        this.total = total;
        this.itemName = itemName;
    }
    int amountGained;
    int total;
    string itemName;

    #region Kill Specific Methods
    public void IncrementAmountCollected(string itemCollected)
    {
        if (itemCollected == itemName)
        {
            amountGained++;
        }
    }
    public int GetAmountCollected()
    {
        return amountGained;
    }
    public int GetTotal()
    {
        return total;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public override string ToSaveString()
    {
        return $"FindMultiple|{amountGained}|{total}|{itemName}";
    }
    #endregion
}
public class Collect : QuestType
{
    public Collect(int total)
    {
        this.total = total;
    }
    int amountCollected;
    int total;

    #region Collect Methods
    public void IncrementAmountCollected(int amount)
    {
        amountCollected += amount;
    }

    public int GetAmountCollected()
    {
        return amountCollected;
    }

    public int GetTotal()
    {
        return total;
    }

    public override string ToSaveString()
    {
        return $"Collect|{amountCollected}|{total}";
    }
    #endregion
}
public class Traverse : QuestType
{
    public Traverse(string room)
    {
        roomName = room;
    }
    string roomName;

    #region Traverse Methods
    public string GetRoomName()
    {
        return roomName;
    }

    public override string ToSaveString()
    {
        return $"Traverse|{roomName}";
    }
    #endregion
}
public class Talk : QuestType
{
    public Talk(GameObject npc)
    {
        this.npc = npc;
    }
    GameObject npc;

    #region Talk Methods
    public string GetNPC()
    {
        return this.npc.name;
    }

    public override string ToSaveString()
    {
        return $"Talk|{npc}";
    }
    #endregion
}
public class TripTrigger : QuestType
{
    public TripTrigger(string triggerName)
    {
        expectedName = triggerName;
    }
    string expectedName;

    public override string ToSaveString()
    {
        return $"TripTrigger|{expectedName}";
    }

    public string GetExpectedTriggerName()
    {
        return expectedName;
    }
}
public class Kill : QuestType
{
    public Kill(int total)
    {
        this.total = total;
    }
    int amountKilled;
    int total;

    #region Kill Methods
    public void IncrementAmountKilled()
    {
        amountKilled++;
    }

    public int GetAmountKilled()
    {
        return amountKilled;
    }

    public int GetTotal()
    {
        return total;
    }

    public override string ToSaveString()
    {
        return $"Kill|{amountKilled}|{total}";
    }
    #endregion
}
public class KillSpecific : QuestType
{
    public KillSpecific(int total, string enemyName)
    {
        this.total = total;
        this.enemyName = enemyName;
    }
    int amountKilled;
    int total;
    string enemyName;

    #region Kill Specific Methods
    public void IncrementAmountKilled(string enemyKilled)
    {
        if (enemyKilled == enemyName)
        {
            amountKilled++;
        }
    }
    public int GetAmountKilled()
    {
        return amountKilled;
    }
    public int GetTotal()
    {
        return total;
    }

    public string GetEnemyName()
    {
        return enemyName;
    }

    public override string ToSaveString()
    {
        return $"KillSpecific|{amountKilled}|{total}|{enemyName}";
    }
    #endregion
}

public class SetCutscene : QuestType
{
    public SetCutscene(string name)
    {
        cutsceneName = name;
    }

    string cutsceneName;

    public string GetCutsceneName()
    {
        return cutsceneName;
    }

    public override string ToSaveString()
    {
        return $"SetCutscene|{cutsceneName}";
    }
}
public class OpenInventory : QuestType
{
    public OpenInventory()
    {
    }

    public override string ToSaveString()
    {
        return $"OpenInventory";
    }
}
public class BreakLootable : QuestType
{
    public BreakLootable(int total)
    {
        this.total = total;
    }

    int total;
    int currentAmount = 0;
    

    public int GetTotal()
    {
        return total;
    }

    public int GetCurrentAmount()
    {
        return currentAmount;
    }

    public void Increment()
    {
        currentAmount++;
    }

    public bool ConditionReached()
    {
        return currentAmount >= total;
    }

    public override string ToSaveString()
    {
        return $"BreakLootable|{total}";
    }
}
