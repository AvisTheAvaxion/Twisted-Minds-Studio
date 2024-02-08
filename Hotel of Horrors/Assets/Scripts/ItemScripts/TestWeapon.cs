using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : Weapon
{
    public override void Attack()
    {
        Debug.Log($"Weapon {GetName()} Used");
    }

    public override void WeaponSkill()
    {
        Debug.Log($"WEAPON SKILL {GetName()} USED");
    }
}
