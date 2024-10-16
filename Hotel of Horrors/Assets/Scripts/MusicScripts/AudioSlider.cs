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
        NewAudioManager.SFXVolume(sfxSlider.value);
    }

    public void BgmUpdate()
    {
        NewAudioManager.BackgroundVolume(BgmSlider.value);
    }
}
