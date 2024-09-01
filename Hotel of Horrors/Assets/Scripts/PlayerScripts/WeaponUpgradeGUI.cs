using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponUpgradeGUI : MonoBehaviour
{
    PlayerInventory inventory;
    PlayerGUI playerGUI;

    [SerializeField] GameObject weaponsContainer;
    [SerializeField] GameObject upgradeGUI;
    [SerializeField] TMP_Text costText;
    [SerializeField] ItemToolTip itemToolTip;

    ItemSlot upgradeSlot;
    ItemSlot[] ingredientSlots;

    ItemSlot[] weaponSlots;

    public int[] ingredientIndices { get; private set; }

    Weapon upgradedWeapon;
    int upgradeEECost;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        playerGUI = FindObjectOfType<PlayerGUI>();

        weaponSlots = weaponsContainer.GetComponentsInChildren<ItemSlot>();

        weaponsContainer.SetActive(true);

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].Init();
        }

        ingredientSlots = new ItemSlot[2];
        ItemSlot[] upgradeSlots = upgradeGUI.GetComponentsInChildren<ItemSlot>();
        ingredientSlots[0] = upgradeSlots[0];
        ingredientSlots[1] = upgradeSlots[1];
        upgradeSlot = upgradeSlots[2];

        ingredientSlots[0].Init();
        ingredientSlots[1].Init();
        upgradeSlot.Init();

        ingredientIndices = new int[2];
        ingredientIndices[0] = -1;
        ingredientIndices[1] = -1;
    }

    public void UpdateGUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            Weapon weapon = inventory.GetWeapon(i);
            weaponSlots[i].UpdateImage(weapon);
        }

        for (int i = 0; i < ingredientIndices.Length; i++)
        {
            Weapon weapon = inventory.GetWeapon(ingredientIndices[i]);
            ingredientSlots[i].UpdateImage(weapon);
        }

        upgradeSlot.UpdateImage(upgradedWeapon);

        if(upgradedWeapon != null)
        {
            costText.text = "EE Cost: " + upgradeEECost;
        }
        else
        {
            costText.text = "";
        }
    }

    public void UpdateWeaponToolTip(int index, ItemSlot.ItemSlotType slotType, Vector2 position)
    {
        Weapon weapon = null;
        switch(slotType)
        {
            case ItemSlot.ItemSlotType.WeaponUpgradeInventorySlot:
                weapon = inventory.GetWeapon(index);
                break;
            case ItemSlot.ItemSlotType.WeaponUpgradeIngredientSlot:
                weapon = inventory.GetWeapon(ingredientIndices[index]);
                break;
            case ItemSlot.ItemSlotType.WeaponUpgradeSlot:
                weapon = upgradedWeapon;
                break;
        }

        if (weapon != null)
        {
            itemToolTip.AssignWeapon(weapon);
            itemToolTip.transform.position = position + new Vector2(50f, 50f);
            itemToolTip.gameObject.SetActive(true);
        }
    }
    public void DisableToolTip()
    {
        itemToolTip.gameObject.SetActive(false);
    }

    public void SetUpgradeIngredientSlot(int weaponIndex)
    {
        if(ingredientIndices[0] < 0 && weaponIndex != ingredientIndices[1])
        {
            ingredientIndices[0] = weaponIndex;
        } 
        else if(weaponIndex != ingredientIndices[0])
        {
            ingredientIndices[1] = weaponIndex;
        }

        if(ingredientIndices[0] >= 0 && ingredientIndices[1] >= 0)
        {
            Weapon weapon1 = inventory.GetWeapon(ingredientIndices[0]);
            Weapon weapon2 = inventory.GetWeapon(ingredientIndices[1]);

            if (weapon1.info.GetName() == weapon2.info.GetName() &&
                weapon1.currentLevel >= weapon2.currentLevel)
            {
                upgradedWeapon = weapon1.UpgradePreview(weapon2);

                upgradeEECost = upgradedWeapon != null ? weapon1.GetUpgradeCost(weapon2) : 0;
            }
            else if (weapon1.info.GetName() == weapon2.info.GetName() &&
                weapon1.currentLevel < weapon2.currentLevel)
            {
                upgradedWeapon = weapon2.UpgradePreview(weapon1);

                upgradeEECost = upgradedWeapon != null ? weapon2.GetUpgradeCost(weapon1) : 0;
            }
            else
            {
                upgradedWeapon = null;
                upgradeEECost = 0;
            }
        }

        UpdateGUI();
    }
    public void UnSetUpgradeIngredienSlot(int ingredientSlotIndex)
    {
        ingredientIndices[ingredientSlotIndex] = -1;
        upgradedWeapon = null;
        upgradeEECost = 0;

        UpdateGUI();
    }

    public void ConfirmUpgrade()
    {
        if(upgradedWeapon != null && inventory.emotionalEnergy >= upgradeEECost)
        {
            //Weapon weapon1 = inventory.GetWeapon(ingredientIndices[0]);
            //Weapon weapon2 = inventory.GetWeapon(ingredientIndices[1]);

            inventory.RemoveWeapon(ingredientIndices[0]);
            inventory.RemoveWeapon(ingredientIndices[1]);

            inventory.AddWeapon(upgradedWeapon);

            inventory.SubtractEmotionialEnergy(upgradeEECost);

            ingredientIndices[0] = -1;
            ingredientIndices[1] = -1;

            upgradedWeapon = null;
            upgradeEECost = 0;

            UpdateGUI();
            playerGUI.UpdateEmotionalEnergy();
        }
    }

    public void ClearUpgradeSlots()
    {
        ingredientIndices[0] = -1;
        ingredientIndices[1] = -1;

        upgradedWeapon = null;
        upgradeEECost = 0;
    }
}
