using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] Image image;
    [SerializeField] Sprite toggleOff;
    [SerializeField] Sprite toggleOn;

    public void UpdateToggle()
    {
        if(toggle.isOn)
        {
            image.sprite = toggleOn;
        }
        else
        {
            image.sprite = toggleOff;
        }
    }
}
