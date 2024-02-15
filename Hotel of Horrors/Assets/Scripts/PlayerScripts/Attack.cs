using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] AttackModes currentAttackMode;

    [Header("Ranged")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnLocation;
    [SerializeField] GameObject rangedCrosshair;
    [SerializeField] float bulletForce = 20f;

    [Header("Melee")]
    [SerializeField] GameObject meleeCrosshair;
    [SerializeField] GameObject meleeTrail;
    Animator animator;

    Inventory inventory;
    protected enum AttackModes
    {
        Melee,
        Ranged,
        None
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
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
        else if(currentAttackMode == AttackModes.Melee)
        {
            if(animator != null)
            {
                meleeTrail.SetActive(true);
                animator.SetTrigger("isAttacking");
            }
        }
        if (inventory.currentWeapon != null)
        {
            inventory.currentWeapon.Use();
        }
    }

    public void DisableTrail()
    {
        meleeTrail.SetActive(false);
    }
}
