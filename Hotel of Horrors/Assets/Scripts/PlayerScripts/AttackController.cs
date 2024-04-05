using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Attacks;

public class AttackController : MonoBehaviour
{
    [SerializeField] AttackModes currentAttackMode;
    [SerializeField] Animator playerAnimator; //Animator that plays the base attack animations
    [SerializeField] Animator weaponAnimator; //Animator for the weapon that plays if clip exists and offers way to add custom input to base animation. Animator exists on the weapon pivot
    [SerializeField] PlayerMovement playerMovement; //Player movement reference to check whether dashing or not
    [SerializeField] Transform weaponStrikeParent; //The parent to spawn the weapon strike into
    [SerializeField] SpriteRenderer weaponVisual; //The sprite renderer of the weapon visual located under Hand on the player

    //Note: the weapon strike effect will be what detects a collision and does damage and be unique to each weapon

    [Header("Ranged")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnLocation;
    [SerializeField] GameObject rangedCrosshair;
    [SerializeField] float bulletForce = 20f;

    [Header("Melee")]
    [SerializeField] GameObject meleeCrosshair;

    bool isAttacking;
    public bool IsAttacking { get => isAttacking; }

    Inventory inventory;

    //Current weapon equipped in the inventory
    Weapon currentWeapon;
    

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void FixedUpdate()
    {
        if(inventory.currentWeapon != null && inventory.currentWeapon.GetWeaponMode() == AttackModes.Melee)
        {
            currentAttackMode = AttackModes.Melee;
        }
        else if(inventory.currentWeapon != null)
        {
            currentAttackMode = AttackModes.Ranged;
        }

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

    //Called by the inventory system to store reference to the current weapon
    public void Equip(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    void OnAttack(InputValue inputValue)
    {
        print(inputValue.isPressed);
        if (inputValue.isPressed && !playerMovement.IsDashing)
        {
            transform.SendMessage("Attacked"); //Set player to attack mode

            if (currentAttackMode == AttackModes.Ranged)
            {
                playerAnimator.SetBool("Shooting1", true);
                playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator

                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnLocation.position, bulletSpawnLocation.rotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(bulletSpawnLocation.up * bulletForce, ForceMode2D.Impulse);
            }
            else if (currentAttackMode == AttackModes.Melee)
            {
                playerAnimator.SetTrigger("MeleeAttack1");
                playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator
            }
        } 
        else if (!inputValue.isPressed && currentAttackMode == AttackModes.Ranged)
        {
            AttackEnd();
        }
    }

    //End of an attack for melee (animation event) and ranged
    public void AttackEnd()
    {
        playerAnimator.SetBool("Shooting1", false);
        playerAnimator.SetLayerWeight(1, 0); //Disables the attack layer on the player animator
    }

    public void ChangeAttackMode(AttackModes currentMode)
    {
        currentAttackMode = currentMode;
    }
}

namespace Attacks
{
    public enum AttackModes
    {
        Melee,
        Ranged,
        None
    }

    public enum AttackType
    {
        Melee1,
        Melee2,
        Range1,
        Range2
    }
}
