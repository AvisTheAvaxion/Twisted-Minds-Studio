using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] float maxHealth;
    [SerializeField] float iFramesTime;
    [SerializeField] float stunTime;
    float currentHealth;
    bool canGetHit = true;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            currentHealth -= Mathf.Abs(amount);
            print("Health: " + currentHealth);

            if (currentHealth <= 0)
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

    public void TakeDamage(float amount, float stunLength)
    {
        //first checking if they have iframes
        if (canGetHit)
        {
            currentHealth -= Mathf.Abs(amount);
            print("Health: " + currentHealth);

            if (currentHealth <= 0)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if an enemy tocuhes the player, the player takes the damage
       /* if(this.gameObject.tag.Equals("Player") && collision.gameObject.tag.Equals("Enemy"))
        {
            TakeDamage(1f);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //handles bullet collision
        /*if((collision.gameObject.tag.Equals("PlayerBullet") && this.gameObject.tag.Equals("Enemy")) ||( collision.gameObject.tag.Equals("EnemyBullet") && this.gameObject.tag.Equals("Player")))
        {
            TakeDamage(2f);
        }*/
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.Abs(amount);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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
