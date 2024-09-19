using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatsController))]
public class EnemyHealth : MonoBehaviour, IHealth
{
    public StatsController stats { get; private set; }
    [SerializeField] bool debug;
    [SerializeField] float iFrameTime = 0.5f;
    [SerializeField] bool healOverTime = false;
    [SerializeField] float timeBtwHeals = 5f;
    [SerializeField] float flashColorLength = 0.2f;
    [SerializeField] FlashColor flashColor;
    [SerializeField] EnemyStateMachine enemyMovement;

    bool canTakeDamage;

    float timer = 0;
    private void Start()
    {
        stats = GetComponent<StatsController>();
        if (enemyMovement == null) enemyMovement = GetComponent<EnemyStateMachine>();

        canTakeDamage = true;
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

    public bool TakeDamage(float amount, Effect effect = null)
    {
        if (!canTakeDamage || enemyMovement.CurrentState == EnemyStateMachine.States.Death) return false;

        stats.TakeDamage(amount, effect);

        if(debug) print("Health: " + stats.GetHealthValue());

        if (flashColor != null) flashColor.Flash(flashColorLength);

        if (stats.GetHealthValue() <= 0)
        {
            transform.SendMessage("OnDeath");
            //Destroy(gameObject);
        } else
        {
            canTakeDamage = false;
            StartCoroutine(IFrame());
        }
        return true;
    }

    public bool TakeDamage(float amount, float stunLength, Effect effect = null)
    {
        return true;
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

    IEnumerator IFrame()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(iFrameTime);
        canTakeDamage = true;
    }
}
