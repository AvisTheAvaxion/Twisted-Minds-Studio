using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public Useables itemHeld;
    Image itemImage;
    Color slotEmptyColor = new Color(1, 1, 1, 0);
    Color slotFilledColor = new Color(1, 1, 1, 1);

    [Header("Item Slot Settings")]
    [SerializeField] bool isWeaponSlot;
    [SerializeField] bool isFreeSlot;
    [SerializeField] bool isMementoSlot;
    public bool IsWeaponSlot { get => isWeaponSlot; }
    public bool IsFreeSlot { get => isFreeSlot; }
    public bool IsMementoSlot { get => isMementoSlot; }

    [Header("Item Tool Tip Reference")]
    [SerializeField] ItemToolTip itemToolTip;
    Inventory inventory;

    bool firstStart = true;
    // Start is called before the first frame update
    void Start()
    {
        firstStart = false;
        inventory = FindObjectOfType<Inventory>();
        itemImage = GetComponent<Image>();
        
        if (itemHeld != null)
            UpdateImage();
        else
            itemImage.color = slotEmptyColor;

        if(itemToolTip == null)
        {
            itemToolTip = FindObjectOfType<ItemToolTip>();
        }
    }

    private void OnEnable()
    {
        if (itemHeld != null && firstStart == false)
        {
            UpdateImage();
        }
    }

    public void UpdateImage()
    {
        if (itemHeld == null)
        {
            itemImage.color = slotEmptyColor;
            itemImage.sprite = null;
        }
        else
        {
            itemImage.color = slotFilledColor;
            itemImage.sprite = itemHeld.GetSprite();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemHeld != null)
        {
            itemToolTip.transform.position = (Vector2)transform.position + new Vector2(50f, 50f);
            itemToolTip.AssignItem(itemHeld);

            itemToolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && itemHeld != null && !isWeaponSlot && !isFreeSlot && !isMementoSlot)
        {
            if (itemHeld.GetType() == typeof(Weapon))
            {
                inventory.EquipWeapon((Weapon)itemHeld);
            }
            if (itemHeld.GetType() != typeof(Weapon) && Input.GetKey(KeyCode.Q))
            {
                //inventory.EquipFreeItem(itemHeld, 0);
            }
            if (itemHeld.GetType() != typeof(Weapon) && itemHeld.GetType() != typeof(Mementos) && Input.GetKey(KeyCode.E))
            {
                inventory.EquipFreeItem(itemHeld);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right && itemHeld != null)
        {
            if (isWeaponSlot)
            {
                inventory.EquipWeapon(null);
                itemToolTip.gameObject.SetActive(false);
            } 
            else if (isFreeSlot)
            {
                itemHeld = null;
                UpdateImage();
                itemToolTip.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemToolTip.gameObject.SetActive(false);
    }
}
