using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour
{
    [SerializeField] PlayerMovement movementScript;
    [SerializeField] float maxHealth;
    [SerializeField] float iFramesTime;
    [SerializeField] float stunTime;
    float currentHealth;
    bool canGetHit = true;


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    TakeDamage(20f);
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Heal(20f);
        //}
    }

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
                else
                {
                    Destroy(gameObject);
                }
            }
                

            StartCoroutine(GiveIFrames());
            StartCoroutine(Stun());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if an enemy tocuhes the player, the player takes the damage
        if(this.gameObject.tag.Equals("Player") && collision.gameObject.tag.Equals("Enemy"))
        {
            TakeDamage(1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //handles bullet collision
        if((collision.gameObject.tag.Equals("PlayerBullet") && this.gameObject.tag.Equals("Enemy")) ||( collision.gameObject.tag.Equals("EnemyBullet") && this.gameObject.tag.Equals("Player")))
        {
            TakeDamage(2f);
        }
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.Abs(amount);
    }

    IEnumerator GiveIFrames()
    {
        canGetHit = false;

        yield return new WaitForSeconds(iFramesTime);

        canGetHit = true;
    }

    IEnumerator Stun()
    {
        movementScript.canMove = false;

        yield return new WaitForSeconds(iFramesTime);

        movementScript.canMove = true;
    }
}
