using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectIcon : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI durationText;
    [SerializeField] TextMeshProUGUI efficacyText;
    
    public void SetEffect(EffectInfo effect)
    {
        icon.sprite = effect.DisplaySprite;
        durationText.text = effect.Duration.ToString("##");
        efficacyText.text = "";
        if (effect.HasEfficacyLevel)
        {
            string[] parsed = effect.Name.Split(" ");
            efficacyText.text = parsed[parsed.Length - 1];
        }
    }

    public void UpdateDuration(float duration)
    {
        durationText.text = duration.ToString("##");
    }
}
