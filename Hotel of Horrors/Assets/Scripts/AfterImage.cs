using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem afterImageEffect;

    [Header("Settings")]
    [SerializeField] float trailLength = 0.4f;
    [SerializeField] int density = 10;
    bool emitOverVelocity = false;

    bool playEffect;

    Coroutine effect;

    public void StartEffect(float length)
    {
        if (effect != null)
            StopCoroutine(effect);

        effect = StartCoroutine(AfterImageEffect(length));
    }

    public void StartEffect()
    {
        if (effect != null)
            StopCoroutine(effect);

        playEffect = true;
        effect = StartCoroutine(AfterImageEffect());
    }

    public void StopEffect()
    {
        playEffect = false;
    }

    IEnumerator AfterImageEffect(float length)
    {
        ParticleSystem.MainModule afterImageMain = afterImageEffect.main;
        ParticleSystem.EmissionModule afterImageEmission = afterImageEffect.emission;
        afterImageMain.startLifetime = trailLength;

        /*if (emitOverVelocity)
        {
            afterImageEmission.rateOverTime = 0;
            afterImageEmission.rateOverDistance = density;
        }
        else
        {
            afterImageEmission.rateOverTime = density;
            afterImageEmission.rateOverDistance = 0;
        }*/

        afterImageEmission.rateOverTime = density;
        afterImageEmission.rateOverDistance = 0;

        var ts = afterImageEffect.textureSheetAnimation;
        ts.SetSprite(0, spriteRenderer.sprite);

        afterImageEffect.Play();

        float t = 0;
        while(t < length)
        {
            ts.SetSprite(0, spriteRenderer.sprite);
            yield return null;
            t += Time.deltaTime;
        }

        afterImageEffect.Stop();
    }

    IEnumerator AfterImageEffect()
    {
        ParticleSystem.MainModule afterImageMain = afterImageEffect.main;
        ParticleSystem.EmissionModule afterImageEmission = afterImageEffect.emission;
        afterImageMain.startLifetime = trailLength;

        /*if (emitOverVelocity)
        {
            afterImageEmission.rateOverTime = 0;
            afterImageEmission.rateOverDistance = density;
        }
        else
        {
            afterImageEmission.rateOverTime = density;
            afterImageEmission.rateOverDistance = 0;
        }*/

        afterImageEmission.rateOverTime = density;
        afterImageEmission.rateOverDistance = 0;

        var ts = afterImageEffect.textureSheetAnimation;
        ts.SetSprite(0, spriteRenderer.sprite);

        afterImageEffect.Play();

        while (playEffect)
        {
            ts.SetSprite(0, spriteRenderer.sprite);
            yield return null;
        }

        afterImageEffect.Stop();
    }
}