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
        if(portalAnim != null)
            portalAnim.gameObject.SetActive(true);
        ActivatePS();

        yield return new WaitForSeconds(0.4f);

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
            Color color = enemySpriteRends[i].color;
            color.a = .5f;
            enemySpriteRends[i].color = color;
        }

        DeactivatePS();
        if (portalAnim != null)
            portalAnim.SetTrigger("Close");

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
}
