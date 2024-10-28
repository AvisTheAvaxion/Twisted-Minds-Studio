using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicShooter))]
public class EnergyMultishot : PlayerAbility
{
    [Header("Multishot Settings")]
    [SerializeField] BasicShooter shooter;
    [SerializeField] float shootDelay = 0.25f;

    public override void CancelAbility()
    {
    }

    public override void Use(ActionController controller, Ability ability)
    {
        this.ability = ability;
        this.controller = controller;

        cooldown = ability.cooldown;
        duration = ability.duration;

        StartCoroutine(Attack());
    }

    protected IEnumerator Attack()
    {
        isAttacking = true;
        shooter.Attack(ability, shootDelay, controller.CrosshairPosition);
        yield return new WaitForSeconds(shootDelay);
        controller.ShakeCamera(cameraShakeFrequency, Mathf.Max(0.5f, duration), false);
        isAttacking = false;
    }
}
