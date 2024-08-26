using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgradeGUI : MonoBehaviour
{
    PlayerInventory inventory;

    [SerializeField] GameObject weaponsContainer;
    [SerializeField] ItemToolTip itemToolTip;

    ItemSlot[] weaponSlots;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();

        weaponSlots = weaponsContainer.GetComponentsInChildren<ItemSlot>();

        weaponsContainer.SetActive(true);

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].Init();
        }
    }

    public void UpdateGUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            Weapon weapon = inventory.GetWeapon(i);
            weaponSlots[i].UpdateImage(weapon);
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
}
