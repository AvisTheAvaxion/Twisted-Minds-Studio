using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Attacks;

public class AttackController : MonoBehaviour
{
    [SerializeField] AttackModes currentAttackMode;
    [SerializeField] Animator playerAnimator; //Animator that plays the base attack animations
    [SerializeField] PlayerMovement playerMovement; //Player movement reference to check whether dashing or not
    [SerializeField] CameraShake cameraShake;
    [SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] StatsController stats;
    [SerializeField] Transform weaponStrikeParent; //The parent to spawn the weapon strike into
    [SerializeField] Transform weaponStrike1SpawnPoint; //The point to spawn the weapon strike
    [SerializeField] Transform weaponStrike2SpawnPoint; //The point to spawn the weapon strike
    [SerializeField] SpriteRenderer weaponVisual; //The sprite renderer of the weapon visual located under Hand on the player
    [SerializeField] Transform hand;

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
    bool attackButtonReleased;

    bool isAttacking;
    public bool IsAttacking { get => isAttacking; }
    public AttackModes CurrentAttackMode { get => currentAttackMode; }

    bool canAttack;

    float currentAmmoCount;
    float rangedFireTimer;

    Inventory inventory;

    //Current weapon equipped in the inventory
    Weapon currentWeapon;
    

    private void Awake()
    {
        inventory = GetComponent<Inventory>();

        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (stats == null) stats = GetComponent<StatsController>();

        canAttack = true;
        isAttacking = false;
        attackButtonReleased = true;

        rangedCrosshair.SetActive(true);
        meleeCrosshair.SetActive(false);

        /*if (currentAttackMode.Equals(AttackModes.Melee) || currentAttackMode.Equals(AttackModes.None))
        {
            rangedCrosshair.SetActive(false);
            meleeCrosshair.SetActive(true);
        }
        else if (currentAttackMode.Equals(AttackModes.Ranged))
        {
            rangedCrosshair.SetActive(true);
            meleeCrosshair.SetActive(false);
        }*/
    }

    //Called by the inventory system to store reference to the current weapon
    public void Equip(Weapon weapon)
    {
        currentWeapon = weapon;

        currentAmmoCount = int.MaxValue;

        if(currentWeapon != null)
        {
            weaponVisual.enabled = true;
            weaponVisual.sprite = currentWeapon.GetSprite();

            currentAttackMode = currentWeapon.GetWeaponMode();

            if(currentAttackMode == AttackModes.Ranged)
            {
                currentAmmoCount = currentWeapon.GetMaxAmmoCount();
                if (uiDisplay.AmmoTextContainer) uiDisplay.AmmoTextContainer.SetActive(true);
                if (uiDisplay.MaxAmmoText) uiDisplay.MaxAmmoText.text = currentAmmoCount.ToString();
                if (uiDisplay.CurrentAmmoText) uiDisplay.CurrentAmmoText.text = currentAmmoCount.ToString();
            } else
            {
                if (uiDisplay.AmmoTextContainer) uiDisplay.AmmoTextContainer.SetActive(false);
            }
        } 
        else
        {
            weaponVisual.enabled = false;
            currentAttackMode = AttackModes.None;
        }

       /* if (currentAttackMode.Equals(AttackModes.Melee) || currentAttackMode.Equals(AttackModes.None))
        {
            rangedCrosshair.SetActive(false);
            meleeCrosshair.SetActive(true);
        }
        else if (currentAttackMode.Equals(AttackModes.Ranged))
        {
            rangedCrosshair.SetActive(true);
            meleeCrosshair.SetActive(false);
        }*/
    }

    void OnAttack(InputValue inputValue)
    {
        attackButtonPressed = inputValue.isPressed;

        if(isAttacking || (!isAttacking && attackButtonReleased == false)) attackButtonReleased = !inputValue.isPressed;

        if (isAttacking && !attackButtonPressed && currentAttackMode == AttackModes.Ranged)
        {
            AttackEnd();
        }
    }

    private void Update()
    {
        if (canAttack && !isAttacking)
        {
            if (attackButtonReleased && attackButtonPressed && !playerMovement.IsDashing)
            {
                Attack();
            }
            if(playerMovement.IsDashing)
            {
                attackButtonReleased = true;
            }
        }

        if(currentAttackMode == AttackModes.Ranged)
        {
            Vector2 dir = (rangedCrosshair.transform.position - hand.position).normalized;
            switch(playerMovement.Direction)
            {
                case "North":
                    hand.rotation = Quaternion.FromToRotation(hand.up, dir) * hand.rotation;
                    break;
                case "South":
                    hand.rotation = Quaternion.FromToRotation(-hand.up, dir) * hand.rotation;
                    break;
                case "East":
                    hand.rotation = Quaternion.FromToRotation(hand.right, dir) * hand.rotation;
                    break;
                case "West":
                    hand.rotation = Quaternion.FromToRotation(-hand.right, dir) * hand.rotation;
                    break;
            }


            if (isAttacking)
            {
                playerAnimator.SetBool("Shooting1", true);
                playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator

                rangedFireTimer += Time.deltaTime;

                if (currentWeapon != null)
                    if (rangedFireTimer > 1 / currentWeapon.GetFireRate()) AttackEnd();
                else
                    if (rangedFireTimer > 0.4f) AttackEnd();
            }
        }
    }

