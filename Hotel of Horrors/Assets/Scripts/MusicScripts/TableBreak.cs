using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBreak : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] bool destroyOnFinish = true;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume");
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if ((!audioSource.isPlaying) && destroyOnFinish) { Destroy(gameObject); }
    }
}
