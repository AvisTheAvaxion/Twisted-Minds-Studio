using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryGUI : MonoBehaviour
{
    PlayerInventory inventory;

    [SerializeField] PlayerGUI playerGUI;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject weaponsContainer;
    [SerializeField] GameObject itemsContainer;
    [SerializeField] GameObject abilitiesContainer;
    [SerializeField] ItemToolTip itemToolTip;

    ItemSlot weaponSlot;
    ItemSlot freeSlot;

    ItemSlot[] itemSlots;
    ItemSlot[] weaponSlots;
    List<AbilitySlot> abilitySlots;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();

        abilitySlots = new List<AbilitySlot>();
        abilitySlots = abilitiesContainer.GetComponentsInChildren<AbilitySlot>().ToList<AbilitySlot>();
        itemSlots = itemsContainer.GetComponentsInChildren<ItemSlot>();
        weaponSlots = weaponsContainer.GetComponentsInChildren<ItemSlot>();

        itemsContainer.SetActive(false);
        abilitiesContainer.SetActive(false);
        weaponsContainer.SetActive(true);

        ItemSlot[] slots = inventoryUI.GetComponentsInChildren<ItemSlot>();
        foreach (ItemSlot slot in slots)
        {
            slot.Init();
            if (slot.IsWeaponEquipSlot)
                weaponSlot = slot;
            else if (freeSlot == null && slot.IsFreeEquipSlot)
                freeSlot = slot;
        }

        /*for (int i = 0; i < abilitySlots.Count; i++)
        {
            inventory.AddPlayerAbility(abilitySlots[i].ability);
        }*/

        inventoryUI.SetActive(false);
    }

    void UpdateGUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Item item = inventory.GetItem(i);
            itemSlots[i].UpdateImage(item);
        }
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            Weapon weapon = inventory.GetWeapon(i);
            weaponSlots[i].UpdateImage(weapon);
        }

        weaponSlot.UpdateImage(inventory.CurrentWeapon, inventory.currentWeaponIndex);

        freeSlot.UpdateImage(inventory.CurrentItem, inventory.currentItemIndex);

        if (inventory.CurrentAbility != null)
        {
            freeSlot.UpdateImage(inventory.CurrentAbility, inventory.currentAbilityIndex);
        }
    }

    public void EquipWeapon(int index) 
    {
        inventory.EquipWeapon(index);

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }
    public void EquipItem(int index)
    {
        inventory.EquipItem(index);

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }
    public void EquipAbility(int index)
    {
        inventory.EquipPlayerAbility(index);

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }

    public void UpdateItemToolTip(int index, bool equippedSlot, Vector2 position)
    {
        Item item = null;
        if (equippedSlot)
        {
            item = inventory.CurrentItem;
        } else
        {
            item = inventory.GetItem(index);
        }

        if (item != null)
        {
            itemToolTip.AssignItem(item.GetInfo());
            itemToolTip.transform.position = position + new Vector2(50f, 50f);
            itemToolTip.gameObject.SetActive(true);
        }
    }
    public void UpdateWeaponToolTip(int index, bool equippedSlot, Vector2 position)
    {
        Weapon weapon = null;
        if (equippedSlot)
        {
            weapon = inventory.CurrentWeapon;
        }
        else
        {
            weapon = inventory.GetWeapon(index);
        }

        if (weapon != null)
        {
            itemToolTip.AssignItem(weapon.GetInfo());
            itemToolTip.transform.position = position + new Vector2(50f, 50f);
            itemToolTip.gameObject.SetActive(true);
        }
    }
    public void DisableToolTip()
    {
        itemToolTip.gameObject.SetActive(false);
    }

    void OnToggleInventory()
    {
        if (inventoryUI.activeSelf == true)
        {
            inventoryUI.SetActive(false);
            Cursor.visible = false;
        }
        else
        {
            inventoryUI.SetActive(true);
            Cursor.visible = true;

            UpdateGUI();
        }
    }
}
