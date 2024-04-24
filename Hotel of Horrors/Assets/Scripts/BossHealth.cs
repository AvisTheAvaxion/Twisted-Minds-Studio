using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour, IHealth
{
    public StatsController stats { get; private set; }
    [SerializeField] bool debug;
    [SerializeField] bool healOverTime = false;
    [SerializeField] float timeBtwHeals = 5f;
    [SerializeField] bool isBoss;
    [SerializeField] UIDisplayContainer uiDisplay;

    float timer = 0;
    private void Start()
    {
        stats = GetComponent<StatsController>();

        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        uiDisplay.Boss_healthBar.value = 1;
    }
    private void Update()
    {
        if (isBoss)
        {

        }

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

        if (isBoss) uiDisplay.Boss_healthBar.value = stats.GetHealthValue01();

        if (debug) print("Health: " + stats.GetHealthValue());

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
        if (isBoss) uiDisplay.Boss_healthBar.value = stats.GetHealthValue01();

        if (stats.GetHealthValue() <= 0)
        {
            transform.SendMessage("OnDeath");
            //Destroy(gameObject);
        }
    }

    public void ShowHealthBar()
    {
        if (isBoss) uiDisplay.Boss_healthBar.gameObject.SetActive(true);
    }
    public void HideHealthBar()
    {
        if (isBoss) uiDisplay.Boss_healthBar.gameObject.SetActive(false);
    }

    public void Knockback(Vector3 dir, float strength)
    {
    }
}
