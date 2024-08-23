using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Quest", menuName = "QuestItems/Quest", order = 0)]
public class Quest : ScriptableObject
{
    public List<QuestStep> Steps;
    public string QuestName;
    public string Description;
    public List<UseableInfo> ItemRewards;
    public bool Completed;


    public void CheckGoals()
    {
        Completed = Steps.All(s => s.Completed);

        if (Completed)
        {
            GiveReward();
        }
        
    }

    void GiveReward()
    {
        if(ItemRewards != null)
        {
            //Give the items to the player
        }
    }

    public List<QuestStep> GetSteps()
    {
        return Steps;
    }

    public void AddStep(QuestStep newStep)
    {
        Steps.Add(newStep);
    }
}
