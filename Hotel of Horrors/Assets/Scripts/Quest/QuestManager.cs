using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    List<Quest> currentQuest;
    [SerializeField] List<Quest> allQuest;

    private void Awake()
    {
        currentQuest = new List<Quest>();
    }

    public void AddQuest(int questIndex)
    {
        currentQuest.Add(allQuest[questIndex]);
    }

    public void RemoveQuest(int questIndex)
    {
        currentQuest.RemoveAt(questIndex);
    }

    public List<Quest> ReturnQuest()
    {
        return currentQuest;
    }

    public List<Quest> GetQuests()
    {
        return allQuest;
    }
}
