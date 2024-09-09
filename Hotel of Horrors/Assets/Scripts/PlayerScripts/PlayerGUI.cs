using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    PlayerInventory inventory;

    [SerializeField] TMP_Text emotionalEnergyText;
    [SerializeField] Image weaponHotbarImage;
    [SerializeField] Image freeSlotHotbarImage;
    [SerializeField] Image mementoHotbarImage;
    [SerializeField] Image weaponCooldownImage;
    [SerializeField] Image freeSlotCooldownImage;
    [SerializeField] Image mementoCooldownImage;

    [SerializeField] TextMeshProUGUI floorNumText;
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] Transform effectsIconHolder;
    [SerializeField] GameObject effectsIconPrefab;

    [SerializeField] HeartsController heartsController;

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();

        weaponCooldownImage.fillAmount = 0;
        freeSlotCooldownImage.fillAmount = 0;
        mementoCooldownImage.fillAmount = 0;
    }

    private void Update()
    {
        
    }

    public void UpdateEmotionalEnergy()
    {
        emotionalEnergyText.text = inventory.emotionalEnergy.ToString();
    }

    public void UpdateHotbarGUI()
    {
        if(inventory.CurrentWeapon != null)
        {
            weaponHotbarImage.enabled = true;
            weaponHotbarImage.sprite = inventory.CurrentWeapon.GetInfo().GetDisplaySprite();
        } else
        {
            weaponHotbarImage.enabled = false;
        }

        if (inventory.CurrentItem != null)
        {
            freeSlotHotbarImage.enabled = true;
            freeSlotHotbarImage.sprite = inventory.CurrentItem.GetInfo().GetDisplaySprite();
        }
        else
        {
            freeSlotHotbarImage.enabled = false;
        }

        if (inventory.CurrentAbility != null)
        {
            freeSlotHotbarImage.enabled = true;
            freeSlotHotbarImage.sprite = inventory.CurrentAbility.GetInfo().GetDisplaySprite();
        }

        if(inventory.CurrentMemento != null)
        {
            mementoHotbarImage.enabled = true;
            mementoHotbarImage.sprite = inventory.CurrentMemento.GetDisplaySprite();
        } else
        {
            mementoHotbarImage.enabled = false;
        }
    }

    public void UpdateWeaponCooldown(float t)
    {
        weaponCooldownImage.fillAmount = t;
    }
    public void UpdateFreeSlotCooldown(float t)
    {
        freeSlotCooldownImage.fillAmount = t;
    }
    public void UpdateMementoCooldown(float t)
    {
        mementoCooldownImage.fillAmount = t;
    }

    public GameObject CreateEffectIcon()
    {
        return Instantiate(effectsIconPrefab, effectsIconHolder);
    }

    public HeartsController GetHeartsController()
    {
        return heartsController;
    }
}
