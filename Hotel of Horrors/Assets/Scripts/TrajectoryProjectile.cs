using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryProjectile : MonoBehaviour
{
    [SerializeField] Color trajectoryColor1;
    [SerializeField] Color trajectoryColor2;
    [SerializeField] SpriteRenderer trajectoryRend;
    [SerializeField] float gracePeriod = 0.75f;
    Projectile projectile;
    Rigidbody2D projectileRb;

    // Start is called before the first frame update
    void Start()
    {
        projectile = GetComponentInChildren<Projectile>();
        projectileRb = projectile.GetComponent<Rigidbody2D>();
    }

    public void SetGracePeriod(float gracePeriod)
    {
        this.gracePeriod = gracePeriod;
    }

    public void Launch(float launchForce, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        StartCoroutine(Trajectory(launchForce, forceMode));
    }

    IEnumerator Trajectory(float launchForce, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        float frameTime = 1 / 12f;
        WaitForSeconds wait = new WaitForSeconds(frameTime);
        float timer = 0;
        int i = 0;
        trajectoryRend.enabled = true;
        while (timer < gracePeriod)
        {
            if(i == 0)
            {
                trajectoryRend.color = trajectoryColor1;
            }
            else
            {
                trajectoryRend.color = trajectoryColor2;
            }
            yield return wait;
            i++;
            i = i % 2;
            timer += frameTime;
        }
        trajectoryRend.enabled = false;

        Vector2 dir = transform.up;
        projectileRb.AddForce(dir * launchForce, forceMode);
    }
}
