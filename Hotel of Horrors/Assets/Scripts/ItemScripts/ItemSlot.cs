using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    // Start is called before the first frame update
    void Start()
    {
        itemImage = GetComponent<Image>();
        slotEmptyColor = new Color(1, 1, 1, 0);
        slotFilledColor = new Color(1, 1, 1, 1);
        if (itemHeld != null)
        {
            itemImage.color = slotFilledColor;
            itemImage.sprite = itemHeld.GetSprite();
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

    public void OnPointerExit(PointerEventData eventData)
    {
        expandedMenu.SetActive(false);
    }
}
