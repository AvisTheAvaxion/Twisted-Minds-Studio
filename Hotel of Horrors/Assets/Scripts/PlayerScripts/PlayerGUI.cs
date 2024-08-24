using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    PlayerInventory inventory;

    [SerializeField] Image weaponHotbarImage;
    [SerializeField] Image freeSlotHotbarImage;
    [SerializeField] Image mementoHotbarImage;

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public void UpdateHotbarGUI()
    {
        if(inventory.CurrentWeapon != null)
        {
            weaponHotbarImage.enabled = true;
            weaponHotbarImage.sprite = inventory.CurrentWeapon.GetInfo().GetSprite();
        } else
        {
            weaponHotbarImage.enabled = false;
        }

        if (inventory.CurrentItem != null)
        {
            freeSlotHotbarImage.enabled = true;
            freeSlotHotbarImage.sprite = inventory.CurrentItem.GetInfo().GetSprite();
        }
        else
        {
            freeSlotHotbarImage.enabled = false;
        }

        if (inventory.CurrentAbility != null)
        {
            freeSlotHotbarImage.enabled = true;
            freeSlotHotbarImage.sprite = inventory.CurrentAbility.GetInfo().GetSprite();
        }

        if(inventory.CurrentMemento != null)
        {
            mementoHotbarImage.enabled = true;
            mementoHotbarImage.sprite = inventory.CurrentMemento.GetSprite();
        } else
        {
            mementoHotbarImage.enabled = false;
        }
    }
}
