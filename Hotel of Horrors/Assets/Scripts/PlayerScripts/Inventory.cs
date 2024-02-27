using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] ItemSlot weaponSlot;
    [SerializeField] ItemSlot freeSlot1;
    [SerializeField] ItemSlot freeSlot2;

    private void Awake()
    {
        inventoryUI.SetActive(true);
        inventoryUI.SetActive(false);
        animator = GetComponent<Animator>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        slots = inventoryUI.GetComponentsInChildren<ItemSlot>().ToList();
        foreach (ItemSlot slot in slots)
        {
            int order = 1;
            slot.gameObject.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = order;
            order++;
        }
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
            SlotInItem(useable);
            if(currentWeapon == null && useable is Weapon)
            {
                currentWeapon = (Weapon)useable;
                EquipWeapon(currentWeapon);
            }
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        weaponPlacement.sprite = currentWeapon.GetSprite();
        overrideController["WeaponAttack"] = currentWeapon.GetWeaponAnimation();
        Debug.Log(overrideController["TestAttackAnim"] = currentWeapon.GetWeaponAnimation());
        //Insert code that is done on equip like moving gameobject beside the player / Swaping Sprites
    }

    public void SlotInItem(Useables item)
    {
        int skipThree = 0;
        foreach(ItemSlot slot in slots)
        {
            if(slot.itemHeld != null || skipThree < 3)
            {
                skipThree++;
                continue;
            }
            else
            {
                slot.itemHeld = item;
                break;
            }
            
        }
    }

    void OnToggleInventory()
    {
        if(inventoryUI.activeSelf == true)
        {
            inventoryUI.SetActive(false);
            Cursor.visible = false;
        }
        else
        {
            inventoryUI.SetActive(true);
            Cursor.visible = true;
        }
    }
    
}
