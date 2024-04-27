using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballHelmetAbility : SpecialAbility
{
    public override void Use(AttackController controller)
    {
        this.controller = controller;
    }
}
