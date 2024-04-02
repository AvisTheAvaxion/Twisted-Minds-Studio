using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> music;
    AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void PlaySong(string songName)
    {
        Debug.Log(songName);
        foreach (AudioClip clip in music)
        {
            if(clip.name == songName)
            {
                musicSource.clip = clip;
                musicSource.Play();
                break;
            }
        }
    }

    public void StopSong()
    {
        musicSource.Stop();
    }
}
