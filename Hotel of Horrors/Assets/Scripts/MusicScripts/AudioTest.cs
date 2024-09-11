using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] NewAudioManager AudioManager;

    bool paused = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (!paused)
            {
                paused = true;
                AudioManager.Pause();

            }
            else
            {
                paused = false;
                AudioManager.Play();
            }
        }
    }
}
