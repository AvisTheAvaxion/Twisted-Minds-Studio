using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Attacks;

public class ActionController : MonoBehaviour
{
    [SerializeField] AttackModes currentAttackMode;
    [SerializeField] Animator playerAnimator; //Animator that plays the base attack animations
    [SerializeField] PlayerMovement playerMovement; //Player movement reference to check whether dashing or not
    [SerializeField] CameraShake cameraShake;
    //[SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] PlayerGUI playerGUI;
    [SerializeField] StatsController stats;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Transform weaponStrikeParent; //The parent to spawn the weapon strike into
    [SerializeField] Transform weaponStrike1SpawnPoint; //The point to spawn the weapon strike
    [SerializeField] Transform weaponStrike2SpawnPoint; //The point to spawn the weapon strike
    [SerializeField] SpriteRenderer weaponVisual; //The sprite renderer of the weapon visual located under Hand on the player
    [SerializeField] Transform hand;

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

    [SerializeField] PlayerAudio playerAudio;

    bool autoAttack = false;

    bool attackButtonPressed;
    bool attackButtonReleased;

    bool isAttacking;
    public bool IsAttacking { get => isAttacking; }
    public AttackModes CurrentAttackMode { get => currentAttackMode; }
    public Vector2 CrosshairPosition { get => rangedCrosshair.transform.position; }

    bool disableAttackControls = false;

    bool canAttack;

    bool canDoPlayerAbility = true;
    bool canDoWeaponAbility = true;
    bool canDoSpecialAbility = true;

    float currentAmmoCount;
    float rangedFireTimer;

    PlayerInventory inventory;

    //Current weapon equipped in the inventory
    //Weapon currentWeapon;
    WeaponAbility currentWeaponAbility;

    public PlayerAbility currentPlayerAbitlity;

    public SpecialAbility currentSpecialAbility;

    public int attackNumber { get; private set; }

    public void ToggleAttackControls(bool toggle)
    {
        disableAttackControls = !toggle;
    }
    

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        playerGUI = FindObjectOfType<PlayerGUI>();

        //if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        //if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (stats == null) stats = GetComponent<StatsController>();

        canAttack = true;
        isAttacking = false;
        attackButtonReleased = true;

