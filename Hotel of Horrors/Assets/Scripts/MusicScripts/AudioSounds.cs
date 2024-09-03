using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSounds : MonoBehaviour
{
    [SerializeField] List<AudioClip> music = new List<AudioClip>();
    [SerializeField] List<AudioClip> effects = new List<AudioClip>();
    [SerializeField] List<AudioClip> ambience = new List<AudioClip>();

    public AudioClip getSong(string name)
    {
        foreach (AudioClip clip in music)
        {
            if (clip.name == name) return clip;
        }
        throw new AudioError($"Could not find a song with the name {name}!");
    }

    public AudioClip getEffect(string name)
    {
        foreach (AudioClip clip in music)
        {
            if (clip.name == name) return clip;
        }
        throw new AudioError($"Could not find an effect with the name {name}!");
    }

    public AudioClip getAmbience(string name)
    {
        foreach (AudioClip clip in ambience)
        {
            if (clip.name == name) return clip;
        }
        throw new AudioError($"Could not find an ambience with the name {name}!");
    }
}
