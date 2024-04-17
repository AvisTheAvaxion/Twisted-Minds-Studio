using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [SerializeField] bool debug;
    [Header("Player Specific Settings")]
    [SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] bool isPlayer = false;
    [Header("Settings")]
    [SerializeField] Stat health = new Stat();
    [SerializeField] Stat[] stats;

    Dictionary<Stat.StatType, Stat> statsDictionary;

    List<Effector> currentEffectors;

    // Start is called before the first frame update
    void Start()
    {
        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError($"UI display container script not assigned to {name} and not found in scene (located on canvas UI prefab");

        statsDictionary = new Dictionary<Stat.StatType, Stat>();
        statsDictionary.Add(health.Type, health);
        health.Reset();

        foreach (Stat stat in stats)
        {
            if (statsDictionary.ContainsKey(stat.Type)) Debug.LogError("Two stats of the same kind exist on " + name + ". Fix it");
            statsDictionary.Add(stat.Type, stat);

            stat.Reset();
        }

        currentEffectors = new List<Effector>();
    }

    public void ResetStat(Stat.StatType type)
    {
        Stat stat = GetStat(type);
        if(stat != null) stat.Reset();
    }

    public Stat GetStat(Stat.StatType type)
    {
        if (statsDictionary.ContainsKey(type))
            return statsDictionary[type];
        return null;
    }
    public float GetCurrentValue(Stat.StatType type)
    {
        if(statsDictionary.ContainsKey(type))
            return statsDictionary[type].CurrentValue;
        return 0;
    }
    /// <summary>
    /// Returns the current value of the stat corresponding to the given stat type in a range from 0 to 1. *Mainly used for sliders*
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetCurrentValue01(Stat.StatType type)
    {
        if (statsDictionary.ContainsKey(type))
            return statsDictionary[type].CurrentValue01;
        return 0;
    }

    public Stat GetHealth()
    {
        if (!statsDictionary.ContainsKey(Stat.StatType.Health)) Debug.LogError(name + " has no health stat. Why!?");
        return statsDictionary[Stat.StatType.Health];
    }
    public float GetHealthValue()
    {
        return GetHealth().CurrentValue;
    }
    /// <summary>
    /// Returns the current health value in a range from 0 to 1. *Mainly used for sliders*
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetHealthValue01()
    {
        return GetHealth().CurrentValue01;
    }

    /// <summary>
    /// Applies damage to the health stat
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="effect"></param>
    /// <returns>Returns the new health value after taking damage</returns>
    public float TakeDamage(float damage)
    {
        Stat health = GetHealth();
        health.RemoveValue(damage, false);
        return health.CurrentValue;
    }
    /// <summary>
    /// Applies damage to the health stat and applies an effect. *If the available effect events are needed, use AddEffect separately*
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="effect"></param>
    /// <returns>Returns the new health value after taking damage</returns>
    public float TakeDamage(float damage, Effect effect)
    {
        Stat health = GetHealth();
        health.RemoveValue(damage, false);
        if(effect != null && UnityEngine.Random.Range(0f, 1f) > effect.chanceToInflictEffect)
            AddEffect(effect);
        return health.CurrentValue;
    }

    public bool Heal(float amount)
    {
        Stat health = GetHealth();
        if (health.CurrentValue >= health.MaxValue) return false;

        health.AddValue(amount, false);
        return true;
    }

    /// <summary>
    /// Changes the stat value corresponding to the given stat type. *The removePercentage parameter ensures percentage based effects are removed properly*
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    /// <param name="isPercentage"></param>
    /// <param name="removePercentage"></param>
    /// <returns>Returns the new current value of the stat</returns>
    public float ChangeStatValue(Stat.StatType type, float amount, bool isPercentage, bool removePercentage = false)
    {
        if (!statsDictionary.ContainsKey(type)) return 0;

        Stat stat = statsDictionary[type];
        if (isPercentage && removePercentage) stat.RemoveValue(amount, isPercentage);
        else stat.AddValue(amount, isPercentage);
        return stat.CurrentValue;
    }
    
    /// <summary>
    /// Adds the given effect to this character
    /// </summary>
    /// <param name="effect"></param>
    /// <returns>(For over time effects)Returns the effector which contains the events to subscribe to. *Make sure to unsubscribe on onEndEffect*</returns>
    public Effector AddEffect(Effect effect)
    {
        if(effect.info != null && (effect.info.Mode == EffectInfo.EffectMode.Permenant))
        {
            foreach (EffectInfo.StatEffect statEffect in effect.info.StatEffects)
            {
                if (statEffect.EffectType == EffectInfo.EffectType.Buff)
                    ChangeStatValue(statEffect.StatType, statEffect.Strength, statEffect.IsPercentage);
                else
                    ChangeStatValue(statEffect.StatType, statEffect.IsPercentage ? statEffect.Strength : -statEffect.Strength, statEffect.IsPercentage, statEffect.IsPercentage);
                if (statEffect.StatType == Stat.StatType.Health) transform.SendMessage("UpdateHealth");
            }
            return null;
        }
        else if (effect.info != null && UnityEngine.Random.Range(0f, 1f) <= effect.chanceToInflictEffect)
        {
            if (debug) print("Add effect " + effect + " to " + name);

            for (int i = 0; i < currentEffectors.Count; i++)
            {
                if (currentEffectors[i].Effect.info.Name == effect.info.Name)
                {
                    if(isPlayer) Destroy(currentEffectors[i].EffectIcon.gameObject);
                    RemoveEffector(currentEffectors[i]);
                    break;
                }
            }

            Effector newEffector = null;
            if (isPlayer)
            {
                GameObject go = Instantiate(uiDisplay.EffectsIconPrefab, uiDisplay.EffectsIconHolder);
                EffectIcon effectIcon = go.GetComponent<EffectIcon>();
                newEffector = new Effector(effect, effectIcon);
            } else
                newEffector = new Effector(effect);

            currentEffectors.Add(newEffector);

            if (effect.info.Mode == EffectInfo.EffectMode.Overtime) newEffector.activeCoroutine = StartCoroutine(EffectOverTime(newEffector));
            else newEffector.activeCoroutine = StartCoroutine(EffectNormal(newEffector));

            return newEffector;
        } 
        else
            return null;
    }

    /// <summary>
    /// Removes the given effector from the list of current effectors and stop their effects if they haven't stopped already
    /// </summary>
    /// <param name="effector"></param>
    public void RemoveEffector(Effector effector)
    {
        if (effector.activeCoroutine != null)
            StopCoroutine(effector.activeCoroutine);
        if(isPlayer) Destroy(effector.EffectIcon.gameObject);
        currentEffectors.Remove(effector);
    }

    public IEnumerator EffectNormal(Effector effector)
    {
        if (debug) Debug.Log(effector.Effect.info.Name + " effect start");
        foreach (EffectInfo.StatEffect statEffect in effector.Effect.info.StatEffects)
        {
            if(statEffect.EffectType == EffectInfo.EffectType.Buff)
                ChangeStatValue(statEffect.StatType, statEffect.Strength, statEffect.IsPercentage);
            else
                ChangeStatValue(statEffect.StatType, statEffect.IsPercentage ? statEffect.Strength : -statEffect.Strength, statEffect.IsPercentage, statEffect.IsPercentage);
            if (statEffect.StatType == Stat.StatType.Health) transform.SendMessage("UpdateHealth");
        }

        WaitForSeconds wait = new WaitForSeconds(0.5f);
        float timer = 0;
        while(timer < effector.Effect.info.Duration)
        {
            timer += Time.deltaTime;
            if(isPlayer) effector.EffectIcon.UpdateDuration(effector.Effect.info.Duration - timer);
            yield return wait;
        }

        if (debug) Debug.Log(effector.Effect.info.Name + " effect ended");
        foreach (EffectInfo.StatEffect statEffect in effector.Effect.info.StatEffects)
        {
            if (statEffect.EffectType == EffectInfo.EffectType.Debuff)
                ChangeStatValue(statEffect.StatType, statEffect.Strength, statEffect.IsPercentage);
            else
                ChangeStatValue(statEffect.StatType, statEffect.IsPercentage ? statEffect.Strength : -statEffect.Strength, statEffect.IsPercentage, statEffect.IsPercentage);

            if (statEffect.StatType == Stat.StatType.Health) transform.SendMessage("UpdateHealth");
        }

        effector.onEffectEnd?.Invoke(this, null);

        RemoveEffector(effector);
    }

    public IEnumerator EffectOverTime(Effector effector)
    {
        Stat[] affectedStats = new Stat[effector.Effect.info.StatEffects.Length];
        float[] beforeValues = new float[effector.Effect.info.StatEffects.Length];
        float[] afterValues = new float[effector.Effect.info.StatEffects.Length];
        if (debug) Debug.Log(effector.Effect.info.Name + " effect start");
        foreach (EffectInfo.StatEffect statEffect in effector.Effect.info.StatEffects)
        {
            if (statEffect.EffectType == EffectInfo.EffectType.Buff)
                ChangeStatValue(statEffect.StatType, statEffect.Strength, statEffect.IsPercentage);
            else
                ChangeStatValue(statEffect.StatType, statEffect.IsPercentage ? statEffect.Strength : -statEffect.Strength, statEffect.IsPercentage, statEffect.IsPercentage);
            if (statEffect.StatType == Stat.StatType.Health) transform.SendMessage("UpdateHealth");
        }

        float timer = 0;
        float intervalTime = effector.Effect.info.Interval;
        while (timer < effector.Effect.info.Duration)
        {
            if (intervalTime <= 0)
            {
                if (debug) Debug.Log(effector.Effect.info.Name + " effect inflicted");
                for (int i = 0; i < effector.Effect.info.StatEffects.Length; i++)
                {
                    EffectInfo.StatEffect currentStatEffect = effector.Effect.info.StatEffects[i];
                    affectedStats[i] = GetStat(currentStatEffect.StatType);
                    beforeValues[i] = GetCurrentValue(currentStatEffect.StatType);
                    if (currentStatEffect.EffectType == EffectInfo.EffectType.Buff)
                        afterValues[i] = ChangeStatValue(currentStatEffect.StatType, currentStatEffect.Strength, currentStatEffect.IsPercentage);
                    else
                        afterValues[i] = ChangeStatValue(currentStatEffect.StatType,
                            currentStatEffect.IsPercentage ? currentStatEffect.Strength : -currentStatEffect.Strength,
                            currentStatEffect.IsPercentage, currentStatEffect.IsPercentage);

                    if (currentStatEffect.StatType == Stat.StatType.Health) transform.SendMessage("UpdateHealth");
                }
                intervalTime = effector.Effect.info.Interval;
                effector.onEffectInterval?.Invoke(this, new EffectEventArgs(affectedStats, beforeValues, afterValues, GetHealthValue()));
            }

            if(isPlayer) effector.EffectIcon.UpdateDuration(effector.Effect.info.Duration - timer);
            timer += Time.deltaTime;
            intervalTime -= Time.deltaTime;

            yield return null;
        }

        if (debug) Debug.Log(effector.Effect.info.Name + " effect ended");
        for (int i = 0; i < effector.Effect.info.StatEffects.Length; i++)
        {
            affectedStats[i] = GetStat(effector.Effect.info.StatEffects[i].StatType);
            afterValues[i] = beforeValues[i] = GetCurrentValue(effector.Effect.info.StatEffects[i].StatType);
        }
        effector.onEffectEnd?.Invoke(this, new EffectEventArgs(affectedStats, beforeValues, afterValues, GetHealthValue()));

        RemoveEffector(effector);
    }

    public class Effector
    {
        public EventHandler onEffectInterval;
        public EventHandler onEffectEnd;

        EffectIcon effectIcon;
        Effect effect;

        public EffectIcon EffectIcon { get => effectIcon; }
        public Effect Effect { get => effect; }

        public Coroutine activeCoroutine;

        public Effector(Effect effect, EffectIcon effectIcon)
        {
            this.effect = effect;
            this.effectIcon = effectIcon;

            this.effectIcon.SetEffect(this.effect.info);
        }
        public Effector(Effect effect)
        {
            this.effect = effect;
            this.effectIcon = null;
        }
    }
}

