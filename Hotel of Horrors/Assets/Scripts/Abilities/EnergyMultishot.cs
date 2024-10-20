using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicShooter))]
public class EnergyMultishot : PlayerAbility
{
    [Header("Multishot Settings")]
    [SerializeField] BasicShooter shooter;

    public override void Use(ActionController controller, Ability ability)
    {
        this.ability = ability;
        this.controller = controller;

        cooldown = ability.cooldown;
        duration = ability.duration;

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.25f);
        isAttacking = false;
        shooter.Attack(ability, controller.CrosshairPosition);
    }
}
