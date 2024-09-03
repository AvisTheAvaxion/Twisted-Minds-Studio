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
        Debug.Log("Could not find a song with that name!");
        return null;
    }

    public AudioClip getEffect(string name)
    {
        foreach (AudioClip clip in music)
        {
            if (clip.name == name) return clip;
        }
        Debug.Log("Could not find an effect with that name");
        return null;
    }

    public AudioClip getAmbience(string name)
    {
        foreach (AudioClip clip in ambience)
        {
            if (clip.name == name) return clip;
        }
        Debug.Log("Could not find an ambience track with that name");
        return null;
    }
}
