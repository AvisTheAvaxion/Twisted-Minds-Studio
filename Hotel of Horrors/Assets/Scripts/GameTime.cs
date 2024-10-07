using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    static bool interruptable = true;
    public static void PauseTime(bool interruptableIn)
    {
        Time.timeScale = 0;

        if(interruptable)
            interruptable = interruptableIn;
    }
    public static void UnpauseTime()
    {
        Time.timeScale = 1;

        interruptable = true;
    }

    static Queue<float> hitStopQueue = new Queue<float>();
    public static void AddHitStop(float length)
    {
        hitStopQueue.Enqueue(length);
    }

    static Coroutine hitStopCoroutine;
    IEnumerator HitStop(float length)
    {
        PauseTime(true);
        yield return new WaitForSecondsRealtime(length);

        if(interruptable)
            UnpauseTime();

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
