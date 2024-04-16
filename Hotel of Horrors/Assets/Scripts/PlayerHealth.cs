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
    [SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] float iFramesTime;
    [SerializeField] float stunTime;
    bool canGetHit = true;

    private void Start()
    {
        stats = GetComponent<StatsController>();

        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (uiDisplay) heartsController = uiDisplay.HeartsController;
        if (heartsController) heartsController.Init(stats.GetHealth());
    }

    public void TakeDamage(float amount, Effect effect = null)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            stats.TakeDamage(amount);
            //print("Health: " + currentHealth);

            if (effect != null && Random.Range(0, 1f) <= effect.chanceToInflictEffect) stats.AddEffect(effect);

            if (heartsController) heartsController.AdjustHearts(stats.GetHealth());

            if (stats.GetHealthValue() <= 0)
            {
                if (gameObject.tag.Equals("Player"))
                {
                    //loads death scene
                    SceneManager.LoadScene("Death Screen");
                }
            }

            GiveIFrames(iFramesTime);
            Stun(stunTime);
        }
    }

    public void TakeDamage(float amount, float stunLength, Effect effect = null)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            stats.TakeDamage(amount);
            print("Health: " + stats.GetHealthValue());

            if (effect != null) stats.AddEffect(effect);

            if (heartsController) heartsController.AdjustHearts(stats.GetHealth());

            if (stats.GetHealthValue() <= 0)
            {
                if (gameObject.tag.Equals("Player"))
                {
                    //loads death scene
                    SceneManager.LoadScene("Death Screen");
                }
            }

            GiveIFrames(iFramesTime);
            Stun(stunLength);
        }
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
} 
