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

public class FindMultiple : QuestType
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
}
public class Collect : QuestType
{
    public Collect(int amountCollected, int total)
    {
        this.amountCollected = amountCollected;
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
    public Talk(GameObject npc, Dialogue.Dialog dialog)
    {
        this.npc = npc;
        this.dialouge = dialog;
    }
    GameObject npc;
    Dialogue.Dialog dialouge;

    #region Talk Methods
    public string GetNPC()
    {
        return this.npc.name;
    }

    public Dialogue.Dialog GetDialog()
    {
        return this.dialouge;
    }

    public override string ToSaveString()
    {
        return $"Talk|{npc}|{dialouge.ToString()}";
    }
    #endregion
}
public class Kill : QuestType
{
    public Kill(int amountKilled, int total)
    {
        this.total = total;
        this.amountKilled = amountKilled;
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
    public KillSpecific(int amoundKilled, int total, string enemyName)
    {
        this.amountKilled=amoundKilled;
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

