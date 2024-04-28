using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStep : ScriptableObject
{
    [Header("Step Info")]
    public bool Completed;
    public int RequiredAmount;
    public string Description;
    public int CurrentAmount;
    

    public virtual void Init()
    {
        //default initialization stuff
    }

    public void Evaluate()
    {
        if(CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        Completed = true;
    }

    public bool GetCompletion()
    {
        return Completed;
    }

    public void SetCompletion(bool status)
    {
        Completed = status;
    }

    public bool GetReqAmount()
    {
        return Completed;
    }

    public void SetReqAmount(int amount)
    {
        RequiredAmount = amount;
    }
}
