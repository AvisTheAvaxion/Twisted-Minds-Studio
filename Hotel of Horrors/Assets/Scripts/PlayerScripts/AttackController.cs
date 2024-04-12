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
    [SerializeField] AnimatorOverrideController weaponOverrideController;
    [SerializeField] PlayerMovement playerMovement; //Player movement reference to check whether dashing or not
    [SerializeField] Transform weaponStrikeParent; //The parent to spawn the weapon strike into
    [SerializeField] Transform weaponStrikeSpawnPoint; //The point to spawn the weapon strike
    [SerializeField] SpriteRenderer weaponVisual; //The sprite renderer of the weapon visual located under Hand on the player

    RuntimeAnimatorController defaultWeaponController;

    //Note: the weapon strike effect will be what detects a collision and does damage and be unique to each weapon

    [Header("Ranged")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnLocation;
    [SerializeField] GameObject rangedCrosshair;
    [SerializeField] float bulletForce = 20f;

    [Header("Melee")]
    [SerializeField] GameObject meleeCrosshair;
    [SerializeField] GameObject defaultMeleeStrike;
    [SerializeField] int defaultDamage = 1;
    [SerializeField] float defaultKnockback = 2f;
    [SerializeField] float defaultAttackSpeed = 3f;
    [SerializeField] float defaultDeflectionStrength = 2f;

    bool autoAttack = false;

    bool attackButtonPressed;

    bool isAttacking;
    public bool IsAttacking { get => isAttacking; }

    bool canAttack;

    Inventory inventory;

    //Current weapon equipped in the inventory
    Weapon currentWeapon;
    

    private void Awake()
    {
        inventory = GetComponent<Inventory>();

        defaultWeaponController = weaponAnimator.runtimeAnimatorController;

        canAttack = true;
        isAttacking = false;
    }

    //Called by the inventory system to store reference to the current weapon
    public void Equip(Weapon weapon)
    {
        currentWeapon = weapon;

        if(currentWeapon != null)
        {
            weaponVisual.sprite = currentWeapon.GetSprite();

            weaponOverrideController["DefaultMelee"] = currentWeapon.GetWeaponAnimation();
            weaponAnimator.runtimeAnimatorController = weaponOverrideController;

            currentAttackMode = currentWeapon.GetWeaponMode();
        } 
        else
        {
            weaponAnimator.runtimeAnimatorController = defaultWeaponController;
            currentAttackMode = AttackModes.Melee;
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

    void OnAttack(InputValue inputValue)
    {
        attackButtonPressed = inputValue.isPressed;
        if (canAttack && !isAttacking)
        {
            if (attackButtonPressed && !playerMovement.IsDashing)
            {
                Attack();
            }
        }
        if (isAttacking && !attackButtonPressed && currentAttackMode == AttackModes.Ranged)
        {
            AttackEnd();
        }
    }

    //Function to attack, it is seperated from OnAttack so auto attacking can be enabled
    private void Attack()
    {
        transform.SendMessage("Attacked"); //Set player to attack mode

        isAttacking = true;

        if (currentAttackMode == AttackModes.Ranged)
        {
            playerAnimator.SetBool("Shooting1", true);
            playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnLocation.position, bulletSpawnLocation.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bulletSpawnLocation.up * bulletForce, ForceMode2D.Impulse);
        }
        else if (currentAttackMode == AttackModes.Melee && currentWeapon != null)
        {
            if(currentWeapon.GetAttackType() == AttackType.Melee1)
            {
                playerAnimator.SetTrigger("MeleeAttack1");
            } 
            else
            {
                playerAnimator.SetTrigger("MeleeAttack2");
            }
            playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator
        }
        else
        {
            playerAnimator.SetTrigger("MeleeAttack1");
            playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator
        }
    }

    //Animation event to spawn the melee strike
    public void MeleeAttack()
    {
        if(currentWeapon != null)
        {
            GameObject go = Instantiate(currentWeapon.GetWeaponStrike(), weaponStrikeSpawnPoint.position, weaponStrikeSpawnPoint.rotation, weaponStrikeParent);
            MeleeStrike meleeStrike = go.GetComponent<MeleeStrike>();
            if (meleeStrike) 
            {
                EffectInfo[] effectInfos = currentWeapon.GetEffectsToInflict();
                Effect[] effects = new Effect[effectInfos.Length];
                for (int i = 0; i < effects.Length; i++)
                {
                    effects[i] = new Effect(effectInfos[i], currentWeapon.GetChanceToInflictEffect());
                }
                meleeStrike.Init(currentWeapon.GetDamage(), currentWeapon.GetKnockback(), currentWeapon.GetDeflectionStrength(), "Enemy"); 
            }
        } 
        else
        {
            GameObject go = Instantiate(defaultMeleeStrike, weaponStrikeSpawnPoint.position, weaponStrikeSpawnPoint.rotation, weaponStrikeParent);
            MeleeStrike meleeStrike = go.GetComponent<MeleeStrike>();
            if (meleeStrike) meleeStrike.Init(defaultDamage, defaultKnockback, defaultDeflectionStrength, "Enemy");
        }
    }

    //End of an attack for melee (animation event) and ranged
    public void AttackEnd()
    {
        playerAnimator.SetBool("Shooting1", false);
        playerAnimator.SetLayerWeight(1, 0); //Disables the attack layer on the player animator

        isAttacking = false;

        if (currentWeapon != null)
            StartCoroutine(AttackCooldown(1 / currentWeapon.GetAttackSpeed()));
        else
            StartCoroutine(AttackCooldown(1 / defaultAttackSpeed));

    }

    public void ChangeAttackMode(AttackModes currentMode)
    {
        currentAttackMode = currentMode;
    }
    public void SetAutoAttack(bool autoAttack)
    {
        this.autoAttack = autoAttack;
    }

    IEnumerator AttackCooldown(float waitTime)
    {
        canAttack = false;
        yield return new WaitForSeconds(waitTime);

        canAttack = true;

        if (attackButtonPressed && (autoAttack || (currentWeapon != null && currentWeapon.IsAutoAttack())))
        {
            Attack();
        }
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
