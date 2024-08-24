using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IPointerClickHandler
{
    PlayerInventory inventory;
    [SerializeField] TextMeshProUGUI abilityName;
    public Ability ability;

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(ability != null)
        {
            //inventory.EquipPlayerAbility(ability);
        }
    }
}
