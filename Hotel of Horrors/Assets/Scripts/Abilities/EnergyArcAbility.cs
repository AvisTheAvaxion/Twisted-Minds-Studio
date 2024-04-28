using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyArcAbility : PlayerAbility
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] float launchForce;
    [SerializeField] float maxLifeTime = 10f;
    [SerializeField] GameObject arc;

    public override void Use(AttackController controller)
    {
        this.controller = controller;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        //Leaves room for start animation
        yield return null;
        transform.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)transform.position).normalized) * transform.rotation;
        GameObject go = Instantiate(arc, spawnPoint.position, spawnPoint.rotation);
        MultiProjectile multi = go.GetComponent<MultiProjectile>();
        if (multi) multi.Launch(launchForce, go.transform.up);
        //Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        //if (rb) rb.AddForce(go.transform.up * launchForce, ForceMode2D.Impulse);
        controller.ShakeCamera(cameraShakeFrequency, duration, false);
        Destroy(go, maxLifeTime);
        isAttacking = false;
    }
}
