using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    bool isCompleted;

    public abstract void StepCheck();

    public void SetCompletion(bool isCompleted)
    {
        this.isCompleted = isCompleted;
    }

    public bool GetIsCompleted()
    {
        return isCompleted;
    }
}
