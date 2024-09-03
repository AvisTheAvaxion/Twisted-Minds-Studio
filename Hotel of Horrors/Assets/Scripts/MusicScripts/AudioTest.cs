using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] NewAudioManager AudioManager;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.PlaySong("KarrenTheme");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
