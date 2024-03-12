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
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(20f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Heal(20f);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (canGetHit)
        {
            currentHealth -= Mathf.Abs(amount);
            print("Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                if (gameObject.tag.Equals("Player"))
                {
                    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
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
