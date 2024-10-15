using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityGUI : MonoBehaviour
{
    PlayerInventory inventory;
    PlayerGUI playerGUI;

    [SerializeField] AbilityInfo[] possibleAbilities;
    [SerializeField] GameObject abilityGUISlotPrefab;
    [SerializeField] GameObject selectedAbilityInfo;
    [SerializeField] Transform[] abilityFloorContainers;
    [SerializeField] TMP_Text buttonText;
    [SerializeField] Image selectedAbilityImage;
    [SerializeField] TMP_Text selectedAbilityName;
    [SerializeField] TMP_Text selectedAbilityDescription;
    [SerializeField] TMP_Text cooldownText;
    [SerializeField] TMP_Text eeCostText;

    AbilityGUISlot[] abilityGUISlots;

    int selectedAbilityIndex;

    bool viewSelectedAbilityInfo = false;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        playerGUI = FindObjectOfType<PlayerGUI>();

        if(abilityGUISlots == null)
        {
            InitializeGUISlots();
        }
    }

    void InitializeGUISlots()
    {
        abilityGUISlots = new AbilityGUISlot[possibleAbilities.Length];

        for (int i = 0; i < possibleAbilities.Length; i++)
        {
            int unlockFloor = possibleAbilities[i].GetUnlockFloor();
            GameObject go = Instantiate(abilityGUISlotPrefab, abilityFloorContainers[unlockFloor - 1]);
            AbilityGUISlot abilityGUISlot = go.GetComponent<AbilityGUISlot>();
            if (abilityGUISlot != null)
            {
                abilityGUISlot.SetInfoIndex(i);
                abilityGUISlot.SetAbilityName(possibleAbilities[i].GetName());

                int inventoryIndex = inventory.HasAbility(possibleAbilities[i].GetName());
                abilityGUISlot.SetInventoryIndex(inventoryIndex);

                abilityGUISlots[i] = abilityGUISlot;
            }
        }

        selectedAbilityIndex = 0;

        UpdateGUI();
    }

    public void SetSelectedAbility(int index)
    {
        AbilityGUISlot slot = abilityGUISlots[index];
        viewSelectedAbilityInfo = false;

        if (slot != null)
        {
            viewSelectedAbilityInfo = true;
            selectedAbilityIndex = index;
        }

        UpdateGUI();
    }

    public void AbilityInteract()
    {
        AbilityGUISlot slot = abilityGUISlots[selectedAbilityIndex];

        if (slot.inventoryIndex >= 0)
        {
            UpgradeAbility(selectedAbilityIndex);
        }
        else
        {
            UnlockAbility(selectedAbilityIndex);
        }

        UpdateGUI();
    }

    public void UpdateGUI()
    {
        for (int i = 0; i < abilityGUISlots.Length; i++)
        {
            if (abilityGUISlots[i].inventoryIndex >= 0)
            {
                abilityGUISlots[i].UpdateImage(inventory.GetAbility(abilityGUISlots[i].inventoryIndex));
            }
            else
            {
                abilityGUISlots[i].UpdateImage(possibleAbilities[i]);
            }
        }

        AbilityGUISlot slot = abilityGUISlots[selectedAbilityIndex];

        selectedAbilityInfo.SetActive(viewSelectedAbilityInfo);

        if (slot != null)
        {
            AbilityInfo info = possibleAbilities[slot.infoIndex];
            selectedAbilityDescription.text = info.GetDescription();
            selectedAbilityName.text = info.GetName();
            selectedAbilityImage.sprite = info.GetSprite();

            if (slot.inventoryIndex >= 0)
            {
                Ability ability = inventory.GetAbility(slot.inventoryIndex);
                cooldownText.text = "Cooldown: " + ability.cooldown.ToString();
                eeCostText.text = "EE Cost: " + ability.GetUpgradeEECost().ToString();
                buttonText.text = "Upgrade";
            }
            else
            {
                cooldownText.text = "Cooldown: " + info.GetCooldown().ToString();
                eeCostText.text = "EE Cost: " + info.GetUnlockEECost().ToString();
                buttonText.text = "Unlock";
            }
        }
    }

    public void UnlockAbility(int index)
    {
        AbilityInfo abilityInfo = possibleAbilities[index];

        if(inventory.emotionalEnergy >= abilityInfo.GetUnlockEECost())
        {
            Ability ability = new Ability(abilityInfo);

            int inventoryIndex = inventory.AddPlayerAbility(ability);

            inventory.SubtractEmotionialEnergy(ability.GetUnlockEECost());
            abilityGUISlots[index].SetInventoryIndex(inventoryIndex);

            UpdateGUI();
        }
    }

    public void UpgradeAbility(int index)
    {
        Ability ability = inventory.GetAbility(abilityGUISlots[index].inventoryIndex);
        if(ability != null && inventory.emotionalEnergy >= ability.GetUpgradeEECost() && ability.CanUpgrade())
        {
            ability.Upgrade(inventory.emotionalEnergy);

            inventory.SubtractEmotionialEnergy(ability.GetUpgradeEECost());

            UpdateGUI();
            playerGUI.UpdateHotbarGUI();
        }
    }

    public void CloseSelectedAbilityInfo()
    {
        viewSelectedAbilityInfo = false;

        UpdateGUI();
    }
}
