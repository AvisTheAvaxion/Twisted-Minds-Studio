using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SliderStart : MonoBehaviour
{

    [SerializeField] Slider slider;
    [SerializeField] private string source;


    // Start is called before the first frame update
    void Start()
    {
        source = source.ToLower();
        float v = 1;
        if (source == "bgm") { v = AudioManager.BackgroundVolume(); }
        else if (source == "sfx") { v = AudioManager.SFXVolume(); }
        else if (source == "ambient") { v = AudioManager.AmbientVolume(); }

        slider.value = v;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
