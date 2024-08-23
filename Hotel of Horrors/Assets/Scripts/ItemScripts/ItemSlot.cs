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
    Inventory inventory;

    public void Init() 
    {
        inventory = FindObjectOfType<Inventory>();
        itemImage = GetComponent<Image>();
        amountText = GetComponentInChildren<TextMeshProUGUI>();

        itemIndex = -1;

        UpdateImage();

        if (itemToolTip == null)
        {
            itemToolTip = FindObjectOfType<ItemToolTip>();
        }
    }

    public void UnequipItem()
    {
        //itemHeld = null;
        //UpdateImage();
    }

    public void UpdateImage()
    {
        if(isItemSlot)
        {
            Item item = inventory.GetItem(transform.parent.GetSiblingIndex());
            if (item != null) 
            { 
                amountText.enabled = true;
                itemImage.color = slotFilledColor;
                amountText.text = item.CurrentAmount.ToString();
                itemImage.sprite = item.GetInfo().GetSprite();
            } else
            {
                itemImage.color = slotEmptyColor;
                itemImage.sprite = null;
                amountText.enabled = false;
            }
        } else if (isWeaponSlot)
        {
            WeaponInfo weapon = inventory.GetWeapon(transform.parent.GetSiblingIndex());
            if (weapon != null)
            {
                amountText.enabled = false;
                itemImage.color = slotFilledColor;
                itemImage.sprite = weapon.GetSprite();
            }
            else
            {
                itemImage.color = slotEmptyColor;
                itemImage.sprite = null;
                amountText.enabled = false;
            }
        } else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void UpdateImage(int index)
    {
       
        if (isFreeEquipSlot)
        {
            Item item = inventory.GetItem(index);
            this.itemIndex = index;
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
        else if (isWeaponEquipSlot)
        {
            WeaponInfo weapon = inventory.GetWeapon(index);
            if (weapon != null)
            {
                this.itemIndex = index;
                amountText.enabled = false;
                itemImage.color = slotFilledColor;
                itemImage.sprite = weapon.GetSprite();
            }
            else
            {
                itemImage.color = slotEmptyColor;
                itemImage.sprite = null;
                amountText.enabled = false;
            }
        } else
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
            amountText.enabled = false;
        }
    }
    public void UpdateImage(Sprite sprite)
    {
        if(isFreeEquipSlot)
        {
            if(sprite != null)
            {
                amountText.enabled = false;
                itemImage.color = slotFilledColor;
                itemImage.sprite = sprite;
            } else
            {
                itemImage.color = slotEmptyColor;
                itemImage.sprite = null;
                amountText.enabled = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UseableInfo info = null;
        if(isItemSlot)
        {
            Item item = inventory.GetItem(transform.parent.GetSiblingIndex());
            if (item != null) info = item.GetInfo();
        }
        else if (isWeaponSlot)
        {
            info = inventory.GetWeapon(transform.parent.GetSiblingIndex());
        }
        else if (isFreeEquipSlot)
        {
            Item item = inventory.GetItem(itemIndex);
            if (item != null) info = item.GetInfo();
        }
        else if (isWeaponEquipSlot)
        {
            info = inventory.GetWeapon(itemIndex);
        }

        if (info != null)
        {
            itemToolTip.transform.position = (Vector2)transform.position + new Vector2(50f, 50f);
            itemToolTip.AssignItem(info);

            itemToolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isWeaponEquipSlot && !isFreeEquipSlot && !isMementoEquipSlot)
        {
            if (isWeaponSlot)
            {
                inventory.EquipWeapon(transform.parent.GetSiblingIndex());
            }
            if (isItemSlot)
            {
                inventory.EquipItem(transform.parent.GetSiblingIndex());
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isWeaponEquipSlot)
            {
                inventory.EquipWeapon(-1);
                itemToolTip.gameObject.SetActive(false);
            }
            else if (isFreeEquipSlot)
            {
                inventory.EquipItem(-1);
                itemToolTip.gameObject.SetActive(false);
            }
            else if (isItemSlot)
            {
                inventory.UseItem(transform.parent.GetSiblingIndex(), this);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemToolTip.gameObject.SetActive(false);
    }
}
