using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource AttackSource;
    [SerializeField] AudioSource DamageSource;

    [SerializeField] AudioClip[] AttackSounds;

    [SerializeField] GameObject EnemyDeath;

    string EnemyType { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        float volume = PlayerPrefs.GetFloat("SFXVolume");
        AttackSource.volume = volume;
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
                AttackSource.clip = AttackSounds[0];
                break;

            case "ScalpelPatient":
                AttackSource.clip = AttackSounds[1];
                break;

            case "TorsoMonster":
                AttackSource.clip = AttackSounds[2];
                break;

            case "CoffinMimic":
                AttackSource.clip = AttackSounds[3];
                break;
            default:
                Debug.Log(name + " No sound");
                break;
        }

        AttackSource.enabled = true;
        AttackSource.Play();
    }

    public void Damage() { DamageSource.Play(); }
    public void Die() { Instantiate(EnemyDeath); }
}
