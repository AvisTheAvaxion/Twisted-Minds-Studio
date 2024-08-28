using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    AbilityInfo info;

    public float cooldown { get; private set; }
    public float duration { get; private set; }
    public float damage { get; private set; }
    public float size { get; private set; }
    public float range { get; private set; }

    public int numberOfProjectiles { get; private set; }
    public float deflectionResistance { get; private set; }
    public int maxTargets { get; private set; }
    public bool goThroughWalls { get; private set; }

    public AbilityInfo GetInfo()
    {
        return info;
    }

    public Ability(AbilityInfo info)
    {
        this.info = info;
    }
}
