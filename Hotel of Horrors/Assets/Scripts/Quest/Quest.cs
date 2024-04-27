using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quest : MonoBehaviour
{
    public List<QuestStep> Steps { get; set; } = new List<QuestStep>();
    public string QuestName { get; set; }
    public string Description { get; set; }
    public Item ItemReward { get; set; }
    public bool Completed { get; set; }


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
        if(ItemReward != null)
        {
            //Give the item to the player
        }
    }
}
