using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effect")]
public class EffectInfo : ScriptableObject
{
    public enum EffectMode
    {
        Duration, Overtime, Permenant
    }
    public enum EffectType
    {
        Buff, Debuff
    }

    [System.Serializable]
    public struct StatEffect
    {
        [SerializeField] Stat.StatType statType;
        [SerializeField] EffectType effectType;
        [SerializeField] float strength;
        [SerializeField] bool isPercentage;

        public EffectType EffectType { get => effectType; }
        public Stat.StatType StatType { get => statType; }
        public float Strength { get => strength; }
        public bool IsPercentage { get => isPercentage; }
    }

    [SerializeField] string name = "New Effect";
    [SerializeField] bool hasEfficacyLevel = false;
    [SerializeField] string description = "New effect goes brrr";
    [SerializeField] GameObject particleEffect;
    [SerializeField] Sprite displaySprite;
    [Header("Settings")]
    [SerializeField] EffectMode mode = EffectMode.Duration;
    [SerializeField] StatEffect[] statEffects;
    [SerializeField] float duration = 5;
    [SerializeField] float interval = 1;

    public string Name { get => name; }
    public string Description { get => description; }
    public GameObject ParticleEffect { get => particleEffect; }
    public Sprite DisplaySprite { get => displaySprite; }
    public EffectMode Mode { get => mode; }
    public StatEffect[] StatEffects { get => statEffects; }
    public float Duration { get => duration; }
    public float Interval { get => interval; }
    public bool HasEfficacyLevel { get => hasEfficacyLevel; }

    public override string ToString()
    {
        string str = $"<b>{name}</b>\n";
        if(mode == EffectMode.Permenant)
        {
            for (int e = 0; e < statEffects.Length; e++)
            {
                StatEffect statEffect = statEffects[e];
                if (statEffect.StatType == Stat.StatType.Health)
                {
                    if (statEffect.EffectType == EffectInfo.EffectType.Buff)
                        str += $"Heals {statEffect.Strength}\n";
                    else
                        str += $"Damages for {statEffect.Strength}\n";
                }
                else
                {
                    if (statEffect.EffectType == EffectInfo.EffectType.Buff)
                        str += $"{(statEffect.IsPercentage ? $"+{string.Format("{0:0.0}", statEffect.Strength * 100)}%" : $"+{string.Format("{0:0.0}", statEffect.Strength)}")} {statEffect.StatType.ToString()}\n";
                    else
                        str += $"{(statEffect.IsPercentage ? $"-{string.Format("{0:0.0}", statEffect.Strength * 100)}%" : $"-{string.Format("{0:0.0}", statEffect.Strength)}")} {statEffect.StatType.ToString()}\n";
                }
            }
        }
        else if (mode == EffectMode.Duration)
        {
            for (int e = 0; e < statEffects.Length; e++)
            {
                StatEffect statEffect = statEffects[e];
                int minutes = Mathf.RoundToInt(duration / 60);
                int seconds = Mathf.RoundToInt(duration % 60);
                string time = "";
                if (minutes > 0)
                {
                    time = minutes.ToString() + " min";
                }
                else
                {
                    time = seconds.ToString() + "s";
                }
                if (statEffect.EffectType == EffectInfo.EffectType.Buff)
                {
                    str += $"{(statEffect.IsPercentage ? $"+{string.Format("{0:0.0}", statEffect.Strength * 100)}%" : $"+{string.Format("{0:0.0}", statEffect.Strength)}")} {statEffect.StatType.ToString()} for {time}\n";
                }
                else
                {
                    str += $"{(statEffect.IsPercentage ? $"-{string.Format("{0:0.0}", statEffect.Strength * 100)}%" : $"-{string.Format("{0:0.0}", statEffect.Strength)}")} {statEffect.StatType.ToString()} for {time}\n";
                }
            }
        }
        else if (mode == EffectMode.Overtime)
        {
            for (int e = 0; e < statEffects.Length; e++)
            {
                StatEffect statEffect = statEffects[e];
                float strength = Mathf.Round(statEffect.Strength * (duration / interval));
                int minutes = Mathf.RoundToInt(duration / 60);
                int seconds = Mathf.RoundToInt(duration % 60);
                string time = "";
                if (minutes > 0)
                {
                    time = minutes.ToString() + " min";
                } else
                {
                    time = seconds.ToString() + "s";
                }
                if (statEffect.EffectType == EffectInfo.EffectType.Buff)
                {
                    str += $"{(statEffect.IsPercentage ? $"+{string.Format("{0:0.0}", strength * 100)}%" : $"+{string.Format("{0:0.0}", strength)}")} {statEffect.StatType.ToString()} over {time}\n";
                }
                else
                {
                    str += $"{(statEffect.IsPercentage ? $"-{string.Format("{0:0.0}", strength * 100)}%" : $"-{string.Format("{0:0.0}", strength)}")} {statEffect.StatType.ToString()} over {time}\n";
                }
            }
        }
        return str;
    }
}
