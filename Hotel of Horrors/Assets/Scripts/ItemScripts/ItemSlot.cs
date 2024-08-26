using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int itemIndex { get; private set; }
    Image itemImage;

    Color slotEmptyColor = new Color(1, 1, 1, 0);
    Color slotFilledColor = new Color(1, 1, 1, 1);

    [Header("Item Slot Settings")]
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] bool isWeaponSlot;
    [SerializeField] bool isItemSlot;
    [SerializeField] bool isWeaponEquipSlot;
    [SerializeField] bool isFreeEquipSlot;
    [SerializeField] bool isMementoEquipSlot;
    [SerializeField] bool isWeaponUpgradeInventory;
    public bool IsWeaponEquipSlot { get => isWeaponEquipSlot; }
    public bool IsFreeEquipSlot { get => isFreeEquipSlot; }
    public bool IsMementoEquipSlot { get => isMementoEquipSlot; }

    public bool IsWeaponSlot { get => isWeaponSlot; }
    public bool IsItemSlot { get => isItemSlot; }

    InventoryGUI inventoryGUI;
    WeaponUpgradeGUI weaponUpgradeGUI;

    public void Init() 
    {
        if(isWeaponUpgradeInventory)
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
        if (weaponUpgradeGUI)
        {
            weaponUpgradeGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), isWeaponEquipSlot, transform.position);
        }
        else
        {
            if (isItemSlot || isFreeEquipSlot)
            {
                inventoryGUI.UpdateItemToolTip(transform.parent.GetSiblingIndex(), isFreeEquipSlot, transform.position);
            }
            else if (isWeaponSlot || isWeaponEquipSlot)
            {
                inventoryGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), isWeaponEquipSlot, transform.position);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isWeaponEquipSlot && !isFreeEquipSlot && !isMementoEquipSlot)
        {
            if (isWeaponUpgradeInventory)
            {

            }
            else
            {
                if (isWeaponSlot)
                {
                    inventoryGUI.EquipWeapon(transform.parent.GetSiblingIndex());
                }
                if (isItemSlot)
                {
                    inventoryGUI.EquipItem(transform.parent.GetSiblingIndex());
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isWeaponUpgradeInventory)
            {

            }
            else 
            { 
                if (isWeaponEquipSlot)
                {
                    inventoryGUI.EquipWeapon(-1);
                    inventoryGUI.DisableToolTip();
                }
                else if (isFreeEquipSlot)
                {
                    inventoryGUI.EquipItem(-1);
                    inventoryGUI.DisableToolTip();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isWeaponUpgradeInventory)
        {
            weaponUpgradeGUI.DisableToolTip();
        }
        else
        {
            inventoryGUI.DisableToolTip();
        }
    }
}
