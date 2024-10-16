using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource effectSource;
    [SerializeField] AudioSource ambientSource;

    [SerializeField] AudioSounds sounds;

    float[] volumes = new float[3];

    void Awake()
    {
        if (PlayerPrefs.HasKey("SFXVolume")) { volumes = Load(); }
        else { volumes = new float[3] { 1f, 1f, 1f }; }

        //volumes = new float[3] { 0.5f, 0.5f, 0.5f };
        //PlayerPrefs.DeleteAll();
        //Save();
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySong(string songName)
    {
        bgmSource.clip = sounds.getSong(songName);
        bgmSource.Play();
    }
    public void PlayEffect(string effectName)
    {
        effectSource.clip = sounds.getEffect(effectName);
        effectSource.Play();
    }
    public void PlayAmbience(string ambienceName)
    {
        ambientSource.clip = sounds.getAmbience(ambienceName);
        ambientSource.Play();
    }



    //Methods for setting the volume
    public void BackgroundVolume(float volume)
    {
        volumes[0] = volume;
        Save();
    }
    public void SFXVolume(float volume)
    {
        volumes[1] = volume;
        Save();
    }
    public void AmbientVolume(float volume)
    {
        volumes[2] = volume;
        Save();

    }

    //Methods for getting the volume
    public float BackgroundVolume()
    {
        return volumes[0];
    }

    public float SFXVolume()
    {
        return volumes[1];
    }
    public float AmbientVolume()
    {
        return volumes[2];
    }


    private void Save()
    {
        PlayerPrefs.SetFloat("BGMVolume", volumes[0]);
        PlayerPrefs.SetFloat("SFXVolume", volumes[1]);
        PlayerPrefs.SetFloat("AmbientVolume", volumes[2]);

        bgmSource.volume = volumes[0];
        effectSource.volume = volumes[1];
        ambientSource.volume = volumes[2];

        Debug.Log($"Save Audio Ran{volumes[0]}|{volumes[1]}|{volumes[2]}");
    }

    private float[] Load()
    {
        Debug.Log($"Load{PlayerPrefs.GetFloat("BGMVolume")},{PlayerPrefs.GetFloat("SFXVolume")},{PlayerPrefs.GetFloat("AmbientVolume")}");

        return new float[3]
        {
            PlayerPrefs.GetFloat("BGMVolume"),
            PlayerPrefs.GetFloat("SFXVolume"),
            PlayerPrefs.GetFloat("AmbientVolume")
        };
    }

    public void Pause()
    {
        bgmSource.Pause();
        ambientSource.Pause();
    }

    public void Play()
    {
        bgmSource.Play();
        ambientSource.Play();
    }



}
