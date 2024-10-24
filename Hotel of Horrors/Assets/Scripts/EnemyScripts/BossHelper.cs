using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHelper : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem particles;
    [SerializeField] AnimationCurve dissolveCurve;
    [SerializeField] float dissolveTime = 2;
    [SerializeField] int frameRate = 24;

    [Header("Memento")]
    [SerializeField] [Range(0,1f)] float mementoSpawnThreshold = 0.5f;
    [SerializeField] MementoInfo mementoInfo;
    [SerializeField] int emotionalEnergyWorth = 500;
    [SerializeField] GameObject holderPrefab;
    [SerializeField] float dropRadius = 0.2f;
    [SerializeField] float startHeight = 0.2f;
    [SerializeField] float upwardsVelocity = 0.2f;
    [SerializeField] Vector2 launchDir = new Vector2(0, 1);

    public void BossDeathEffects()
    {
        StartCoroutine(BossDeath());
    }

    IEnumerator BossDeath()
    {
        bool spawnedMemento = false;

        particles.Play();

        float waitAmount = 1f / frameRate;
        WaitForSeconds wait = new WaitForSeconds(waitAmount);

        Color color = spriteRenderer.color;
        color.a = 1;
        spriteRenderer.color = color;
        float t = 0;
        while (t < dissolveTime)
        {
            color.a = Mathf.Lerp(1f, 0.7f, dissolveCurve.Evaluate(t / dissolveTime));
            spriteRenderer.color = color;
            yield return wait;
            t += waitAmount;

            if(!spawnedMemento && (t / dissolveTime) > mementoSpawnThreshold)
            {
                SpawnMemento();
                spawnedMemento = true;
            }

            if(t / dissolveTime > 0.65f) particles.Stop();
        }

        particles.Stop();

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    protected virtual void SpawnMemento()
    {
        GameObject go = Instantiate(holderPrefab, transform.position, Quaternion.identity);
        MementoData data = go.GetComponent<MementoData>();
        if (data) data.SetItemData(mementoInfo, 1, emotionalEnergyWorth);
        CustomRigidbody2D rb = go.GetComponent<CustomRigidbody2D>();
        if (rb)
        {
            rb.Initialize(startHeight, UnityEngine.Random.Range(0f, upwardsVelocity));
            rb.AddForce(launchDir.normalized * dropRadius * 2, ForceMode2D.Impulse);
        }
    }
}
