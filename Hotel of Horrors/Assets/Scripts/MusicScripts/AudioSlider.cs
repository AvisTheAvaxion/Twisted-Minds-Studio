using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] NewAudioManager NewAudioManager;

    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider BgmSlider;

    private void Start()
    {
        sfxSlider.value = NewAudioManager.SFXVolume();
        BgmSlider.value = NewAudioManager.BackgroundVolume();
    }

    public void SfxUpdate()
    {
        Debug.Log(Normalize(sfxSlider));
        NewAudioManager.SFXVolume(Normalize(sfxSlider));
        NewAudioManager.PlayEffect("NextLine");
    }

    public void BgmUpdate()
    {
        Debug.Log(Normalize(BgmSlider));
        NewAudioManager.BackgroundVolume(Normalize(BgmSlider));
    }

    private float Normalize(Slider slider)
    {
        float myValue = slider.value / 20f;
        return myValue;
    }
}
