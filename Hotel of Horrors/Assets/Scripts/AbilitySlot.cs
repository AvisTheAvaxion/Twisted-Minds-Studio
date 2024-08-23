using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IPointerClickHandler
{
    Inventory inventory;
    [SerializeField] TextMeshProUGUI abilityName;
    public AbilityInfo abilityInfo;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(abilityInfo != null)
        {
            inventory.EquipPlayerAbility(abilityInfo);
        }
    }
}
