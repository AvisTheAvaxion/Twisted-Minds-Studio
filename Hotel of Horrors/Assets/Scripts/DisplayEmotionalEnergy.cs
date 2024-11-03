using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayEmotionalEnergy : MonoBehaviour
{
    [SerializeField] TMP_Text eeText;
    [SerializeField] PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

    private void Update()
    {
        if(playerInventory != null)
        {
            eeText.text = playerInventory.emotionalEnergy.ToString();
        }
    }
}
