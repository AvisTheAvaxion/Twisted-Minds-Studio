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
        spawnPoint.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)spawnPoint.position).normalized) * spawnPoint.rotation;
        GameObject go = Instantiate(arc, spawnPoint.position, spawnPoint.rotation);
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb) rb.AddForce(go.transform.up * launchForce, ForceMode2D.Impulse);
        controller.ShakeCamera(cameraShakeFrequency, duration);
        Destroy(go, maxLifeTime);
        isAttacking = false;
    }
}
