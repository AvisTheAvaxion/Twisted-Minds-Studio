using System;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource soundEffectSource;
    [SerializeField] AudioSource ambientSource;

    [SerializeField] AudioSource DashSource;

    private static Dictionary<string, int> audioDict = new Dictionary<string, int>();

    private static List<int> soundQueue = new List<int>();
    private static List<int> songQueue = new List<int>();
    private static List<int> ambientQueue = new List<int>();

    [SerializeField] AudioClip[] audioClips;

    private static bool updateVolume = false;
    private static float[] volumes;

    public static bool Dash = false;
    public static bool Attack = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (audioDict.Count == 0) { createDict(); }
        if (PlayerPrefs.HasKey("SFXVolume")) { volumes = Load(); }
        else { volumes = new float[3] { 1f, 1f, 1f}; }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H)) { bgmSource.Stop(); soundEffectSource.Stop(); ambientSource.Stop(); }

        if (updateVolume)
        {
            bgmSource.volume = volumes[0];
            soundEffectSource.volume = volumes[1];
            ambientSource.volume = volumes[2];
            Save();
            updateVolume = false;
        }
        
        if (songQueue.Count != 0)
        {
            if (songQueue[0] == 5) { 
                bgmSource.Stop();
                songQueue.RemoveAt(0);
                return;
            }

            bgmSource.clip = audioClips[songQueue[0]];
            bgmSource.Play();
            songQueue.RemoveAt(0);
            //Debug.Log("Played a song");

        }

        if (soundQueue.Count != 0)
        {
            if (soundQueue[0] == 5)
            {
                soundEffectSource.Stop();
                soundQueue.RemoveAt(0);
                return;
            }

            soundEffectSource.clip = audioClips[soundQueue[0]];
            soundEffectSource.Play();
            soundQueue.RemoveAt(0);
            //Debug.Log("Played a sound effect");
        }

        if (ambientQueue.Count != 0)
        {
            if (ambientQueue[0] == 5)
            {
                ambientSource.Stop();
                ambientQueue.RemoveAt(0);
                return;
            }

            ambientSource.clip = audioClips[ambientQueue[0]];
            ambientSource.Play();
            ambientQueue.RemoveAt(0);
            //Debug.Log("Played a ambient track");
        }
    }

    private static void createDict()
    {
        audioDict.Add("StienTheme", 0);
        audioDict.Add("KarrenTheme", 1);
        audioDict.Add("Elevator", 2);
        audioDict.Add("Door", 3);
        audioDict.Add("Dash", 4);
        audioDict.Add("Attack", 5);
    }

    public static void Play(string audioName)
    {
        if (audioDict.ContainsKey(audioName))
        {
            Play(audioToId(audioName));
        }
        else
        {
            throw new AudioError("The specified track '" + audioName + "' could not be found. Please call AudioManager.Sounds() to view all available tracks");
        }
    }

    public static void Play(int id)
    {
        if (id > audioDict.Count)
        {
            throw new AudioError("The track with id '" + id + "' could not be found. Please call AudioManager.Sounds() to view all available tracks");
        }

        if (id == 4)
        {
            Dash = true;
            return;
        }
        if (id == 5)
        {
            Attack = true;
            return;
        }

        if (id <= 2) { songQueue.Add(id); }

        //else if (id >= 2) { ambientQueue.Add(id); }

        else { soundQueue.Add(id); }
    }

    public static int audioToId(string audioName)
    {
        int id;
        try
        {
            id = audioDict[audioName];
            return id;
        }
        catch (KeyNotFoundException)
        {
            throw new AudioError("The specified track '" + audioName + "' could not be found. Please call AudioManager.Sounds() to view all available tracks");
        }
    }

    public new static string ToString()
    {
        string s = "";

        foreach (KeyValuePair<string, int> kvp  in audioDict)
        {
            s = s + kvp.Key + ":" + kvp.Value + "\n";
        }
        return s;
    }

    public static string ToString(string search)
    {
        string s = "";

        foreach(KeyValuePair<string, int> kvp in audioDict)
        {
            if(kvp.Key.ToLower().Contains(search.ToLower()))
            {
                s = s + kvp.Key + ":" + kvp.Value + "\n";
            }
        }
        return s;
    }

    public static void Stop(string source)
    {
        if (source == "bgm")
        {
            songQueue.Clear();
            songQueue.Add(5);
        }
        else if (source == "se")
        {
            soundQueue.Clear();
            soundQueue.Add(5);
        }
        else if (source == "ambient")
        {
            ambientQueue.Clear();
            ambientQueue.Add(5);
        }
        else if (source == "all")
        {
            Stop("bgm");
            Stop("se");
            Stop("ambient");
        }
    }

    public static void BackgroundVolume(float volume)
    {
        volumes[0] = volume;
        updateVolume = true;
    }

    public static void SFXVolume(float volume)
    {
        volumes[1] = volume;
        updateVolume = true;
    }

    public static void AmbientVolume(float volume)
    {
        volumes[2] = volume;
        updateVolume = true;

    }

    public static float BackgroundVolume()
    {
        return volumes[0];
    }

    public static float SFXVolume()
    {
        return volumes[1];
    }

    public static float AmbientVolume()
    {
        return volumes[2];
    }

    private static void Save()
    {
        PlayerPrefs.SetFloat("BGMVolume", volumes[0]);
        PlayerPrefs.SetFloat("SFXVolume", volumes[1]);
        PlayerPrefs.SetFloat("AmbientVolume", volumes[2]);
    }

    private static float[] Load()
    {
        return new float[3] {PlayerPrefs.GetFloat("BGMVolume"), PlayerPrefs.GetFloat("SFXVolume"),
            PlayerPrefs.GetFloat("AmbientVolume") };
    }
}
