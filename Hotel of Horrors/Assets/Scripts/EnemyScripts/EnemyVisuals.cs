using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] enemySpriteRends;
    [SerializeField] float dissolveRate = 0.5f;
    [SerializeField] int frameRate = 16;
    [SerializeField] Animator portalAnim;
    [SerializeField] ParticleSystem[] portalPSs;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            Color color = enemySpriteRends[i].color;
            color.a = 1;
            enemySpriteRends[i].color = color;
        }

        if (portalAnim != null)
            portalAnim.gameObject.SetActive(false);
    }

    public void StartDissolve(float initialWait)
    {
        StartCoroutine(Dissolve(initialWait));
    }

    IEnumerator Dissolve(float initialWait)
    {
        if (portalAnim != null)
        {
            portalAnim.gameObject.SetActive(true);
            portalAnim.SetBool("Close", false);
        }
        ActivatePS();

        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            enemySpriteRends[i].enabled = true;
        }

        yield return new WaitForSeconds(initialWait);

        WaitForSeconds wait = new WaitForSeconds(1f / frameRate);
        float t = 0;
        while(t < dissolveRate)
        {
            for (int i = 0; i < enemySpriteRends.Length; i++)
            {
                Color color = enemySpriteRends[i].color;
                color.a = Mathf.Lerp(1, 0.7f, (t / dissolveRate) * (t / dissolveRate));
                enemySpriteRends[i].color = color;
            }
            yield return wait;
            t += 1f / frameRate;
        }

        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            enemySpriteRends[i].enabled = false;
        }

        DeactivatePS();
        if (portalAnim != null)
            portalAnim.SetBool("Close", true);

        gameObject.SendMessage("OnSpawnItemDrops");
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }

    public void ActivatePS()
    {
        for (int i = 0; i < portalPSs.Length; i++)
        {
            portalPSs[i].Play();
        }
    }
    public void DeactivatePS()
    {
        for (int i = 0; i < portalPSs.Length; i++)
        {
            portalPSs[i].Stop();
        }
    }

    public void StartAppear(float initialWait)
    {
        StartCoroutine(Appear(initialWait));
    }

    IEnumerator Appear(float initialWait)
    {
        yield return null;

        if (portalAnim != null)
            portalAnim.gameObject.SetActive(true);
        ActivatePS();

        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            enemySpriteRends[i].enabled = false;
        }

        yield return new WaitForSeconds(initialWait);

        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            enemySpriteRends[i].enabled = true;
        }

        WaitForSeconds wait = new WaitForSeconds(1f / frameRate);
        float t = 0;
        while (t < dissolveRate)
        {
            for (int i = 0; i < enemySpriteRends.Length; i++)
            {
                Color color = enemySpriteRends[i].color;
                color.a = Mathf.Lerp(0.7f, 1f, (t / dissolveRate) * (t / dissolveRate));
                enemySpriteRends[i].color = color;
            }
            yield return wait;
            t += 1f / frameRate;
        }

        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            Color color = enemySpriteRends[i].color;
            color.a = 1;
            enemySpriteRends[i].color = color;
        }

        DeactivatePS();
        if (portalAnim != null)
            portalAnim.SetBool("Close", true);

        yield return new WaitForSeconds(0.4f);

        if (portalAnim != null)
        {
            portalAnim.SetBool("Close", false);   
            portalAnim.gameObject.SetActive(false);
        }
    }
}
