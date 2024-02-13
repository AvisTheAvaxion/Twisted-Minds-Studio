//Note: This is only the logic for the bullet hell part of the enemy, not the state machine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShooter : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] int burstCount = 3;
    [SerializeField] float timeBetweenBursts = 1f;
    [SerializeField] float timeBetweenShots = .3f;

    bool isShooting = false;

    private void FixedUpdate()
    {
        Attack();
    }

    public void Attack()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    IEnumerator ShootRoutine()
    {
        isShooting = true;

        for(int i = 0; i < burstCount; i++)
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);


            var dir = newBullet.transform.position - GameObject.Find("Player").transform.position;
            var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            newBullet.transform.rotation = Quaternion.AngleAxis(angle, -Vector3.forward);

            newBullet.GetComponent<Rigidbody2D>().AddForce(-newBullet.transform.up * bulletForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(timeBetweenShots);
        }



        yield return new WaitForSeconds(timeBetweenBursts);

        isShooting = false;
    }
}
