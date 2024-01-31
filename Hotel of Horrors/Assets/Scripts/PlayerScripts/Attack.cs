using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] AttackModes currentAttackMode;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnLocation;
    [SerializeField] GameObject meleeCrosshair;
    [SerializeField] GameObject rangedCrosshair;
    [SerializeField] float bulletForce = 20f;

    protected enum AttackModes
    {
        Melee,
        Ranged,
        None
    }

    private void FixedUpdate()
    {
        if (currentAttackMode.Equals(AttackModes.Melee))
        {
            rangedCrosshair.SetActive(false);
            meleeCrosshair.SetActive(true);
        }
        else if (currentAttackMode.Equals(AttackModes.Ranged))
        {
            rangedCrosshair.SetActive(true);
            meleeCrosshair.SetActive(false);
        }
    }
    void OnAttack()
    {
        if (currentAttackMode == AttackModes.Ranged)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnLocation.position, bulletSpawnLocation.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bulletSpawnLocation.up * bulletForce, ForceMode2D.Impulse);
        }
    }
}
