using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalpelAbility : WeaponAbility
{
    [SerializeField] float spawnDistance = 0.25f;
    [SerializeField] float forceDelay = 0.25f;
    [SerializeField] float launchForce = 4f;
    [SerializeField] GameObject arc;

    public override void Use(ActionController controller, Weapon weapon)
    {
        this.weapon = weapon;
        this.controller = controller;

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        //Leaves room for start animation
        Vector2 dir = (controller.CrosshairPosition - (Vector2)transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, dir) * transform.rotation;
        GameObject go = Instantiate(arc, transform.position + (Vector3)dir * spawnDistance, transform.rotation);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj)
        {
            proj.Initialize(weapon, forceDelay);
            proj.StartApplyForce(go.transform.up * launchForce, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(forceDelay);

        controller.ShakeCamera(cameraShakeFrequency, duration, false);
        isAttacking = false;
    }
}
