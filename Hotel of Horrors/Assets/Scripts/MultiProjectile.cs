using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiProjectile : MonoBehaviour
{
    public enum LaunchMode
    {
        Circle, Direction, CircleSpin
    }
    [SerializeField] Projectile[] projectiles;
    [SerializeField] float spinForceScalar = 0.5f;
    [SerializeField] LaunchMode launchMode = LaunchMode.Direction;

    Rigidbody2D[] rigidBodies;

    public void Initialize()
    {
        if (projectiles == null || projectiles.Length == 0)
            projectiles = GetComponentsInChildren<Projectile>();

        if(launchMode == LaunchMode.Circle || launchMode == LaunchMode.CircleSpin)
        {
            transform.rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.forward);
        }

        rigidBodies = new Rigidbody2D[projectiles.Length];
        for (int i = 0; i < projectiles.Length; i++)
        {
            rigidBodies[i] = projectiles[i].GetComponent<Rigidbody2D>();

            if (launchMode == LaunchMode.Circle || launchMode == LaunchMode.CircleSpin)
            {
                float angle = Vector3.Angle(Vector3.up, (projectiles[i].transform.position - transform.position).normalized);
                //projectiles[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);// * rigidBodies[i].transform.rotation;
                projectiles[i].transform.rotation = Quaternion.FromToRotation(-projectiles[i].transform.up, (projectiles[i].transform.position - transform.position).normalized) * projectiles[i].transform.rotation;
            }
        }
    }

    public void Initialize(Ability ability)
    {
        if (projectiles == null || projectiles.Length == 0)
            projectiles = GetComponentsInChildren<Projectile>();

        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i].Initialize(ability);
        }

        rigidBodies = new Rigidbody2D[projectiles.Length];
        for (int i = 0; i < projectiles.Length; i++)
        {
            rigidBodies[i] = projectiles[i].GetComponent<Rigidbody2D>();
        }
    }

    public void Launch(float launchForce, Vector2 dir, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        if (rigidBodies == null) Initialize();

        for (int i = 0; i < rigidBodies.Length; i++)
        {
            if(launchMode == LaunchMode.Direction)
            {
                rigidBodies[i].AddForce(dir * launchForce, forceMode);
            }
            else if (launchMode == LaunchMode.Circle)
            {
                dir = (rigidBodies[i].transform.position - transform.position).normalized;
                rigidBodies[i].AddForce(dir * launchForce, forceMode);
            }
            else if (launchMode == LaunchMode.CircleSpin)
            {
                dir = (rigidBodies[i].transform.position - transform.position).normalized;
                rigidBodies[i].AddForce(dir * launchForce, forceMode);
                rigidBodies[i].AddForce(Vector3.Cross(dir, Vector3.forward) * launchForce * spinForceScalar, forceMode);
            }
        }
    }
}
