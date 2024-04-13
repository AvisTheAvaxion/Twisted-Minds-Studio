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
    [SerializeField] GameObject inventoryUI;
    ItemSlot weaponSlot;
    ItemSlot freeSlot1;
    ItemSlot freeSlot2;
    List<ItemSlot> slots;

    AttackController attackController;

    private void Awake()
    {
        inventoryUI.SetActive(true);
        inventoryUI.SetActive(false);
        attackController = GetComponent<AttackController>();

        slots = inventoryUI.GetComponentsInChildren<ItemSlot>().ToList();
        foreach (ItemSlot slot in slots)
        {
            int order = 1;
            slot.gameObject.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = order;
            order++;

            if (slot.IsWeaponSlot)
                weaponSlot = slot;
            else if (freeSlot1 == null && slot.IsFreeSlot)
                freeSlot1 = slot;
            else if (freeSlot1 != null && freeSlot2 == null && slot.IsFreeSlot)
                freeSlot2 = slot;
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
    }
    public void EquipFreeItem(Useables item, int slotNum)
    {
        if(slotNum == 0)
        {
            freeSlot1.itemHeld = itemOne = item;
            freeSlot1.UpdateImage();
        }
        else
        {
            freeSlot2.itemHeld = itemTwo = item;
            freeSlot2.UpdateImage();
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

    public void UpdatePlayerMode()
    {
        if (currentWeapon == null)
            attackController.ChangeAttackMode(AttackModes.None);
        else
            attackController.ChangeAttackMode(currentWeapon.GetWeaponMode());
    }
    
}
