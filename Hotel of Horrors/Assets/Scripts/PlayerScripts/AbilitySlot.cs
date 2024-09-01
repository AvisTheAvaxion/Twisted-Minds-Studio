using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IPointerClickHandler
{
    InventoryGUI inventoryGUI;
    [SerializeField] TextMeshProUGUI abilityName;

    private void Start()
    {
        inventoryGUI = FindObjectOfType<InventoryGUI>();
    }

    public void UpdateImage(Ability ability)
    {
        if (ability != null)
            abilityName.text = ability.GetInfo().GetName();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryGUI.EquipAbility(transform.GetSiblingIndex());
    }
}