[System.Serializable]
public class Stat
{
    public enum StatType
    {
        Health, Regeneration, MeleeDamage, AttackSpeed, RangedDamage, Defense, MovementSpeed, Stamina, StaminaRegeneration
    }

    [SerializeField] StatType type;
    [SerializeField] float defaultValue;
    [SerializeField] bool hasMax;
    [SerializeField] float maxValue;

    float currentValue;
    public float CurrentValue { get => currentValue; }
    public float CurrentValue01 { get => hasMax ? currentValue / maxValue : currentValue / defaultValue; }
    public float MaxValue { get => maxValue; }
    public float DefaultValue { get => defaultValue; }
    public StatType Type { get => type; }

    public Stat()
    {
        type = StatType.Health;
        defaultValue = 25f;
        hasMax = true;
        maxValue = 25f;
    }

    public void AddValue(float strength, bool isPercentage)
    {
        if (isPercentage) currentValue *= strength;
        else currentValue += strength;

        if (hasMax && currentValue > maxValue) currentValue = maxValue;
    }

    public void RemoveValue(float strength, bool isPercentage)
    {
        if (isPercentage) currentValue /= strength;
        else currentValue -= strength;
    }

    public void Reset()
    {
        if (hasMax) currentValue = maxValue;
        else currentValue = defaultValue;
    }
    public void SetCurrentValue(float value)
    {
        currentValue = value;
        if (hasMax && currentValue > maxValue) currentValue = maxValue;
    }
}



class EffectEventArgs : EventArgs
{
    Stat[] affectedStats;
    float[] beforeValues;
    float[] afterValues;
    float healthValue;

    public EffectEventArgs(Stat[] affectedStats, float[] beforeValues, float[] afterValues, float healthValue)
    {
        this.affectedStats = affectedStats;
        this.beforeValues = beforeValues;
        this.afterValues = afterValues;
        this.healthValue = healthValue;
    }

    public Stat[] GetAffectedStats()
    {
        return affectedStats;
    }
    public float[] GetBeforeValues()
    {
        return beforeValues;
    }
    public float[] GetAfterValues()
    {
        return afterValues;
    }
    public float GetHealthValue()
    {
        return healthValue;
    }
}
