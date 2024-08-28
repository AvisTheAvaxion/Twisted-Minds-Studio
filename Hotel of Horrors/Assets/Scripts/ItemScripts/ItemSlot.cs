using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum ItemSlotType
    {
        WeaponSlot, ItemSlot, WeaponEquipSlot, FreeEquipSlot, WeaponUpgradeInventorySlot, WeaponUpgradeIngredientSlot, WeaponUpgradeSlot
    }

    public int itemIndex { get; private set; }
    Image itemImage;

    Color slotEmptyColor = new Color(1, 1, 1, 0);
    Color slotFilledColor = new Color(1, 1, 1, 1);

    [Header("Item Slot Settings")]
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] ItemSlotType slotType = ItemSlotType.ItemSlot;

    public ItemSlotType SlotType { get => slotType; }

    InventoryGUI inventoryGUI;
    WeaponUpgradeGUI weaponUpgradeGUI;

    public void Init() 
    {
        if(slotType == ItemSlotType.WeaponUpgradeInventorySlot || slotType == ItemSlotType.WeaponUpgradeIngredientSlot || slotType == ItemSlotType.WeaponUpgradeSlot)
        {
            weaponUpgradeGUI = FindObjectOfType<WeaponUpgradeGUI>();
        } else
        {
            inventoryGUI = FindObjectOfType<InventoryGUI>();
        }
        itemImage = GetComponent<Image>();
        amountText = GetComponentInChildren<TextMeshProUGUI>();

        itemIndex = -1;
    }

    public void UpdateImage(Item item)
    {
        if (item != null)
        {
            amountText.enabled = true;
            itemImage.color = slotFilledColor;
            amountText.text = item.CurrentAmount.ToString();
            itemImage.sprite = item.GetInfo().GetSprite();
        }
        else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void UpdateImage(Item item, int index)
    {
        if (item != null)
        {
            this.itemIndex = index;

            amountText.enabled = true;
            itemImage.color = slotFilledColor;
            amountText.text = item.CurrentAmount.ToString();
            itemImage.sprite = item.GetInfo().GetSprite();
        }
        else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void UpdateImage(Weapon weapon)
    {
        if (weapon != null)
        {
            amountText.enabled = false;
            itemImage.color = slotFilledColor;
            itemImage.sprite = weapon.GetInfo().GetSprite();
        }
        else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void UpdateImage(Weapon weapon, int index)
    {
        if (weapon != null)
        {
            this.itemIndex = index;

            amountText.enabled = false;
            itemImage.color = slotFilledColor;
            itemImage.sprite = weapon.GetInfo().GetSprite();
        }
        else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }

    public void UpdateImage(Ability ability)
    {
        if (ability != null)
        {
            amountText.enabled = false;
            itemImage.color = slotFilledColor;
            itemImage.sprite = ability.GetInfo().GetSprite();
        }
        else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void UpdateImage(Ability ability, int index)
    {
        if (ability != null)
        {
            this.itemIndex = index;

            amountText.enabled = false;
            itemImage.color = slotFilledColor;
            itemImage.sprite = ability.GetInfo().GetSprite();
        }
        else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch(slotType)
        {
            case ItemSlotType.WeaponUpgradeInventorySlot:
                weaponUpgradeGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), slotType, transform.position);
                break;
            case ItemSlotType.WeaponUpgradeIngredientSlot:
                weaponUpgradeGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), slotType, transform.position);
                break;
            case ItemSlotType.WeaponUpgradeSlot:
                weaponUpgradeGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), slotType, transform.position);
                break;
            case ItemSlotType.ItemSlot:
                inventoryGUI.UpdateItemToolTip(transform.parent.GetSiblingIndex(), false, transform.position);
                break;
            case ItemSlotType.FreeEquipSlot:
                inventoryGUI.UpdateItemToolTip(transform.parent.GetSiblingIndex(), true, transform.position);
                break;
            case ItemSlotType.WeaponSlot:
                inventoryGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), false, transform.position);
                break;
            case ItemSlotType.WeaponEquipSlot:
                inventoryGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), true, transform.position);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            switch (slotType)
            {
                case ItemSlotType.WeaponUpgradeInventorySlot:
                    weaponUpgradeGUI.SetUpgradeIngredientSlot(transform.parent.GetSiblingIndex());
                    break;
                case ItemSlotType.WeaponUpgradeIngredientSlot:
                    break;
                case ItemSlotType.WeaponUpgradeSlot:
                    weaponUpgradeGUI.ConfirmUpgrade();
                    break;
                case ItemSlotType.ItemSlot:
                    inventoryGUI.EquipItem(transform.parent.GetSiblingIndex());
                    break;
                case ItemSlotType.FreeEquipSlot:
                    break;
                case ItemSlotType.WeaponSlot:
                    inventoryGUI.EquipWeapon(transform.parent.GetSiblingIndex());
                    break;
                case ItemSlotType.WeaponEquipSlot:
                    break;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            switch (slotType)
            {
                case ItemSlotType.WeaponUpgradeInventorySlot:
                    break;
                case ItemSlotType.WeaponUpgradeIngredientSlot:
                    weaponUpgradeGUI.UnSetUpgradeIngredienSlot(transform.parent.parent.GetSiblingIndex());
                    break;
                case ItemSlotType.WeaponUpgradeSlot:
                    break;
                case ItemSlotType.ItemSlot:
                    break;
                case ItemSlotType.FreeEquipSlot:
                    inventoryGUI.EquipItem(-1);
                    inventoryGUI.DisableToolTip();
                    break;
                case ItemSlotType.WeaponSlot:
                    break;
                case ItemSlotType.WeaponEquipSlot:
                    inventoryGUI.EquipWeapon(-1);
                    inventoryGUI.DisableToolTip();
                    break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (slotType)
        {
            case ItemSlotType.WeaponUpgradeInventorySlot:
                weaponUpgradeGUI.DisableToolTip();
                break;
            case ItemSlotType.WeaponUpgradeIngredientSlot:
                weaponUpgradeGUI.DisableToolTip();
                break;
            case ItemSlotType.WeaponUpgradeSlot:
                weaponUpgradeGUI.DisableToolTip();
                break;
            case ItemSlotType.ItemSlot:
                inventoryGUI.DisableToolTip();
                break;
            case ItemSlotType.FreeEquipSlot:
                inventoryGUI.DisableToolTip();
                break;
            case ItemSlotType.WeaponSlot:
                inventoryGUI.DisableToolTip();
                break;
            case ItemSlotType.WeaponEquipSlot:
                inventoryGUI.DisableToolTip();
                break;
        }
    }
}
