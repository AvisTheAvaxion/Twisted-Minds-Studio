using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(StatsController))]
public class PlayerHealth : MonoBehaviour, IHealth
{
    StatsController stats;

    [SerializeField] PlayerMovement movement;
    [SerializeField] Slider healthBar;
    [SerializeField] float iFramesTime;
    [SerializeField] float stunTime;
    bool canGetHit = true;

    private void Start()
    {
        stats = GetComponent<StatsController>();

        if (healthBar) healthBar.value = 1;
    }

    public void TakeDamage(float amount, Effect effect = null)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            stats.TakeDamage(amount);
            //print("Health: " + currentHealth);

            if (effect != null && Random.Range(0, 1f) <= effect.chanceToInflictEffect) stats.AddEffect(effect);

            if (healthBar) healthBar.value = stats.GetHealthValue01();

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

            if (healthBar) healthBar.value = stats.GetHealthValue01();

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

        if (healthBar) healthBar.value = stats.GetHealthValue01();
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
} 
