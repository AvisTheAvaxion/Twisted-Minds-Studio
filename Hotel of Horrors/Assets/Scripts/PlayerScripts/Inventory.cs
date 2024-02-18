using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] List<Useables> inventoryItems;
    [SerializeField] public Weapon currentWeapon;
    [SerializeField] public Useables itemOne;
    [SerializeField] public Useables itemTwo;

    [Space(10)]
    [SerializeField] GameObject inventoryUI;
    [SerializeField] SpriteRenderer weaponPlacement;
    [SerializeField] Animator animator;
    AnimatorOverrideController overrideController;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ItemData>())
        {
            Debug.Log("PickUp Get");
            Useables useable = collision.GetComponent<ItemData>().GetItemData();
            Debug.Log(useable.GetDescription());
            collision.gameObject.SetActive(false);
            inventoryItems.Add(useable);
            if(currentWeapon == null && useable is Weapon)
            {
                currentWeapon = (Weapon)useable;
                EquipWeapon(currentWeapon);
            }
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        weaponPlacement.sprite = currentWeapon.GetWeaponSprite();
        overrideController["WeaponAttack"] = currentWeapon.GetWeaponAnimation();
        Debug.Log(overrideController["TestAttackAnim"] = currentWeapon.GetWeaponAnimation());
        //Insert code that is done on equip like moving gameobject beside the player / Swaping Sprites
    }

    void OnToggleInventory()
    {
        if(inventoryUI.activeSelf == true)
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(true);
        }
    }
    
}
