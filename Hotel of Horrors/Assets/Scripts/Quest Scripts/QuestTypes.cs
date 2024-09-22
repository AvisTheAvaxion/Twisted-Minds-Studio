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
    public FindMultiple()
    {
        
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

    GameObject npc;
    Dialogue.Dialog dialouge;

    public string ToSaveString()
    {
        return $"Talk|{npc}|{dialouge.ToString()}";
    }
}

public class Kill : QuestType
{
    int amountKilled;
    int total;

    public string ToSaveString()
    {
        return $"Kill|{amountKilled}|{total}";
    }
}

public class KillSpecific : QuestType
{
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
    int amountCollected;
    int total;

    public string ToSaveString()
    {
        return $"Collect|{amountCollected}|{total}";
    }
}

public class Traverse : QuestType
{
    string roomName;

    public string ToSaveString()
    {
        return $"Traverse|{roomName}";
    }
}