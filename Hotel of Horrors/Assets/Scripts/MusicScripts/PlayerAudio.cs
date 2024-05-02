using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource MovementSource;
    [SerializeField] private AudioSource AttackSource;
    [SerializeField] private AudioSource DashSource;
    [SerializeField] private AudioSource AbilitySource;
    [SerializeField] private AudioSource DamageSource;

    [SerializeField] private AudioClip[] AudioClips;

    [SerializeField] Animator animator;

    private Dictionary<string, int> audioDict = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Awake()
    {
        CreateDict();

    }

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume");

        MovementSource.volume = volume;
        AttackSource.volume = volume;
        DashSource.volume = volume;
        AbilitySource.volume = volume;
        DamageSource.volume = volume;

        AudioManager.Play("KarrenTheme");
    }

    private void CreateDict()
    {
        audioDict.Add("WoodWalk", 0);
        audioDict.Add("Dash", 1);
        audioDict.Add("Attack", 2);
        audioDict.Add("EnergyArcAbility", 3);
        audioDict.Add("PlayerDamage", 4);
    }

    // Update is called once per frame
    void Update()
    {
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

        //need to figure out attacking and abilities
    }

    public void Play(string name)
    {
        try
        {
            Play(audioDict[name]);
        }
        catch (KeyNotFoundException)
        {
            throw new AudioError("The specified track " + name + " could not be found");
        }
    }

    public void Play(int id)
    {
        try
        {
            switch (id)
            {
                case 1:
                    DashSource.Play();
                    break;

                case 2:
                    AttackSource.Play();
                    break;

                case 3:
                    AbilitySource.clip = AudioClips[1];
                    AbilitySource.Play();
                    break;

                case 4:
                    DamageSource.Play();
                    break;
            }
        }
        catch (KeyNotFoundException)
        {
            throw new AudioError("The specified track " + id + " could not be found");
        }
    }

    public void PlayAbility(string Abilityname)
    {
        Play(Abilityname.Split("(Clone)")[0]);
    }
}
