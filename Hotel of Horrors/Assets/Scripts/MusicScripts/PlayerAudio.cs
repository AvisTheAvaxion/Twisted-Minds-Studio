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
    }

    public void Dash()
    {
        DashSource.Play();
    }

    public void Damage()
    {
        DamageSource.Play();
    }







    void Movement()
    {
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
