using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public Useables itemHeld;
    Image itemImage;
    Color slotEmptyColor;
    Color slotFilledColor;

    [Space(10), Header("ExpandedMenuVariables")]
    GameObject expandedMenu;
    TextMeshProUGUI itemName;
    TextMeshProUGUI itemDesc;
    TextMeshProUGUI itemstat1;
    TextMeshProUGUI itemstat2;
    TextMeshProUGUI itemstat3;
    [SerializeField] bool IsImportantSlots;
    ItemSlot weaponSlot;
    ItemSlot freeSlot1;
    ItemSlot freeSlot2;
    [SerializeField] GameObject importantSlotParent;
    Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        itemImage = GetComponent<Image>();
        if (!IsImportantSlots)
        {
            importantSlotParent = this.transform.parent
                .transform.parent
                .transform.parent
                .transform.parent
                .transform.GetChild(0).gameObject;
            weaponSlot = importantSlotParent.transform.GetChild(0).GetComponentInChildren<ItemSlot>();
            freeSlot1 = importantSlotParent.transform.GetChild(1).GetComponentInChildren<ItemSlot>();
            freeSlot2 = importantSlotParent.transform.GetChild(2).GetComponentInChildren<ItemSlot>();
        }
        slotEmptyColor = new Color(1, 1, 1, 0);
        slotFilledColor = new Color(1, 1, 1, 1);
        if (itemHeld != null)
        {
            UpdateImage();
        }
        else
        {
            itemImage.color = slotEmptyColor;
        }
        expandedMenu = transform.GetChild(0).gameObject;
        itemName = transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemDesc = transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        itemstat1 = transform.GetChild(0).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        itemstat2 = transform.GetChild(0).transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        itemstat3 = transform.GetChild(0).transform.GetChild(4).GetComponent<TextMeshProUGUI>();
    }

    public void UpdateImage()
    {
        itemImage.color = slotFilledColor;
        itemImage.sprite = itemHeld.GetSprite();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemHeld != null)
        {
            itemstat1.gameObject.SetActive(true);
            itemstat2.gameObject.SetActive(true);
            itemstat3.gameObject.SetActive(true);

            itemName.text = itemHeld.GetName();
            itemDesc.text = itemHeld.GetDescription();
            if (itemHeld.GetType() == typeof(Weapon))
            {
                Weapon heldWeapon = (Weapon)itemHeld;
                itemstat1.text = $"Weapon Type: {heldWeapon.GetWeaponMode()}";
                itemstat2.text = $"Damage: {heldWeapon.GetDamage()}";
                itemstat3.text = $"Attack Speed: {heldWeapon.GetAttackSpeed()}";
            }
            else if (itemHeld.GetType() == typeof(Item))
            {
                Item heldItem = (Item)itemHeld;
                itemstat1.text = $"Potency: {heldItem.GetPotency()}";
                itemstat2.gameObject.SetActive(false);
                itemstat3.gameObject.SetActive(false);
            }
            else
            {
                itemstat1.gameObject.SetActive(false);
                itemstat2.gameObject.SetActive(false);
                itemstat3.gameObject.SetActive(false);
            }

            expandedMenu.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && itemHeld != null)
        {
            if (itemHeld.GetType() == typeof(Weapon))
            {
                weaponSlot.itemHeld = this.itemHeld;
                weaponSlot.UpdateImage();
                inventory.currentWeapon = (Weapon)this.itemHeld;
            }
            if (itemHeld.GetType() != typeof(Weapon) && Input.GetKey(KeyCode.Q))
            {
                freeSlot1.itemHeld = this.itemHeld;
                freeSlot1.UpdateImage();
                inventory.itemOne = this.itemHeld;
            }
            if (itemHeld.GetType() != typeof(Weapon) && Input.GetKey(KeyCode.E))
            {
                freeSlot2.itemHeld = this.itemHeld;
                freeSlot2.UpdateImage();
                inventory.itemTwo = this.itemHeld;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        expandedMenu.SetActive(false);
    }
}
