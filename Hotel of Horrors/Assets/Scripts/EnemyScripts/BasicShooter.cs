//Note: This is only the logic for the bullet hell part of the enemy, not the state machine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShooter : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] Vector2 debugTargetPos = new Vector2(1, 1);
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float startingDistance = 0.1f;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] int burstCount = 3;
    [SerializeField] int projectilesPerBurst;
    [SerializeField] [Range(0, 359)] float angleSpread;
    [SerializeField] float timeBetweenBursts = 1f;
    [SerializeField] float timeBetweenShots = .3f;
    [SerializeField] bool stagger;
    [SerializeField] bool oscillate;
    [Space(15f)]
    [SerializeField] Transform target;

    bool isShooting = false;

    private void Start()
    {
        if (bulletSpawnPoint == null)
            bulletSpawnPoint = transform;

        if(target == null)
        {
            target = GameObject.Find("Player").transform;
        }
    }

    public void Attack()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    IEnumerator ShootRoutine()
    {
        if (target != null)
        {
            isShooting = true;

            float startAngle, currentAngle, angleStep, endAngle;
            float timeTimeBetweenProjectiles = 0f;

            TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);

            if (stagger)
                timeTimeBetweenProjectiles = timeBetweenBursts / projectilesPerBurst;

            for (int i = 0; i < burstCount; i++)
            {
                if (!oscillate)
                {
                    TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
                }
                else
                {
                    currentAngle = endAngle;
                    endAngle = startAngle;
                    startAngle = currentAngle;
                    angleStep *= -1;
                }

                for (int j = 0; j < projectilesPerBurst; j++)
                {
                    Vector2 pos = FindBulletSpawnPos(currentAngle);
                    GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);


                    //var dir = newBullet.transform.position - GameObject.Find("Player").transform.position;
                    //var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                    //newBullet.transform.rotation = Quaternion.AngleAxis(currentAngle, -Vector3.forward);
                    newBullet.transform.rotation = Quaternion.FromToRotation(newBullet.transform.up, (pos - (Vector2)bulletSpawnPoint.position).normalized) * newBullet.transform.rotation;

                    newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.up * bulletForce, ForceMode2D.Impulse);

                    currentAngle += angleStep;

                    if (stagger)
                        yield return new WaitForSeconds(timeTimeBetweenProjectiles);
                }

                currentAngle = startAngle;

                yield return new WaitForSeconds(timeBetweenShots);
            }
        }

        yield return new WaitForSeconds(timeBetweenBursts);

        isShooting = false;
    }

    Vector2 FindBulletSpawnPos(float currentAngle)
    {
        float x = bulletSpawnPoint.position.x + startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = bulletSpawnPoint.position.y + startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 pos = new Vector2(x, y);

        return pos;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        var dir = (bulletSpawnPoint.position - target.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;

        startAngle = angle;
        endAngle = angle;
        currentAngle = angle;
        float halfAngleSpread = 0f;
        angleStep = 0;
        if (angleSpread != 0)
        {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2f;
            startAngle = angle - halfAngleSpread;
            endAngle = angle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }
    private void TargetConeOfInfluence(Vector3 targetPos, out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        var dir = (bulletSpawnPoint.position - targetPos).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;

        startAngle = angle;
        endAngle = angle;
        currentAngle = angle;
        float halfAngleSpread = 0f;
        angleStep = 0;
        if (angleSpread != 0)
        {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2f;
            startAngle = angle - halfAngleSpread;
            endAngle = angle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        float startAngleDebug;
        float currentAngleDebug;
        float angleStepDebug;
        float endAngleDebug;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(bulletSpawnPoint.position, 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(debugTargetPos, 0.05f);
        Gizmos.color = Color.white;

        if (target != null)
            TargetConeOfInfluence(out startAngleDebug, out currentAngleDebug, out angleStepDebug, out endAngleDebug);
        else
            TargetConeOfInfluence(debugTargetPos, out startAngleDebug, out currentAngleDebug, out angleStepDebug, out endAngleDebug);

        for (int j = 0; j < projectilesPerBurst; j++)
        {
            Vector2 pos = FindBulletSpawnPos(currentAngleDebug);

            Gizmos.DrawWireSphere(pos, 0.1f);
            //var dir = newBullet.transform.position - GameObject.Find("Player").transform.position;
            //var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            currentAngleDebug += angleStepDebug;

        }
    }
}
