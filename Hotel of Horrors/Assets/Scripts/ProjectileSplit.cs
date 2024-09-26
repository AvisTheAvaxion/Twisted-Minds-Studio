using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSplit : Projectile
{
    [SerializeField] float timeTillDestroy = 2;
    [SerializeField] Projectile projetilesToSpawn;
    [SerializeField] int numOfChildProj = 3;
    [SerializeField] float childStartingDistance = 0.01f;
    [SerializeField] float childLaunchForce = 20;
    [SerializeField] [Range(0, 359)] int spread = 359;

    protected override void Start()
    {
        base.Start();

        Invoke("DestroySelf", timeTillDestroy);
    }

    protected override void DestroySelf()
    {
        float startAngle, currentAngle, angleStep, endAngle;
        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
        float angleOffset = Random.Range(0f, 90f);
        for (int i = 0; i < numOfChildProj; i++)
        {
            Vector2 pos = FindBulletSpawnPos(currentAngle + angleOffset);
            GameObject newBullet = Instantiate(projetilesToSpawn.gameObject, pos, Quaternion.identity);

            newBullet.transform.rotation = Quaternion.FromToRotation(newBullet.transform.up, (pos - (Vector2)transform.position).normalized) * newBullet.transform.rotation;

            newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.up * childLaunchForce, ForceMode2D.Impulse);

            currentAngle += angleStep;
        }
        base.DestroySelf();
    }


    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        var dir = transform.up;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;

        startAngle = angle;
        endAngle = angle;
        currentAngle = angle;
        float halfAngleSpread = 0f;
        angleStep = 0;
        if (spread != 0)
        {
            angleStep = spread / (numOfChildProj - 1);
            halfAngleSpread = spread / 2f;
            startAngle = angle - halfAngleSpread;
            endAngle = angle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    Vector2 FindBulletSpawnPos(float currentAngle)
    {
        float x = transform.position.x + childStartingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + childStartingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 pos = new Vector2(x, y);

        return pos;
    }

    private void OnDrawGizmosSelected()
    {
        float startAngle, currentAngle, angleStep, endAngle;
        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
        for (int i = 0; i < numOfChildProj; i++)
        {
            Vector2 pos = FindBulletSpawnPos(currentAngle);
            Gizmos.DrawWireSphere(pos, 0.05f);

            currentAngle += angleStep;
        }
    }
}
