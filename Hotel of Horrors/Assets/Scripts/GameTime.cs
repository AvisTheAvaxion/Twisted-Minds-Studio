using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    static bool interruptable = true;

    public static bool paused { get; private set; }
    
    public static void PauseTime(bool interruptableIn)
    {
        Time.timeScale = 0;

        paused = true;

        if (interruptable)
            interruptable = interruptableIn;
    }
    public static void UnpauseTime()
    {
        Time.timeScale = 1;

        paused = false;

        interruptable = true;
    }

    static void HitStopPause()
    {
        Time.timeScale = 0;
    }
    static void HitStopUnpause()
    {
        Time.timeScale = 1;
    }

    static Queue<float> hitStopQueue = new Queue<float>();
    public static void AddHitStop(float length)
    {
        int count = hitStopQueue.Count;
        hitStopQueue.Enqueue(length / (count + 1));
    }

    static Coroutine hitStopCoroutine;
    IEnumerator HitStop(float length)
    {
        HitStopPause();
        yield return new WaitForSecondsRealtime(length);

        if(interruptable)
            HitStopUnpause();

        hitStopCoroutine = null;
    }

    private void Update()
    {
        if (hitStopQueue.Count > 0)
        {
            if (hitStopCoroutine == null)
            {
                hitStopCoroutine = StartCoroutine(HitStop(hitStopQueue.Dequeue()));
            }
        }
    }
}
