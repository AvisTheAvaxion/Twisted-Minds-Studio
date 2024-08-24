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
    public bool IsWeaponEquipSlot { get => isWeaponEquipSlot; }
    public bool IsFreeEquipSlot { get => isFreeEquipSlot; }
    public bool IsMementoEquipSlot { get => isMementoEquipSlot; }

    public bool IsWeaponSlot { get => isWeaponSlot; }
    public bool IsItemSlot { get => isItemSlot; }

    [Header("Item Tool Tip Reference")]
    [SerializeField] ItemToolTip itemToolTip;
    InventoryGUI inventoryGUI;

    public void Init() 
    {
        inventoryGUI = FindObjectOfType<InventoryGUI>();
        itemImage = GetComponent<Image>();
        amountText = GetComponentInChildren<TextMeshProUGUI>();

        itemIndex = -1;

        //UpdateImage();

        if (itemToolTip == null)
        {
            itemToolTip = FindObjectOfType<ItemToolTip>();
        }
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
        //UseableInfo info = null;
        if(isItemSlot || isFreeEquipSlot)
        {
            inventoryGUI.UpdateItemToolTip(transform.parent.GetSiblingIndex(), isFreeEquipSlot, transform.position);
            //Item item = inventory.GetItem(transform.parent.GetSiblingIndex());
            //if (item != null) info = item.GetInfo();
        }
        else if (isWeaponSlot || isWeaponEquipSlot)
        {
            inventoryGUI.UpdateWeaponToolTip(transform.parent.GetSiblingIndex(), isWeaponEquipSlot, transform.position);
            //Weapon weapon = inventory.GetWeapon(transform.parent.GetSiblingIndex());
            //if (weapon != null) info = weapon.GetInfo();
        }
        /*else if (isFreeEquipSlot)
        {
            Item item = inventory.GetItem(itemIndex);
            if (item != null) info = item.GetInfo();
        }
        else if (isWeaponEquipSlot)
        {
            Weapon weapon = inventory.GetWeapon(itemIndex);
            if (weapon != null) info = weapon.GetInfo();
        }*/

        /*if (info != null)
        {
            itemToolTip.transform.position = (Vector2)transform.position + new Vector2(50f, 50f);
            itemToolTip.AssignItem(info);

            itemToolTip.gameObject.SetActive(true);
        }*/
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isWeaponEquipSlot && !isFreeEquipSlot && !isMementoEquipSlot)
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
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isWeaponEquipSlot)
            {
                inventoryGUI.EquipWeapon(-1);
                itemToolTip.gameObject.SetActive(false);
            }
            else if (isFreeEquipSlot)
            {
                inventoryGUI.EquipItem(-1);
                itemToolTip.gameObject.SetActive(false);
            }
            /*else if (isItemSlot)
            {
                inventory.UseItem(transform.parent.GetSiblingIndex(), this);
            }*/
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryGUI.DisableToolTip();
        //itemToolTip.gameObject.SetActive(false);
    }
}
