using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillStep : QuestStep
{
    [SerializeField] int amountRequired;
    [SerializeField] int amountRemaining;
    public override void StepCheck()
    {
        if (amountRequired >= amountRemaining)
        {
            SetCompletion(true);
        }
    }

    void IncrementRemaining()
    {
        amountRemaining++;
    }
}
