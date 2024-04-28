using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiProjectile : MonoBehaviour
{
    public enum LaunchMode
    {
        Circle, Direction
    }
    [SerializeField] Projectile[] projectiles;
    [SerializeField] LaunchMode launchMode = LaunchMode.Direction;

    Rigidbody2D[] rigidBodies;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (projectiles == null || projectiles.Length == 0)
            projectiles = GetComponentsInChildren<Projectile>();

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
            else
            {
                dir = (rigidBodies[i].transform.position - transform.position).normalized;
                rigidBodies[i].AddForce(dir * launchForce, forceMode);
            }
        }
    }
}
