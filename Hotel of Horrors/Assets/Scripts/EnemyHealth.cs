using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    [SerializeField] bool debug;
    [SerializeField] float maxHealth;
    public float currentHealth { get; private set; }

    [SerializeField] bool healOverTime = false;
    [SerializeField] float timeBtwHeals = 5f;
    [SerializeField] float rateOfHealing = 5f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    float timer = 0;
    private void Update()
    {
        if (healOverTime)
        {
            if (timer <= 0)
            {
                Heal(rateOfHealing);

                timer += timeBtwHeals;
            }

            timer -= Time.deltaTime;
        }
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.Abs(amount);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= Mathf.Abs(amount);

        if(debug) print("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount, float stunLength)
    {
        throw new System.NotImplementedException();
    }
}
