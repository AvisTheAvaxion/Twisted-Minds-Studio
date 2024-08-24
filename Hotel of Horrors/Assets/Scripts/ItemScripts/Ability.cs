using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    AbilityInfo info;

    public AbilityInfo GetInfo()
    {
        return info;
    }

    public Ability(AbilityInfo info)
    {
        this.info = info;
    }
}
