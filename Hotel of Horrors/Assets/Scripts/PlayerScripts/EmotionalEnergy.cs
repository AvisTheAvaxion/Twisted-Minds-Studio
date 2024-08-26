using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionalEnergy : MonoBehaviour
{
    public int emtionalEnergy { get; private set; }

    public void SetEmotionalEnergy(int emotionalEnergy)
    {
        this.emtionalEnergy = emotionalEnergy;
    }
}
