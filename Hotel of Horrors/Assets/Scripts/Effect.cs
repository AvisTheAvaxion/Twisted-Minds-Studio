using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public float chanceToInflictEffect { get; private set; }
    public EffectInfo info { get; private set; }
    public Effect(EffectInfo info, float chanceToInflictEffect)
    {
        this.info = info;
        this.chanceToInflictEffect = chanceToInflictEffect;
    }
}
