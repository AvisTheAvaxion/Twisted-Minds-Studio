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
    [SerializeField] float flashColorLength = 0.2f;
    [SerializeField] FlashColor flashColor;
    [SerializeField] EnemyStateMachine enemyMovement;

    float timer = 0;
    private void Start()
    {
        stats = GetComponent<StatsController>();
        if (enemyMovement == null) enemyMovement = GetComponent<EnemyStateMachine>();
    }
    private void Update()
    {
        if (healOverTime)
        {
            if (timer <= 0)
            {
                Heal(stats.GetCurrentValue(Stat.StatType.Regeneration));

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

        if (flashColor != null) flashColor.Flash(flashColorLength);

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

    public void UpdateHealth()
    {
        if (stats.GetHealthValue() <= 0)
        {
            transform.SendMessage("OnDeath");
            //Destroy(gameObject);
        }
    }

    public void Knockback(Vector3 dir, float strength)
    {
        if (enemyMovement != null) enemyMovement.Knockback(dir, strength);
    }
}
