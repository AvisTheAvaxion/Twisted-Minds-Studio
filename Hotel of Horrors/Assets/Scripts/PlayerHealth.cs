using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(StatsController))]
public class PlayerHealth : MonoBehaviour, IHealth
{
    StatsController stats;

    HeartsController heartsController;

    [SerializeField] PlayerMovement movement;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] FlashColor flashColor;
    [SerializeField] PlayerGUI playerGUI;
    [SerializeField] float iFramesTime;
    [SerializeField] float stunTime;
    [SerializeField] float flashColorLength = 0.2f;

    [SerializeField] PlayerAudio playerAudio;


    bool canGetHit = true;

    private void Start()
    {
        stats = GetComponent<StatsController>();

        playerGUI = FindObjectOfType<PlayerGUI>();


        if (playerGUI) heartsController = playerGUI.GetHeartsController();
        if (heartsController) heartsController.Init(stats.GetHealth());
    }

    public bool TakeDamage(float amount, Effect effect = null)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            stats.TakeDamage(amount);
            //print("Health: " + currentHealth);

            if(playerAudio)
                playerAudio.Damage();

            if (effect != null && Random.Range(0, 1f) <= effect.chanceToInflictEffect) stats.AddEffect(effect);

            if (heartsController) heartsController.AdjustHearts(stats.GetHealth());
            if (cameraShake != null) cameraShake.ShakeCamera(amount / 15f);
            if (flashColor != null) flashColor.Flash(flashColorLength);

            if (stats.GetHealthValue() <= 0)
            {
                //loads death scene
                SceneManager.LoadScene("Death Screen");
            }

            GiveIFrames(iFramesTime);
            Stun(stunTime);

            return true;
        }
        return false;
    }

    public bool TakeDamage(float amount, float stunLength, Effect effect = null)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            stats.TakeDamage(amount);
            print("Health: " + stats.GetHealthValue());

            if (effect != null) stats.AddEffect(effect);

            if (heartsController) heartsController.AdjustHearts(stats.GetHealth());
            if (cameraShake != null) cameraShake.ShakeCamera(amount / 15f);
            if (flashColor != null) flashColor.Flash(flashColorLength);

            if (stats.GetHealthValue() <= 0)
            {
                //loads death scene
                SceneManager.LoadScene("Death Screen");
            }

            GiveIFrames(iFramesTime);
            Stun(stunLength);

            return true;
        }
        return false;
    }

    public void Heal(float amount)
    {
        stats.Heal(amount);

        if (heartsController) heartsController.AdjustHearts(stats.GetHealth());
    }

    public StatsController.Effector InflictEffect(Effect effect)
    {
        return stats.AddEffect(effect);
    }

    public void GiveIFrames(float length)
    {
        StartCoroutine(GiveIFramesSequence(length));
    }
    IEnumerator GiveIFramesSequence(float length)
    {
        canGetHit = false;

        yield return new WaitForSeconds(length);

        canGetHit = true;
    }

    public void Stun(float length)
    {
        movement.Stun(length);
    }

    public void UpdateHealth()
    {
        if (heartsController) heartsController.AdjustHearts(stats.GetHealth());

        if (stats.GetHealthValue() <= 0)
        {
            //loads death scene
            SceneManager.LoadScene("Death Screen");
        }
    }

    public void Knockback(Vector3 dir, float strength)
    {
        movement.Knockback(dir, strength);
    }
} 
