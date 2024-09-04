using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioClip dash;
    [SerializeField] AudioClip damage;

    [SerializeField] List<AudioClip> AttackSounds;
    [SerializeField] List<AudioClip> AbilitySounds;
    [SerializeField] List<AudioClip> MementoSounds;
    [SerializeField] List<AudioClip> WalkSounds;

    public AudioClip getAttackSound(string weaponName)
    {
        foreach (AudioClip sound in AttackSounds)
        {
            if (sound.name == weaponName)
            {
                return sound;
            }
        }
        //throw new AudioError($"Can't find an attack sound with the name {weaponName}");
        return null;
    }

    public AudioClip getAbilitySound(string abilityName)
    {
        foreach (AudioClip sound in AbilitySounds)
        {
            if (sound.name == abilityName)
            {
                return sound;
            }
        }
        //throw new AudioError($"Can't find an ability sound with the name {abilityName}");
        return null;
    }

    public AudioClip getMementoSound(string mementoName)
    {
        foreach (AudioClip sound in MementoSounds)
        {
            if (sound.name == mementoName)
            {
                return sound;
            }
        }
        //throw new AudioError($"Can't find an attack sound with the name {mementoName}");
        return null;
    }

    public AudioClip getWalkSound(string surface)
    {
        foreach (AudioClip sound in WalkSounds)
        {
            if (sound.name == surface)
            {
                return sound;
            }
        }
        //throw new AudioError($"Can't find an surface sound with the name {surface}");
        return null;
    }


    public AudioClip Dash()
    {
        return dash;
    }

    public AudioClip Damage()
    {
        return damage;
    }
}
