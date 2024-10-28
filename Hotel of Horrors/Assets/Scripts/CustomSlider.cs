using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Slider slider;

    public void UpdateVisuals()
    {
        image.fillAmount = (slider.value - slider.minValue) / (slider.maxValue - slider.minValue);
    }
}