    //Function to attack, it is seperated from OnAttack so auto attacking can be enabled
    private void Attack()
    {
        transform.SendMessage("Attacked"); //Set player to attack mode

        isAttacking = true;
        attackButtonReleased = false;

        AudioManager.Play("Attack");

        if (currentAttackMode == AttackModes.Ranged)
        {
            rangedFireTimer = 0;

            currentAmmoCount--;
            if (uiDisplay.CurrentAmmoText) uiDisplay.CurrentAmmoText.text = currentAmmoCount.ToString();

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
                hand.transform.localRotation = Quaternion.identity;
                playerAnimator.SetTrigger("MeleeAttack1");
            } 
            else
            {
                Vector2 dir = (meleeCrosshair.transform.position - hand.position).normalized;
                switch (playerMovement.Direction)
                {
                    case "North":
                        hand.rotation = Quaternion.FromToRotation(hand.up, dir) * hand.rotation;
                        break;
                    case "South":
                        hand.rotation = Quaternion.FromToRotation(-hand.up, dir) * hand.rotation;
                        break;
                    case "East":
                        hand.rotation = Quaternion.FromToRotation(hand.right, dir) * hand.rotation;
                        break;
                    case "West":
                        hand.rotation = Quaternion.FromToRotation(-hand.right, dir) * hand.rotation;
                        break;
                }
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
            MeleeSlash meleeStrike = null;
            if (currentWeapon.GetAttackType() == AttackType.Melee1)
            {
                GameObject go = Instantiate(currentWeapon.GetWeaponStrike(), weaponStrike1SpawnPoint.position + weaponStrikeParent.up * currentWeapon.GetRange(), weaponStrike1SpawnPoint.rotation, weaponStrikeParent);
                meleeStrike = go.GetComponent<MeleeSlash>();
            } else
            {
                GameObject go = Instantiate(currentWeapon.GetWeaponStrike(), weaponStrike2SpawnPoint.position + weaponStrikeParent.up * currentWeapon.GetRange(), weaponStrike2SpawnPoint.rotation, weaponStrikeParent);
                meleeStrike = go.GetComponent<MeleeSlash>();
            }
            if (meleeStrike)
            {
                EffectInfo[] effectInfos = currentWeapon.GetEffectsToInflict();
                Effect[] effects = new Effect[effectInfos.Length];
                for (int i = 0; i < effects.Length; i++)
                {
                    effects[i] = new Effect(effectInfos[i], currentWeapon.GetChanceToInflictEffect());
                }
                meleeStrike.Init(currentWeapon.GetDamage(), currentWeapon.GetKnockback(), currentWeapon.GetDeflectionStrength(), "Enemy", cameraShake, effects);
            }
        } 
        else
        {
            GameObject go = Instantiate(defaultMeleeStrike, weaponStrike1SpawnPoint.position, weaponStrike1SpawnPoint.rotation, weaponStrikeParent);
            MeleeSlash meleeStrike = go.GetComponent<MeleeSlash>();
            if (meleeStrike) meleeStrike.Init(defaultDamage, defaultKnockback, defaultDeflectionStrength, "Enemy", cameraShake);
        }
    }

    //End of an attack for melee (animation event) and ranged
    public void AttackEnd()
    {
        playerAnimator.SetBool("Shooting1", false);
        playerAnimator.SetLayerWeight(1, 0); //Disables the attack layer on the player animator

        isAttacking = false;

        if (hand) hand.rotation = Quaternion.identity;

        canAttack = false;
        if (currentWeapon != null)
        {
            if (currentWeapon.GetWeaponMode() == AttackModes.Ranged && currentAmmoCount <= 0)
                StartCoroutine(ReloadCooldown(currentWeapon.GetReloadTime()));
            else
                StartCoroutine(AttackCooldown(1 / (currentWeapon.GetAttackSpeed() + stats.GetCurrentValue(Stat.StatType.AttackSpeed))));
        }
        else
        {
            StartCoroutine(AttackCooldown(1 / defaultAttackSpeed));
        }

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

    IEnumerator ReloadCooldown(float reloadTime)
    {
        Weapon startWeapon = currentWeapon;

        //playerAnimator.SetBool("Shooting1", false);
        //playerAnimator.SetLayerWeight(1, 0); //Disables the attack layer on the player animator
        //isAttacking = false;

        float timer = 0;
        canAttack = false;

        if (uiDisplay.ReloadCircle) uiDisplay.ReloadCircle.gameObject.SetActive(true);

        while (timer <= reloadTime)
        {
            timer += Time.deltaTime;
            if (uiDisplay.ReloadCircle) uiDisplay.ReloadCircle.fillAmount = timer / reloadTime;
            yield return null;
        }

        if (uiDisplay.ReloadCircle) uiDisplay.ReloadCircle.gameObject.SetActive(false);

        canAttack = true;
        if (startWeapon == currentWeapon)
        {
            currentAmmoCount = currentWeapon.GetMaxAmmoCount();
            if (uiDisplay.CurrentAmmoText) uiDisplay.CurrentAmmoText.text = currentAmmoCount.ToString();

            if (attackButtonPressed && (autoAttack || (currentWeapon != null && currentWeapon.IsAutoAttack())))
            {
                Attack();
            }
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
    }
}
