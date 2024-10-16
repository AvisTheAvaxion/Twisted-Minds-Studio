using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    [SerializeField] NewAudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager.PlaySong("KarrenTheme");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
