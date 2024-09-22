using UnityEngine;

interface QuestType
{
    string ToSaveString();
}

public class FindObject : QuestType
{
    public FindObject(string objName)
    {
        objectName = objName;
    }

    string objectName;

    public string ToSaveString()
    {
        return $"FindObject|{objectName}";
    }
}

public class FindMultiple : QuestType
{
    public FindMultiple(string[] objs)
    {
        objects = objs;
    }

    string[] objects;

    public string ToSaveString()
    {
        string s = "";

        foreach (string s1 in objects)
        {
            s += s1;
            s += "|";
        }

        return $"FindMultiple|{s}";
    }
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

    public string ToSaveString()
    {
        return $"Talk|{npc}|{dialouge.ToString()}";
    }
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

    public string ToSaveString()
    {
        return $"Kill|{amountKilled}|{total}";
    }
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
    public string ToSaveString()
    {
        return $"KillSpecific|{amountKilled}|{total}|{enemyName}";
    }
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

    public string ToSaveString()
    {
        return $"Collect|{amountCollected}|{total}";
    }
}

public class Traverse : QuestType
{
    public Traverse(string room)
    {
        roomName = room;
    }
    string roomName;

    public string ToSaveString()
    {
        return $"Traverse|{roomName}";
    }
}