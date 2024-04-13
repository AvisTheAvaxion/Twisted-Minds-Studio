using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatsController))]
public class EnemyHealth : MonoBehaviour, IHealth
{
    public StatsController stats { get; private set; }
    [SerializeField] bool debug;
    [SerializeField] bool healOverTime = false;
    [SerializeField] float timeBtwHeals = 5f;

    float timer = 0;
    private void Start()
    {
        stats = GetComponent<StatsController>();
    }
    private void Update()
    {
        if (healOverTime)
        {
            if (timer <= 0)
            {
                Heal(stats.GetCurrentValue(Stat.StatType.regeneration));

                timer += timeBtwHeals;
            }

            timer -= Time.deltaTime;
        }
    }

    public void Heal(float amount)
    {
        stats.Heal(amount);
    }

    public void TakeDamage(float amount, Effect effect = null)
    {
        stats.TakeDamage(amount, effect);

        if(debug) print("Health: " + stats.GetHealthValue());

        if (stats.GetHealthValue() <= 0)
        {
            transform.SendMessage("OnDeath");
            //Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount, float stunLength, Effect effect = null)
    {
        throw new System.NotImplementedException();
    }

    public StatsController.Effector InflictEffect(Effect effect)
    {
        return stats.AddEffect(effect);
    }
}