        rangedCrosshair.SetActive(true);
        meleeCrosshair.SetActive(false);
    }

    //Called by the inventory system to store reference to the current weapon
    public void EquipWeapon(Weapon weapon)
    {

        if(inventory.CurrentWeapon != null)
        {
            weaponVisual.enabled = true;
            weaponVisual.sprite = inventory.CurrentWeapon.GetInfo().GetSprite();

            currentAttackMode = inventory.CurrentWeapon.GetInfo().GetWeaponMode();

            if (currentWeaponAbility != null) Destroy(currentWeaponAbility.gameObject);

            if (weapon.GetInfo().GetWeaponAbility() != null)
            {
                GameObject go = Instantiate(weapon.GetInfo().GetWeaponAbility(), transform.position + inventory.CurrentWeapon.GetInfo().GetWeaponAbility().transform.localPosition, Quaternion.identity, transform);
                WeaponAbility ability = go.GetComponent<WeaponAbility>();
                if (ability != null) currentWeaponAbility = ability;
            }
        } 
        else
        {
            if (currentWeaponAbility != null) Destroy(currentWeaponAbility.gameObject);

            weaponVisual.enabled = false;
            currentAttackMode = AttackModes.None;
        }
    }

    public void UnequipPlayerAbility()
    {
        if (currentPlayerAbitlity != null) Destroy(currentPlayerAbitlity.gameObject);
    }
    public void EquipPlayerAbility(Ability ability)
    {
        if (ability != null && canDoPlayerAbility)
        {
            if (currentPlayerAbitlity != null) Destroy(currentPlayerAbitlity.gameObject);

            if (ability.GetInfo().GetPlayerAbility() != null)
            {
                GameObject go = Instantiate(ability.GetInfo().GetPlayerAbility(), transform.position + ability.GetInfo().GetPlayerAbility().transform.localPosition, Quaternion.identity, transform);
                PlayerAbility playerAbility = go.GetComponent<PlayerAbility>();
                if (playerAbility != null) currentPlayerAbitlity = playerAbility;
            }
        }
    }
    public void EquipSpecialAbility(MementoInfo mententos)
    {
        if (mententos != null && canDoSpecialAbility)
        {
            if (currentSpecialAbility != null) Destroy(currentSpecialAbility.gameObject);

            if (mententos.GetSpecialAbility() != null)
            {
                GameObject go = Instantiate(mententos.GetSpecialAbility(), transform.position + mententos.GetSpecialAbility().transform.localPosition, Quaternion.identity, transform);
                SpecialAbility ability = go.GetComponent<SpecialAbility>();
                if (ability != null) currentSpecialAbility = ability;
            }
        }
    }

    #region Input Detection
    void OnAttack(InputValue inputValue)
    {
        if (disableAttackControls) return;

        attackButtonPressed = inputValue.isPressed;

        if(isAttacking || (!isAttacking && attackButtonReleased == false)) attackButtonReleased = !inputValue.isPressed;

        /*if (isAttacking && !attackButtonPressed && currentAttackMode == AttackModes.Ranged)
        {
            AttackEnd();
        }*/
    }
    void OnAttackAbility(InputValue inputValue)
    {
        /*if (disableAttackControls) return;

        if (currentWeaponAbility != null && canDoWeaponAbility)
        {
            currentWeaponAbility.Use(this);
            StartCoroutine(WeaponAbilityCooldown(currentWeaponAbility.Cooldown));
        }*/
    }
    void OnPrimaryAction(InputValue inputValue)
    {
        if (disableAttackControls) return;

        if (currentSpecialAbility != null && canDoSpecialAbility)
        {
            currentSpecialAbility.Use(this);
            StartCoroutine(SpecialAbilityCooldown(currentSpecialAbility.Cooldown));
        }
    }
    void OnSecondaryAction(InputValue inputValue)
    {
        if (disableAttackControls) return;

        if (currentPlayerAbitlity != null && canDoPlayerAbility)
        {
            currentPlayerAbitlity.Use(this, inventory.CurrentAbility);
            StartCoroutine(PlayerAbilityCooldown(currentPlayerAbitlity.Cooldown));
        }
        else if (inventory.CurrentItem != null)
        {
            Item item = inventory.CurrentItem;
            inventory.UseItem(inventory.currentItemIndex);
            foreach (EffectInfo effectInfo in item.GetInfo().GetEffectInfos())
            {
                Effect effect = new Effect(effectInfo, 1);
                playerHealth.InflictEffect(effect);
            }
            playerGUI.UpdateHotbarGUI();
        }
    }
    #endregion

    private void Update()
    {
        if (canAttack && !isAttacking)
        {
            if (attackButtonReleased && attackButtonPressed && !playerMovement.IsDashing)
            {
                StartCoroutine(Attack());
                //playerAudio.Play("Attack");
            }
            if(playerMovement.IsDashing)
            {
                attackButtonReleased = true;
            }
        }
    }

    //Function to attack, it is seperated from OnAttack so auto attacking can be enabled
    private IEnumerator Attack()
    {
        transform.SendMessage("Attacked"); //Set player to attack mode

        isAttacking = true;
        attackButtonReleased = false;
        
        /*if (currentWeapon != null)
        {
            if(currentWeapon.GetInfo().GetAttackType() == AttackType.Melee1)
            {
                hand.transform.localRotation = Quaternion.identity;
                //playerAnimator.SetTrigger("MeleeAttack1");
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
                //playerAnimator.SetTrigger("MeleeAttack2");
            }
            //playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator
        }
        else
        {
            //playerAnimator.SetTrigger("MeleeAttack1");
            //playerAnimator.SetLayerWeight(1, 1); //Enables the attack layer on the player animator
        }*/

        yield return new WaitForSeconds(0.1f);

        if(attackNumber == 2 && currentWeaponAbility != null)
        {
            currentWeaponAbility.Use(this, inventory.CurrentWeapon);
            yield return new WaitForSeconds(inventory.CurrentWeapon.GetInfo().GetWeaponAbilityDuration());
            AttackEnd();
        } else
        {
            MeleeAttack();
        }
    }

    //Animation event to spawn the melee strike
    public void MeleeAttack()
    {
        if(inventory.CurrentWeapon != null)
        {
            MeleeSlash meleeStrike = null;
            if (inventory.CurrentWeapon.GetInfo().GetAttackType() == AttackType.Melee1)
            {
                GameObject go = Instantiate(inventory.CurrentWeapon.GetInfo().GetWeaponStrike(attackNumber), 
                    weaponStrike1SpawnPoint.position + weaponStrikeParent.up * inventory.CurrentWeapon.GetInfo().GetRange(), weaponStrike1SpawnPoint.rotation, weaponStrikeParent);
                meleeStrike = go.GetComponent<MeleeSlash>();
            } else
            {
                GameObject go = Instantiate(inventory.CurrentWeapon.GetInfo().GetWeaponStrike(attackNumber), 
                    weaponStrike1SpawnPoint.position + weaponStrikeParent.up * inventory.CurrentWeapon.GetInfo().GetRange(), weaponStrike1SpawnPoint.rotation, weaponStrikeParent);
                meleeStrike = go.GetComponent<MeleeSlash>();
            }
            if (meleeStrike)
            {
                meleeStrike.Init(this, inventory.CurrentWeapon, "Enemy", cameraShake);
            }
        } 
        else
        {
            GameObject go = Instantiate(defaultMeleeStrike, weaponStrike1SpawnPoint.position, weaponStrike1SpawnPoint.rotation, weaponStrikeParent);
            MeleeSlash meleeStrike = go.GetComponent<MeleeSlash>();
            if (meleeStrike) meleeStrike.Init(this, defaultDamage, defaultKnockback, defaultDeflectionStrength, "Enemy", cameraShake);
        }
    }

    //End of an attack for melee (animation event) and ranged
    public void AttackEnd()
    {
        //playerAnimator.SetBool("Shooting1", false);
        //playerAnimator.SetLayerWeight(1, 0); //Disables the attack layer on the player animator

        isAttacking = false;

        attackNumber++;

        attackNumber = attackNumber % 3;

        if (hand) hand.rotation = Quaternion.identity;

        canAttack = false;
        if (inventory.CurrentWeapon != null)
        {
            StartCoroutine(AttackCooldown(1 / (inventory.CurrentWeapon.GetInfo().GetAttackSpeed() + stats.GetCurrentValue(Stat.StatType.AttackSpeed))));
        }
        else
        {
            StartCoroutine(AttackCooldown(1 / defaultAttackSpeed));
        }
    }

    #region Cooldowns
    IEnumerator AttackCooldown(float waitTime)
    {
        canAttack = false;
        
        yield return new WaitForSeconds(waitTime);

        canAttack = true;

        if (attackButtonPressed && (autoAttack || (inventory.CurrentWeapon != null && inventory.CurrentWeapon.GetInfo().IsAutoAttack())))
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator PlayerAbilityCooldown(float cooldown)
    {
        float timer = 0;
        canDoPlayerAbility = false;
        //playerAudio.PlayAbility(currentPlayerAbitlity.ToString());

        yield return new WaitUntil(() => !currentPlayerAbitlity.isAttacking);

        while (timer <= cooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canDoPlayerAbility = true;
    }
    IEnumerator WeaponAbilityCooldown(float cooldown)
    {
        float timer = 0;
        canDoWeaponAbility = false;

        yield return new WaitUntil(() => !currentWeaponAbility.isAttacking);

        while (timer <= cooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canDoWeaponAbility = true;
    }
    IEnumerator SpecialAbilityCooldown(float cooldown)
    {
        float timer = 0;
        canDoSpecialAbility = false;

        yield return new WaitUntil(() => !currentSpecialAbility.isAttacking);

        while (timer <= cooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canDoSpecialAbility = true;
    }
    #endregion

    public void ShakeCamera(float frequency, float length, bool interruptable = true)
    {
        cameraShake.ShakeCamera(frequency, 0.5f, length, interruptable);
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
    public enum WeaponClass
    {
        Stab, HeavyStab, Slash, HeavySlash, Slam, Swing
    }
}
