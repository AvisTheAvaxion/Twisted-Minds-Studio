using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [SerializeField] List<QuestStep> steps;
    Queue<QuestStep> stepQueue;
    QuestStep currentStep;
    float percentComplete;

    public void SetCurrentStep()
    {
        currentStep = stepQueue.Dequeue();

        int stepTotal = steps.Count;
        int stepsLeft = stepQueue.Count;
        percentComplete = stepTotal - stepsLeft;
    }

    
}
