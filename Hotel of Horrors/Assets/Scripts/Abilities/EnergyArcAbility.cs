using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyArcAbility : PlayerAbility
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] float launchForce;
    [SerializeField] float maxLifeTime = 10f;
    [SerializeField] float forceDelay = 0.25f;
    [SerializeField] GameObject arc;

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
        //Leaves room for start animation
        transform.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)transform.position).normalized) * transform.rotation;
        GameObject go = Instantiate(arc, spawnPoint.position, spawnPoint.rotation);
        MultiProjectile multi = go.GetComponent<MultiProjectile>();
        yield return new WaitForSeconds(forceDelay);
        if (multi)
        {
            multi.Initialize(ability);
            multi.Launch(launchForce, go.transform.up);
        }
        controller.ShakeCamera(cameraShakeFrequency, duration, false);
        //Destroy(go, maxLifeTime);
        isAttacking = false;
    }
}
