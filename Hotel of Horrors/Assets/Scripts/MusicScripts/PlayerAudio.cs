using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerSounds sounds;

    [SerializeField] AudioSource DashSource;
    [SerializeField] AudioSource DamageSource;
    [SerializeField] AudioSource MovementSource;
    [SerializeField] AudioSource AttackSource;
    [SerializeField] AudioSource AbilitySource;
    [SerializeField] AudioSource NextLineSource;



    private void Start()
    {
        UpdateVolume();
        DashSource.clip = sounds.Dash();
        DamageSource.clip = sounds.Damage();
        MovementSource.clip = sounds.getWalkSound("Wood");
    }

    void Update()
    {
        Movement();
    }

    void UpdateWalkSurface(string surfaceName)
    {
        MovementSource.clip = sounds.getWalkSound(surfaceName);
    }

    //update the volume of all Audiosources depending on the player's settings
    public void UpdateVolume()
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume");

        MovementSource.volume = volume;
        AttackSource.volume = volume;
        DashSource.volume = volume;
        AbilitySource.volume = volume;
        DamageSource.volume = volume;
        NextLineSource.volume = volume;
    }

    public void Dash()
    {
        UpdateVolume();
        DashSource.Play();
    }

    public void Damage()
    {
        UpdateVolume();
        DamageSource.Play();
    }

    public void NextLine(int currentLine)
    {
        UpdateVolume();
        if (currentLine == 0) { return; }
        NextLineSource.Play();
    }






    void Movement()
    {
        UpdateVolume();
        //Sounds for when the player walks
        if (animator.GetBool("isWalking"))
        {
            if (!MovementSource.isPlaying)
            {
                MovementSource.Play();
            }
        }
        else
        {
            if (MovementSource.isPlaying)
            {
                MovementSource.Stop();
            }
        }
    }
}
