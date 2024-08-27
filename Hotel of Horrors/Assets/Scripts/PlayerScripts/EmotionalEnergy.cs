using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionalEnergy : MonoBehaviour
{
    public const int minEmotionalEnergyPickup = 5;
    public const int maxEmotionalEnergyPickup = 20;

    [SerializeField] float activationTime = 0.7f;
    public int emotionalEnergy { get; private set; }

    private void Start()
    {
        Invoke("ActivateCollider", activationTime);

        if(emotionalEnergy <= 0) 
        {
            emotionalEnergy = 5;
        }
    }

    public void SetEmotionalEnergy(int emotionalEnergy)
    {
        this.emotionalEnergy = emotionalEnergy;
    }
    void ActivateCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }
}
