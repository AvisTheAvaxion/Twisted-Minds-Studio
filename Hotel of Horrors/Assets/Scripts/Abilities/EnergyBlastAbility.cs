using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlastAbility : PlayerAbility
{
    [SerializeField] float maxRange = 3f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] BoxCollider2D collider2D;
    [SerializeField] LayerMask blockBlastMask;
    [SerializeField] Transform spawnPoint;

    Vector3 localPos;

    private void Start()
    {
        localPos = spawnPoint.localPosition;

    }

    public override void Use(AttackController controller)
    {
        this.controller = controller;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {

        isAttacking = true;
        //Entrance animation
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Vector3.zero);
        collider2D.enabled = true;
        transform.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)transform.position).normalized) * transform.rotation;
        spawnPoint.localPosition = localPos;
        controller.ShakeCamera(cameraShakeFrequency, duration, false);
        float timer = 0;
        while(timer < duration)
        {
            Vector2 dir = (controller.CrosshairPosition - (Vector2)transform.position).normalized;
            RaycastHit2D hitInfo = Physics2D.Raycast(spawnPoint.position, dir, maxRange, blockBlastMask);
            float distance = maxRange;
            if(hitInfo.collider != null)
            {
                distance = hitInfo.distance;
            }
            lineRenderer.SetPosition(1, Vector3.up * distance);
            collider2D.offset = new Vector2(0, distance / 2);
            collider2D.size = new Vector2(collider2D.size.x, distance);
            transform.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)transform.position).normalized) * transform.rotation;
            spawnPoint.localPosition = localPos;
            yield return null;
            timer += Time.deltaTime;
        }
        //Exit animation
        lineRenderer.enabled = false;
        collider2D.enabled = false;
        isAttacking = false;
    }
}
