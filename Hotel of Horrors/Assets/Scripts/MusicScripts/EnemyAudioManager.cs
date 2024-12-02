using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource[] AttackSources;
    [SerializeField] AudioSource DamageSource;

    [SerializeField] AudioClip[] AttackSounds;

    [SerializeField] GameObject EnemyDeath;

    string EnemyType { get; set; }

    bool audioMuted = false;

    // Start is called before the first frame update
    void Start()
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume");
        foreach(AudioSource source in AttackSources)
        {
            source.volume = volume;
        }
        DamageSource.volume = volume;
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(GameObject go) {
        string name = go.name.Split("(Clone)")[0];
        switch (name)
        {
            case "FlyingBrain":
                AttackSources[0].clip = AttackSounds[0];
                break;

            case "ScalpelPatient":
                AttackSources[0].clip = AttackSounds[1];
                break;

            case "TorsoMonster":
                AttackSources[0].clip = AttackSounds[2];
                break;

            case "CoffinMimic":
                AttackSources[0].clip = AttackSounds[3];
                break;
            default:
                Debug.Log(name + " No sound");
                break;
        }

        AttackSources[0].enabled = true;
        AttackSources[0].Play();
    }

    public void Damage() 
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume");
        DamageSource.volume = volume;
        DamageSource.Play(); 
    }
    public void Die() { Instantiate(EnemyDeath); }

    public void PlaySound(string audioName)
    {
        if (!audioMuted)
        {
            AudioSource AttackSource = GetOpenAttackSource();
            AttackSource.Stop();
            float volume = PlayerPrefs.GetFloat("SFXVolume");
            AttackSource.volume = volume;
            foreach (AudioClip clip in AttackSounds)
            {
                if (clip.name == audioName)
                {
                    AttackSource.clip = clip;
                    break;
                }
            }
            AttackSource.enabled = true;
            AttackSource.Play();
        }
    }

    AudioSource GetOpenAttackSource()
    {
        for (int i = 0; i < AttackSources.Length; i++)
        {
            if (!AttackSources[i].isPlaying) return AttackSources[i];
        }
        return AttackSources[0];
    }

    public void MuteAudio()
    {
        foreach (AudioSource source in AttackSources)
        {
            source.volume = 0;
        }
        DamageSource.volume = 0;

        audioMuted = true;
    }
    public void UnMuteAudio()
    {
        audioMuted = false;
    }
}
