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
}
