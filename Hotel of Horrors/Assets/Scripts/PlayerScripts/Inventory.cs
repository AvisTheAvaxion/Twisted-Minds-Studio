using Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    WeaponInfo[] weaponsInventory;
    Item[] itemsInventory;
    List<MementoInfo> mementosInventory;
    List<AbilityInfo> abilitiesInventory;

    [Header("Items")]
    [SerializeField] public WeaponInfo currentWeapon;
    [SerializeField] public UseableInfo itemOne;
    [SerializeField] public MementoInfo currentMemento;

    [Header("References")]
    [SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] PlayerHealth playerHealth;
    ItemSlot weaponSlot;
    ItemSlot freeSlot;
    ItemSlot mementoSlot;

    ItemSlot[] itemSlots;
    ItemSlot[] weaponSlots;
    List<AbilitySlot> abilitySlots;

    AttackController attackController;

    private void Awake()
    {
        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        uiDisplay.InventoryUI.SetActive(true);
        uiDisplay.InventoryUI.SetActive(false);
        attackController = GetComponent<AttackController>();

        abilitySlots = new List<AbilitySlot>();
        abilitySlots = uiDisplay.AbilitiesContainer.GetComponentsInChildren<AbilitySlot>().ToList<AbilitySlot>();
        itemSlots = uiDisplay.ItemsContainer.GetComponentsInChildren<ItemSlot>();
        weaponSlots = uiDisplay.WeaponsContainer.GetComponentsInChildren<ItemSlot>();

        itemsInventory = new Item[itemSlots.Length];
        weaponsInventory = new WeaponInfo[weaponSlots.Length];

        abilitiesInventory = new List<AbilityInfo>();
        mementosInventory = new List<MementoInfo>();

        ItemSlot[] slots = uiDisplay.InventoryUI.GetComponentsInChildren<ItemSlot>();
        foreach (ItemSlot slot in slots)
        {
            slot.Init();
            if (slot.IsWeaponEquipSlot)
                weaponSlot = slot;
            else if (freeSlot == null && slot.IsFreeEquipSlot)
                freeSlot = slot;
        }

        for (int i = 0; i < abilitySlots.Count; i++)
        {
            AddPlayerAbility(abilitySlots[i].abilityInfo);
        }

        uiDisplay.ItemsContainer.SetActive(false);
        uiDisplay.AbilitiesContainer.SetActive(false);
        uiDisplay.WeaponsContainer.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemData data = collision.GetComponent<ItemData>();

        if (data)
        {
            bool remove = true;
            UseableInfo useable = data.GetItemData();
            int count = data.GetCount();
            if (useable.GetType() == typeof(ItemInfo))
            {
                Item newItem = new Item((ItemInfo)useable, count);
                remove = AddItem(newItem);
            }
            else if (useable.GetType() == typeof(MementoInfo))
            {
                remove = AddMemento((MementoInfo)useable);
            }
            else if (useable.GetType() == typeof(AbilityInfo))
            {

            }
            else if (useable.GetType() == typeof(WeaponInfo))
            {
                remove = AddWeapon((WeaponInfo)useable);
            }
            if(remove) Destroy(collision.gameObject);
        }
    }

    public WeaponInfo GetWeapon(int index)
    {
        return index >= 0 && index < weaponsInventory.Length ? weaponsInventory[index] : null;
    }
    public Item GetItem(int index)
    {
        return index >= 0 && index < itemsInventory.Length ? itemsInventory[index] : null;
    }

    public void EquipWeapon(int index)
    {
        WeaponInfo weapon = GetWeapon(index);
        attackController.EquipWeapon(weapon);
        weaponSlot.UpdateImage(index);

        if (weapon == null)
        {
            uiDisplay.WeaponHotbarImage.enabled = false;
            uiDisplay.WeaponAbilityHotbarImage.enabled = false;
        }
        else
        {
            uiDisplay.WeaponHotbarImage.enabled = true;
            uiDisplay.WeaponHotbarImage.sprite = weapon.GetSprite();

            if(weapon.GetWeaponAbility() != null)
            {
                uiDisplay.WeaponAbilityHotbarImage.enabled = true;
                uiDisplay.WeaponAbilityHotbarImage.sprite = weapon.GetWeaponAbilitySprite();
            }
        }
    }
    public void EquipItem(int index)
    {
        Item item = GetItem(index);
        freeSlot.UpdateImage(index);

        if (item == null)
        {
            uiDisplay.FreeSlotHotbarImage.enabled = false;
        }
        else
        {
            attackController.UnequipPlayerAbility();
            uiDisplay.FreeSlotHotbarImage.enabled = true;
            uiDisplay.FreeSlotHotbarImage.sprite = item.GetInfo().GetSprite();
        }
    }
    public void EquipPlayerAbility(int index)
    {
        AbilityInfo ability = index >= 0 && index < abilitiesInventory.Count ? abilitiesInventory[index] : null;

        if (ability == null)
        {
            freeSlot.UpdateImage(null);
            attackController.UnequipPlayerAbility();
            uiDisplay.FreeSlotHotbarImage.enabled = false;
        }
        else
        {
            EquipItem(-1);
            attackController.EquipPlayerAbility(ability);
            uiDisplay.FreeSlotHotbarImage.enabled = true;
            uiDisplay.FreeSlotHotbarImage.sprite = ability.GetSprite();
            freeSlot.UpdateImage(ability.GetSprite());
        }
    }
    public void EquipPlayerAbility(AbilityInfo ability)
    {
        //Abilities ability = index < abilitiesInventory.Count ? abilitiesInventory[index] : null;
        //ItemInstance item = itemsInventory[index];
        //freeSlot.UpdateImage(index);

        if (ability == null)
        {
            freeSlot.UpdateImage(null);
            attackController.UnequipPlayerAbility();
            uiDisplay.FreeSlotHotbarImage.enabled = false;
        }
        else
        {
            EquipItem(-1);
            attackController.EquipPlayerAbility(ability);
            uiDisplay.FreeSlotHotbarImage.enabled = true;
            uiDisplay.FreeSlotHotbarImage.sprite = ability.GetSprite();
            freeSlot.UpdateImage(ability.GetSprite());
        }
    }
    public void EquipMemento(int index)
    {
        MementoInfo mementos = index < mementosInventory.Count ? mementosInventory[index] : null;
        //ItemInstance item = itemsInventory[index];
        //freeSlot.UpdateImage(index);

        if (mementos == null)
        {
            uiDisplay.MementoHotbarImage.enabled = false;
        }
        else
        {
            attackController.EquipSpecialAbility(mementos);
            uiDisplay.MementoHotbarImage.enabled = true;
            uiDisplay.MementoHotbarImage.sprite = mementos.GetSprite();
        }
    }

    public bool AddItem(Item itemInstance)
    {
        for (int i = 0; i < itemsInventory.Length; i++)
        {
            if (itemsInventory[i] == null)
            {
                itemsInventory[i] = itemInstance;
                itemSlots[i].UpdateImage();
                return true;
            } 
            else if (itemsInventory[i].GetInfo().GetName() == itemInstance.GetInfo().GetName())
            {
                int newAmount = itemsInventory[i].AddAmount(itemInstance.CurrentAmount);
                if (freeSlot.itemIndex == i) freeSlot.UpdateImage(i);
                itemSlots[i].UpdateImage();
                if (newAmount > 0)
                {
                    itemInstance.SetCurrentAmount(newAmount);
                    continue;
                }
                return true;
            }
        }
        return false;
    }

    public bool AddWeapon(WeaponInfo weapon)
    {
        for (int i = 0; i < weaponsInventory.Length; i++)
        {
            if (weaponsInventory[i] == null)
            {
                weaponsInventory[i] = weapon;
                weaponSlots[i].UpdateImage();
                return true;
            }
        }
        return false;
    }
    public bool AddMemento(MementoInfo memento)
    {
        if(!mementosInventory.Contains(memento))
        {
            mementosInventory.Add(memento);
            return true;
        } else
        {
            return false;
        }
    }
    public bool AddPlayerAbility(AbilityInfo ability)
    {
        if (!abilitiesInventory.Contains(ability))
        {
            abilitiesInventory.Add(ability);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnToggleInventory()
    {
        if(uiDisplay.InventoryUI.activeSelf == true)
        {
            uiDisplay.InventoryUI.SetActive(false);
            Cursor.visible = false;
        }
        else
        {
            uiDisplay.InventoryUI.SetActive(true);
            Cursor.visible = true;
        }
    }
    void OnSecondaryAction(InputValue inputValue)
    {
        UseItem(freeSlot.itemIndex, freeSlot);
    }

    public void UseItem(int index, ItemSlot slot)
    {
        Item item = GetItem(index);
        if (item != null)
        {
            item.RemoveAmount(1);
            foreach (EffectInfo effectInfo in item.GetInfo().GetEffectInfos())
            {
                Effect effect = new Effect(effectInfo, 1);
                playerHealth.InflictEffect(effect);
            }

            if(item.CurrentAmount <= 0)
            {
                itemsInventory[index] = null;
            }

            itemSlots[index].UpdateImage();
            if (freeSlot.itemIndex == index)
            {
                if (item.CurrentAmount > 0)
                    freeSlot.UpdateImage(index);
                else
                {
                    uiDisplay.FreeSlotHotbarImage.enabled = false;
                    freeSlot.UpdateImage(-1);
                }
            }
        }
    }

    public void UpdatePlayerMode()
    {
        if (currentWeapon == null)
            attackController.ChangeAttackMode(AttackModes.None);
        else
            attackController.ChangeAttackMode(currentWeapon.GetWeaponMode());
    }
    
}

public class Useable
{
    protected UseableInfo info;
    protected int currentAmount;

    protected bool isFull;

    public int CurrentAmount { get => currentAmount; }
    public bool IsFull { get => isFull; }

    public virtual UseableInfo GetInfo()
    {
        return info;
    }

    public Useable(UseableInfo item, int amount)
    {
        this.info = item;
        currentAmount = amount;
    }

    public int AddAmount(int amount)
    {
        currentAmount += amount;
        isFull = currentAmount >= info.GetMaxStackAmount();
        if(isFull)
        {
            int amountOver = currentAmount - info.GetMaxStackAmount();
            currentAmount = info.GetMaxStackAmount();
            return amountOver;
        } else
            return 0;
    }
    public int RemoveAmount(int amount)
    {
        currentAmount -= amount;
        isFull = false;
        currentAmount = currentAmount < 0 ? 0 : currentAmount;
        return currentAmount;
    }

    public void SetCurrentAmount(int amount)
    {
        currentAmount = amount;
    }
}

public class Item : Useable
{
    public Item(ItemInfo item, int amount) : base(item, amount)
    {
    }

    public ItemInfo GetInfo()
    {
        return (ItemInfo)info;
    }
}
