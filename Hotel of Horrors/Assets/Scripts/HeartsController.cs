using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsController : MonoBehaviour
{
    struct Heart
    {
        public Image heartOutlineRenderer;
        public Image heartRenderer;
    }

    [SerializeField] Sprite[] heartOutlines;
    [SerializeField] Sprite[] hearts;
    [SerializeField] float heartHealthValue = 20;
    [SerializeField] Transform heartContainer;
    [SerializeField] GameObject heartPrefab;

    Heart[] currentHearts;

    public void Init(Stat healthStat)
    {
        currentHearts = new Heart[Mathf.CeilToInt(healthStat.MaxValue / heartHealthValue)];

        for (int i = 0; i < currentHearts.Length; i++)
        {
            GameObject go = Instantiate(heartPrefab, heartContainer);
            currentHearts[i].heartOutlineRenderer = go.GetComponent<Image>();
            currentHearts[i].heartRenderer = go.transform.GetChild(0).GetComponent<Image>();
        }
    }

    public void AdjustHearts(Stat healthStat)
    {
        //print(healthStat.CurrentValue);

        for (int i = currentHearts.Length - 1; i >= 0; i--)
        {
            float relativeHealthValue = healthStat.CurrentValue - i * heartHealthValue;
            float percentage = Mathf.Clamp01(relativeHealthValue / heartHealthValue);

            if(percentage >= 0.9f)
            {
                currentHearts[i].heartRenderer.enabled = true;
                currentHearts[i].heartRenderer.sprite = hearts[0];
            }
            else if (percentage >= 0.6f)
            {
                currentHearts[i].heartRenderer.enabled = true;
                currentHearts[i].heartRenderer.sprite = hearts[1];
            }
            else if (percentage >= 0.3f)
            {
                currentHearts[i].heartRenderer.enabled = true;
                currentHearts[i].heartRenderer.sprite = hearts[3];
            } 
            else if (percentage > 0f)
            {
                currentHearts[i].heartRenderer.enabled = true;
                currentHearts[i].heartRenderer.sprite = hearts[5];
            } else
            {
                currentHearts[i].heartRenderer.enabled = false;
            }
        }
    }
}
