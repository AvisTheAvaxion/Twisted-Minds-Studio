using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] enemySpriteRends;
    [SerializeField] float dissolveRate = 0.5f;
    [SerializeField] int frameRate = 24;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < enemySpriteRends.Length; i++)
        {
            Color color = enemySpriteRends[i].color;
            color.a = 1;
            enemySpriteRends[i].color = color;
        }
    }

    public void StartDissolve(float initialWait)
    {
        StartCoroutine(Dissolve(initialWait));
    }

    IEnumerator Dissolve(float initialWait)
    {
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

        gameObject.SendMessage("OnSpawnItemDrops");
        yield return null;
        Destroy(gameObject);
    }
}
