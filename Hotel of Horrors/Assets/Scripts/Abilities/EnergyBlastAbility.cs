using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlastAbility : PlayerAbility
{
    [SerializeField] GameObject blast;
    [SerializeField] Transform spawnPoint;

    public override void Use(AttackController controller)
    {
        this.controller = controller;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        //Entrance animation
        spawnPoint.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)spawnPoint.position).normalized) * spawnPoint.rotation;
        GameObject go = Instantiate(blast, spawnPoint.position, spawnPoint.rotation, transform);
        controller.ShakeCamera(cameraShakeFrequency, duration);
        yield return new WaitForSeconds(duration);
        //Exit animation
        Destroy(go);
        isAttacking = false;
    }
}
