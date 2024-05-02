using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource MovementSource;
    [SerializeField] private AudioSource AttackSource;
    [SerializeField] private AudioSource DashSource;

    [SerializeField] private AudioClip[] AudioClips;

    [SerializeField]  Animator animator;

    // Start is called before the first frame update
    void Start()
    {

        AudioManager.Play("KarrenTheme");
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("isWalking"))
        {
            if (!MovementSource.isPlaying)
            {
                MovementSource.Play();
            }
        }
        else
        {
            if (MovementSource.isPlaying)
            {
                MovementSource.Stop();
            }
        }

        if (AudioManager.Dash)
        {
            AudioManager.Dash = false;
            DashSource.Play();
        }

        if (AudioManager.Attack)
        {
            AudioManager.Attack = false;
            AttackSource.Play();
        }
    }
}
