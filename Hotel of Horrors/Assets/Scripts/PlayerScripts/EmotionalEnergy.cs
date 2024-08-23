using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionalEnergy : MonoBehaviour
{
    [SerializeField] int emtionalEnergy;

    private void Start()
    {
        emtionalEnergy = 0;
    }

    public int GetEmotionalEnergy()
    {
        return this.emtionalEnergy;
    }
}
