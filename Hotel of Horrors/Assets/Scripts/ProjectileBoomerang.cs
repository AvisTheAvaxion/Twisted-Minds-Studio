using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBoomerang : Projectile
{
    [SerializeField] float trackingForce = 5f;
    [SerializeField] float trackTime = 1.5f;
    [SerializeField] Transform rotateTarget;
    [SerializeField] float rotateSpeed = 10;

    Vector2 targetPos;
    bool trackTarget = true;

    public void Initialize(Vector2 targetPosIn)
    {
        targetPos = targetPosIn;
        trackTarget = true;

        Invoke("StopTracking", trackTime);
    }

    private void Update()
    {
        if(rotateSpeed > 0)
        {
            rotateTarget.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime * 20, Space.Self);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackTarget)
        {
            Vector2 dirToTarget = (targetPos - (Vector2)transform.position).normalized;
            rb.AddForce(dirToTarget * trackingForce * 10 * Time.fixedDeltaTime);
        }
    }

    void StopTracking()
    {
        trackTarget = false;
    }
}
