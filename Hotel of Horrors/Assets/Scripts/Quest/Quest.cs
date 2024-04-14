using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [SerializeField] List<QuestStep> steps;
    Queue<QuestStep> stepQueue;

    public void CompleteQuest()
    {

    }


}
