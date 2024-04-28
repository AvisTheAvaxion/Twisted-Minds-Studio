using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] SpriteRenderer lightRing;
    [SerializeField] Light2D light2D;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float minFlickerRate, maxFlickerRate;
    [SerializeField] bool changeSize = false;
    [SerializeField] float minSizeChange, maxSizeChange;

    Vector3 defaultLightSize;
    Vector3 defaultLightRingSize;

    float timer = 0;

    float currentFlickerRate;

    int index;

    // Start is called before the first frame update
    void Start()
    {
        if (light2D == null) light2D = GetComponentInChildren<Light2D>();
        if (lightRing == null) lightRing = GetComponentInChildren<SpriteRenderer>();

        defaultLightSize = light2D.gameObject.transform.localScale;
        defaultLightRingSize = lightRing.gameObject.transform.localScale;

        currentFlickerRate = 1f / Random.Range(minFlickerRate, maxFlickerRate);

        index = 0;
        lightRing.sprite = sprites[index];
        light2D.lightCookieSprite = sprites[index];
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= currentFlickerRate) 
        {
            index = (index + 1) % sprites.Length;
            lightRing.sprite = sprites[index];
            light2D.lightCookieSprite = sprites[index];

            if (changeSize)
            {
                lightRing.transform.localScale = defaultLightRingSize * (1 + Random.Range(minSizeChange, maxSizeChange));
                light2D.transform.localScale = defaultLightSize * (1 + Random.Range(minSizeChange, maxSizeChange));
            }

            currentFlickerRate = 1f / Random.Range(minFlickerRate, maxFlickerRate);

            timer = 0;
        }

        timer += Time.deltaTime;
    }
}
