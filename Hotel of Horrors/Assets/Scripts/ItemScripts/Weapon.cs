using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    int currentLevel;
    WeaponInfo info;

    public WeaponInfo GetInfo()
    {
        return info;
    }

    public Weapon(WeaponInfo info)
    {
        this.info = info;
        currentLevel = info.GetBaseLevel();
    }

    public void Upgrade(Weapon otherWeapon)
    {

    }
}
