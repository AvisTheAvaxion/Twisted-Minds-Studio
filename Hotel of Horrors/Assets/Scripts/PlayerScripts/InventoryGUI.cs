using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryGUI : MonoBehaviour
{
    PlayerInventory inventory;

    [SerializeField] PlayerGUI playerGUI;
    [SerializeField] GameObject inventoryUI;
    [Header("UI Elements")]
    [SerializeField] GameObject weaponsContainer;
    [SerializeField] GameObject itemsContainer;
    [SerializeField] GameObject abilitiesContainer;
    [SerializeField] GameObject itemSlotPrefab;
    [SerializeField] ItemToolTip itemToolTip;
    [SerializeField] InventoryTab[] tabs;
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite[] backgroundSprites;
    [Header("Selected Item Elements")]
    [SerializeField] Image selectedItemImage;
    [SerializeField] TMP_Text selectedItemName;
    [SerializeField] TMP_Text selectedItemDesc;
    [SerializeField] TMP_Text selectedItemEffects;
    [SerializeField] TMP_Text selectedAbilityCooldown;
    [SerializeField] GameObject equipButton;
    [SerializeField] GameObject destroyButton;
    [SerializeField] TMP_Text equipButtonText;
    [SerializeField] Sprite defaultSelectedSprite;

    ItemSlot weaponSlot;
    ItemSlot freeSlot;

    ItemSlot[] itemSlots;
    ItemSlot[] weaponSlots;
    List<ItemSlot> abilitySlots;
    DialogueManager dialogueManager;
    QuestSystem questSystem;
    public int tab { get; private set; }

    public int selectedItemIndex { get; private set; }
    public int selectedWeaponIndex { get; private set; }
    public int selectedAbilityIndex { get; private set; }

    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        questSystem = FindObjectOfType<QuestSystem>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        inventory = FindObjectOfType<PlayerInventory>();

        abilitySlots = new List<ItemSlot>();
        abilitySlots = abilitiesContainer.GetComponentsInChildren<ItemSlot>().ToList<ItemSlot>();
        itemSlots = itemsContainer.GetComponentsInChildren<ItemSlot>();
        weaponSlots = weaponsContainer.GetComponentsInChildren<ItemSlot>();

        itemsContainer.SetActive(false);
        abilitiesContainer.SetActive(false);
        weaponsContainer.SetActive(true);

        ItemSlot[] slots = inventoryUI.GetComponentsInChildren<ItemSlot>();
        foreach (ItemSlot slot in slots)
        {
            slot.Init();
            if (slot.SlotType == ItemSlot.ItemSlotType.WeaponEquipSlot)
                weaponSlot = slot;
            else if (freeSlot == null && slot.SlotType == ItemSlot.ItemSlotType.FreeEquipSlot)
                freeSlot = slot;
        }
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Init();
        }

        inventoryUI.SetActive(false);

        initialized = true;

        selectedItemIndex = -1;
        selectedWeaponIndex = -1;
        selectedAbilityIndex = -1;

        tab = 0;
        backgroundImage.sprite = backgroundSprites[tab];

        SetUnselected();
    }

    void UpdateGUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Item item = inventory.GetItem(i);
            itemSlots[i].UpdateImage(item, i == inventory.currentItemIndex);
        }
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            Weapon weapon = inventory.GetWeapon(i);
            weaponSlots[i].UpdateImage(weapon, i == inventory.currentWeaponIndex);
        }

        if(tab == 0)
        {
            if (selectedWeaponIndex >= 0 && inventory.GetWeapon(selectedWeaponIndex) != null)
            {
                weaponSlots[selectedWeaponIndex].SelectImage(true);
                equipButton.SetActive(true);
                destroyButton.SetActive(true);
                if (selectedWeaponIndex == inventory.currentWeaponIndex)
                    equipButtonText.text = "Unequip";
                else
                    equipButtonText.text = "Equip";
            } 
            else
            {
                equipButton.SetActive(false);
                destroyButton.SetActive(false);
            }
        }
        else if (tab == 1)
        {
            if (selectedItemIndex >= 0 && inventory.GetItem(selectedItemIndex) != null)
            {
                itemSlots[selectedItemIndex].SelectImage(true);
                equipButton.SetActive(true);
                destroyButton.SetActive(true);
                if (selectedItemIndex == inventory.currentItemIndex)
                    equipButtonText.text = "Unequip";
                else
                    equipButtonText.text = "Equip";
            } 
            else
            {
                equipButton.SetActive(false);
                destroyButton.SetActive(false);
            }
        }
        else if (tab == 2)
        {
            //if (selectedAbilityIndex >= 0)
            //    ab[selectedItemIndex].SelectImage(true);

            if (selectedAbilityIndex >= 0 && inventory.GetAbility(selectedAbilityIndex) != null)
            {
                abilitySlots[selectedAbilityIndex].SelectImage(true);
                equipButton.SetActive(true);
                destroyButton.SetActive(false);
                if (selectedAbilityIndex == inventory.currentAbilityIndex)
                    equipButtonText.text = "Unequip";
                else
                    equipButtonText.text = "Equip";
            }
            else
            {
                equipButton.SetActive(false);
                destroyButton.SetActive(false);
            }
        }

        //weaponSlot.UpdateImage(inventory.CurrentWeapon, inventory.currentWeaponIndex);

        //freeSlot.UpdateImage(inventory.CurrentItem, inventory.currentItemIndex);

        //if (inventory.CurrentAbility != null)
        //{
            //freeSlot.UpdateImage(inventory.CurrentAbility, inventory.currentAbilityIndex);
        //}

        for (int i = 0; i < inventory.GetAbilities().Count; i++)
        {
            if(i >= abilitySlots.Count)
            {
                GameObject go = Instantiate(itemSlotPrefab, abilitiesContainer.transform);
                ItemSlot newSlot = go.GetComponentInChildren<ItemSlot>();
                if (newSlot) 
                {
                    newSlot.SetSlotType(ItemSlot.ItemSlotType.AbilitySlot);
                    newSlot.Init();
                    abilitySlots.Add(newSlot); 
                }
            }

            abilitySlots[i].UpdateImage(inventory.GetAbility(i), i == inventory.currentAbilityIndex);
        }
    }

    public void SwitchTabs(int tabIn)
    {
        tab = tabIn;
        backgroundImage.sprite = backgroundSprites[tab];

        if (tab == 0)
        {
            weaponsContainer.SetActive(true);
            itemsContainer.SetActive(false);
            abilitiesContainer.SetActive(false);

            tabs[1].Unselect();
            tabs[2].Unselect();

            if (selectedWeaponIndex >= 0)
                SetSelectedWeapon(selectedWeaponIndex);
            else
                SetUnselected();
        }
        else if (tab == 1)
        {
            weaponsContainer.SetActive(false);
            itemsContainer.SetActive(true);
            abilitiesContainer.SetActive(false);

            tabs[0].Unselect();
            tabs[2].Unselect();

            if (selectedItemIndex >= 0)
                SetSelectedItem(selectedItemIndex);
            else
                SetUnselected();
        }
        else if (tab == 2)
        {
            weaponsContainer.SetActive(false);
            itemsContainer.SetActive(false);
            abilitiesContainer.SetActive(true);

            tabs[0].Unselect();
            tabs[1].Unselect();

            if (selectedAbilityIndex >= 0)
                SetSelectedAbility(selectedAbilityIndex);
            else
                SetUnselected();
        }
        else
        {
            UpdateGUI();
        }
    }


    public void Equip()
    {
        if(tab == 0)
        {
            if (selectedWeaponIndex == inventory.currentWeaponIndex)
                EquipWeapon(-1);
            else
                EquipWeapon(selectedWeaponIndex);
        }
        else if (tab == 1)
        {
            if (selectedItemIndex == inventory.currentItemIndex)
                EquipItem(-1);
            else
                EquipItem(selectedItemIndex);
        }
        else if(tab == 2)
        {
            if (selectedAbilityIndex == inventory.currentAbilityIndex)
                EquipAbility(-1);
            else
                EquipAbility(selectedAbilityIndex);
        }
    }
    public void EquipWeapon(int index) 
    {
        inventory.EquipWeapon(index);

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }
    public void EquipItem(int index)
    {
        inventory.EquipItem(index);

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }
    public void EquipAbility(int index)
    {
        inventory.EquipPlayerAbility(index);

        UpdateGUI();
        playerGUI.UpdateHotbarGUI();
    }

    public void UpdateItemToolTip(int index, bool equippedSlot, Vector2 position)
    {
        Item item = null;
        if (equippedSlot)
        {
            item = inventory.CurrentItem;
        } else
        {
            item = inventory.GetItem(index);
        }

        if (item != null)
        {
            itemToolTip.AssignItem(item);
            itemToolTip.transform.position = position + new Vector2(50f, 50f);
            itemToolTip.gameObject.SetActive(true);
        }
    }
    public void UpdateWeaponToolTip(int index, bool equippedSlot, Vector2 position)
    {
        Weapon weapon = null;
        if (equippedSlot)
        {
            weapon = inventory.CurrentWeapon;
        }
        else
        {
            weapon = inventory.GetWeapon(index);
        }

        if (weapon != null)
        {
            itemToolTip.AssignWeapon(weapon);
            itemToolTip.transform.position = position + new Vector2(50f, 50f);
            itemToolTip.gameObject.SetActive(true);
        }
    }
    public void UpdateAbilityToolTip(int index, bool equippedSlot, Vector2 position)
    {
        Ability ability = null;
        if (equippedSlot)
        {
            ability = inventory.CurrentAbility;
        }
        else
        {
            ability = inventory.GetAbility(index);
        }

        if (ability != null)
        {
            itemToolTip.AssignAbility(ability);
            itemToolTip.transform.position = position + new Vector2(50f, 50f);
            itemToolTip.gameObject.SetActive(true);
        }
    }
    public void DisableToolTip()
    {
        itemToolTip.gameObject.SetActive(false);
    }

    void OnToggleInventory()
    {
        if (dialogueManager.getCutsceneState() == DialogueManager.CutsceneState.None)
        {
            if (inventoryUI.activeSelf == true)
            {
                for (int i = 0; i < itemSlots.Length; i++)
                {
                    itemSlots[i].SelectImage(false);
                }
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    weaponSlots[i].SelectImage(false);
                }

                inventoryUI.SetActive(false);
                itemToolTip.gameObject.SetActive(false);
                Cursor.visible = false;

                GameTime.UnpauseTime();
            }
            else
            {
                inventoryUI.SetActive(true);
                Cursor.visible = true;

                if (!initialized) Start();

                UpdateGUI();

                questSystem.QuestEvent(QuestSystem.QuestEventType.InventoryOpened, null);
                GameTime.PauseTime(false);
            }
        }
    }

    public void SetSelectedItem(int selectedItem)
    {
        if (itemSlots == null || inventory == null) return;

        if (selectedItemIndex >= 0)
            itemSlots[selectedItemIndex].SelectImage(false);

        selectedItemIndex = selectedItem;
        //selectedWeaponIndex = -1;
        //selectedAbilityIndex = -1;

        Item item = inventory.GetItem(selectedItem);
        if (item != null)
        {
            selectedItemName.text = item.GetName();
            selectedItemDesc.text = item.GetDescription();
            selectedItemImage.sprite = item.GetInfo().GetDisplaySprite();
            selectedItemEffects.text = item.GetInfo().GetEffectsDescription();
            selectedItemImage.enabled = true;

            UpdateGUI();
        }
        else
        {
            SetUnselected();
        }
    }
    public void SetSelectedWeapon(int selectedWeapon)
    {
        if (weaponSlots == null || inventory == null) return;

        if (selectedWeaponIndex >= 0)
            weaponSlots[selectedWeaponIndex].SelectImage(false);

        //selectedItemIndex = -1;
        selectedWeaponIndex = selectedWeapon;
        //selectedAbilityIndex = -1;

        Weapon weapon = inventory.GetWeapon(selectedWeapon);
        if (weapon != null)
        {
            selectedItemName.text = weapon.GetName();
            selectedItemDesc.text = weapon.GetDescription();
            selectedItemEffects.text = weapon.GetInfo().GetEffectsDescription();
            selectedItemImage.sprite = weapon.GetInfo().GetDisplaySprite();
            selectedItemImage.enabled = true;

            UpdateGUI();
        } 
        else
        {
            SetUnselected();
        }
    }
    public void SetSelectedAbility(int selectedAbility)
    {
        if (abilitySlots == null || inventory == null) return;

        if (selectedAbilityIndex >= 0)
            abilitySlots[selectedAbilityIndex].SelectImage(false);

        //selectedItemIndex = -1;
        //selectedWeaponIndex = -1;
        selectedAbilityIndex = selectedAbility;
        
        Ability ability = inventory.GetAbility(selectedAbility);
        if (ability != null)
        {
            selectedItemName.text = ability.GetName();
            selectedItemDesc.text = ability.GetDescription();
            selectedAbilityCooldown.text = $"{ability.GetInfo().GetCooldown()}s";
            selectedItemImage.sprite = ability.GetInfo().GetDisplaySprite();
            selectedItemImage.enabled = true;

            UpdateGUI();
        }
        else
        {
            SetUnselected();
        }
    }
    public void SetUnselected()
    {
        selectedItemName.text = "Fist";
        selectedItemDesc.text = "Just your fist. Not very strong dealing only 1 damage per hit and a very short range.";
        selectedAbilityCooldown.text = "";
        selectedItemEffects.text = "";
        selectedItemImage.sprite = defaultSelectedSprite;
        selectedItemImage.enabled = true;

        equipButton.SetActive(false);

        UpdateGUI();
    }

    public void DestroyCurrentItem()
    {
        if (tab == 0)
        {
            if(selectedWeaponIndex >= 0)
            {
                if (selectedWeaponIndex == inventory.currentWeaponIndex)
                    EquipWeapon(-1);

                inventory.RemoveWeapon(selectedWeaponIndex);
                weaponSlots[selectedWeaponIndex].SelectImage(false);
                selectedWeaponIndex = -1;
                SetUnselected();
            }
        }
        else if (tab == 1)
        {
            if(selectedItemIndex >= 0)
            {
                if (selectedItemIndex == inventory.currentItemIndex)
                    EquipItem(-1);

                inventory.RemoveItem(selectedItemIndex);
                itemSlots[selectedItemIndex].SelectImage(false);
                selectedItemIndex = -1;
                SetUnselected();
            }
        }
    }
}
