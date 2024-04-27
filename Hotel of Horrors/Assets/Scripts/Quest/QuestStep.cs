using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : ScriptableObject
{
    public abstract void StepCheck();

    public event EventHandler OnStepComplete;

    public void SetCompletion()
    {
        OnStepComplete?.Invoke(this, EventArgs.Empty);
    }
}
