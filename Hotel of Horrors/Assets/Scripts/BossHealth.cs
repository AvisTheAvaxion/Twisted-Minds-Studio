using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatsController))]
public class BossHealth : MonoBehaviour, IHealth
{
    public StatsController stats { get; private set; }
    [SerializeField] bool debug;
    [SerializeField] float iFrameTime = 0.5f;
    [SerializeField] bool healOverTime = false;
    [SerializeField] float timeBtwHeals = 5f;
    [SerializeField] float flashColorLength = 0.2f;
    [SerializeField] FlashColor flashColor;
    [SerializeField] BossGUI bossGUI;

    bool canTakeDamage;

    float timer = 0;
    private void Start()
    {
        stats = GetComponent<StatsController>();

        if (bossGUI == null) bossGUI = FindObjectOfType<BossGUI>();

        bossGUI.Boss_healthBar.value = 1;
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
        if (!canTakeDamage) return false;

        stats.TakeDamage(amount, effect);

        bossGUI.Boss_healthBar.value = stats.GetHealthValue01();
        if (flashColor != null) flashColor.Flash(flashColorLength);

        if (debug) print("Health: " + stats.GetHealthValue());

        if (stats.GetHealthValue() <= 0)
        {
            transform.SendMessage("OnDeath");
            //Destroy(gameObject);
        }else
        {
            canTakeDamage = false;
            StartCoroutine(IFrame());
        }
        return true;
    }

    public bool TakeDamage(float amount, float stunLength, Effect effect = null)
    {
        throw new System.NotImplementedException();
    }

    public StatsController.Effector InflictEffect(Effect effect)
    {
        return stats.AddEffect(effect);
    }

    public void UpdateHealth()
    {
       bossGUI.Boss_healthBar.value = stats.GetHealthValue01();

        if (stats.GetHealthValue() <= 0)
        {
            transform.SendMessage("OnDeath");
            //Destroy(gameObject);
        }
    }

    public void ShowHealthBar()
    {
        bossGUI.Boss_healthBar.gameObject.SetActive(true);
    }
    public void HideHealthBar()
    {
        bossGUI.Boss_healthBar.gameObject.SetActive(false);
    }

    public void Knockback(Vector3 dir, float strength)
    {
    }

    IEnumerator IFrame()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(iFrameTime);
        canTakeDamage = true;
    }
}
