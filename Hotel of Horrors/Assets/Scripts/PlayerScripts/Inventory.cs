using Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    Weapon[] weaponsInventory;
    ItemInstance[] itemsInventory;
    List<MementoInstance> mementosInventory;
    List<AbilityInstance> abilitiesInventory;

    [Header("Items")]
    [SerializeField] public Weapon currentWeapon;
    [SerializeField] public Useables itemOne;
    [SerializeField] public Mementos currentMemento;

    [Header("References")]
    [SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] PlayerHealth playerHealth;
    ItemSlot weaponSlot;
    ItemSlot freeSlot;
    ItemSlot mementoSlot;

    ItemSlot[] itemSlots;
    ItemSlot[] weaponSlots;

    AttackController attackController;

    private void Awake()
    {
        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        uiDisplay.InventoryUI.SetActive(true);
        uiDisplay.InventoryUI.SetActive(false);
        attackController = GetComponent<AttackController>();

        itemSlots = uiDisplay.ItemsContainer.GetComponentsInChildren<ItemSlot>();
        weaponSlots = uiDisplay.WeaponsContainer.GetComponentsInChildren<ItemSlot>();

        itemsInventory = new ItemInstance[itemSlots.Length];
        weaponsInventory = new Weapon[weaponSlots.Length];

        ItemSlot[] slots = uiDisplay.InventoryUI.GetComponentsInChildren<ItemSlot>();
        foreach (ItemSlot slot in slots)
        {
            slot.Init();
            if (slot.IsWeaponEquipSlot)
                weaponSlot = slot;
            else if (freeSlot == null && slot.IsFreeEquipSlot)
                freeSlot = slot;
        }

        uiDisplay.ItemsContainer.SetActive(false);
        uiDisplay.WeaponsContainer.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemData data = collision.GetComponent<ItemData>();

        if (data)
        {
            bool remove = true;
            Useables useable = data.GetItemData();
            int count = data.GetCount();
            if (useable.GetType() == typeof(Item))
            {
                ItemInstance newItem = new ItemInstance((Item)useable, count);
                remove = AddItem(newItem);
            }
            else if (useable.GetType() == typeof(Mementos))
            {

            }
            else if (useable.GetType() == typeof(Abilities))
            {

            }
            else if (useable.GetType() == typeof(Weapon))
            {
                remove = AddWeapon((Weapon)useable);
            }
            if(remove) Destroy(collision.gameObject);
        }
    }

    public Weapon GetWeapon(int index)
    {
        return index >= 0 && index < weaponsInventory.Length ? weaponsInventory[index] : null;
    }
    public ItemInstance GetItem(int index)
    {
        return index >= 0 && index < itemsInventory.Length ? itemsInventory[index] : null;
    }

    public void EquipWeapon(int index)
    {
        Weapon weapon = weaponsInventory[index];
        attackController.Equip(weapon);
        weaponSlot.UpdateImage(index);

        if (weapon == null)
        {
            uiDisplay.WeaponHotbarImage.enabled = false;
        }
        else
        {
            uiDisplay.WeaponHotbarImage.enabled = true;
            uiDisplay.WeaponHotbarImage.sprite = weapon.GetSprite();
        }
    }
    public void EquipFreeItem(int index)
    {
        ItemInstance item = itemsInventory[index];
        freeSlot.UpdateImage(index);

        if (item == null)
        {
            uiDisplay.FreeSlotHotbarImage.enabled = false;
        }
        else
        {
            uiDisplay.FreeSlotHotbarImage.enabled = true;
            uiDisplay.FreeSlotHotbarImage.sprite = item.GetInfo().GetSprite();
        }
    }

    public bool AddItem(ItemInstance itemInstance)
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

    public bool AddWeapon(Weapon weapon)
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
        ItemInstance item = GetItem(index);
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
            if (slot.IsFreeEquipSlot)
            {
                if(item.CurrentAmount > 0)
                    slot.UpdateImage(index);
                else
                    slot.UpdateImage(-1);
            } else if (freeSlot.itemIndex == index)
            {
                freeSlot.UpdateImage(index);
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

public class UseableInstance
{
    protected Useables info;
    protected int currentAmount;

    protected bool isFull;

    public int CurrentAmount { get => currentAmount; }
    public bool IsFull { get => isFull; }

    public virtual Useables GetInfo()
    {
        return info;
    }

    public UseableInstance(Useables item, int amount)
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

public class ItemInstance : UseableInstance
{
    public ItemInstance(Item item, int amount) : base(item, amount)
    {
    }

    public Item GetInfo()
    {
        return (Item)info;
    }
}

public class MementoInstance : UseableInstance
{
    public MementoInstance(Mementos item, int amount) : base(item, amount)
    {
    }

    public Mementos GetInfo()
    {
        return (Mementos)info;
    }
}

public class AbilityInstance : UseableInstance
{
    public AbilityInstance(Abilities item, int amount) : base(item, amount)
    {
    }

    public Abilities GetInfo()
    {
        return (Abilities)info;
    }
}
