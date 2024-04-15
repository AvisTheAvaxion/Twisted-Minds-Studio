using Attacks;
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

    [Header("References")]
    [SerializeField] UIDisplayContainer uiDisplay;
    ItemSlot weaponSlot;
    ItemSlot freeSlot;
    ItemSlot mementoSlot;
    List<ItemSlot> slots;

    AttackController attackController;

    private void Awake()
    {
        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();

        uiDisplay.InventoryUI.SetActive(true);
        uiDisplay.InventoryUI.SetActive(false);
        attackController = GetComponent<AttackController>();

        slots = uiDisplay.InventoryUI.GetComponentsInChildren<ItemSlot>().ToList();
        foreach (ItemSlot slot in slots)
        {
            //int order = 1;
            //slot.gameObject.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = order;
            //order++;

            if (slot.IsWeaponSlot)
                weaponSlot = slot;
            else if (freeSlot == null && slot.IsFreeSlot)
                freeSlot = slot;
            else if (mementoSlot == null && slot.IsMementoSlot)
                mementoSlot = slot;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ItemData>())
        {
            //Debug.Log("PickUp Get");
            Useables useable = collision.GetComponent<ItemData>().GetItemData();
            //Debug.Log(useable.GetDescription());
            collision.gameObject.SetActive(false);
            inventoryItems.Add(useable);
            SlotInItem(useable);
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        attackController.Equip(weapon);
        weaponSlot.itemHeld = currentWeapon = weapon;
        weaponSlot.UpdateImage();

        if(weapon == null)
        {
            uiDisplay.WeaponHotbarImage.sprite = null;
        }
        else
        {
            uiDisplay.WeaponHotbarImage.sprite = weapon.GetSprite();
        }
    }
    public void EquipFreeItem(Useables item)
    {
        freeSlot.itemHeld = itemOne = item;
        freeSlot.UpdateImage();

        if (item == null)
        {
            uiDisplay.FreeSlotHotbarImage.sprite = null;
        }
        else
        {
            uiDisplay.FreeSlotHotbarImage.sprite = item.GetSprite();
        }
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
        if(uiDisplay.InventoryUI.activeSelf == true)
        {
            uiDisplay.InventoryUI.SetActive(false);
            Cursor.visible = false;
        }
        else
        {
            uiDisplay.InventoryUI.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void UpdatePlayerMode()
    {
        if (currentWeapon == null)
            attackController.ChangeAttackMode(AttackModes.None);
        else
            attackController.ChangeAttackMode(currentWeapon.GetWeaponMode());
    }
    
}
