using Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    Weapon[] weaponsInventory;
    Item[] itemsInventory;
    List<MementoInfo> mementosInventory;
    List<Ability> abilitiesInventory;

    public int currentWeaponIndex { get; private set; }
    public int currentItemIndex { get; private set; }
    public int currentAbilityIndex { get; private set; }
    public int currentMementoIndex { get; private set; }
    public Weapon CurrentWeapon { get => GetWeapon(currentWeaponIndex); }
    public Item CurrentItem { get => GetItem(currentItemIndex); }
    public Ability CurrentAbility { get => GetAbility(currentAbilityIndex); }
    public MementoInfo CurrentMemento { get => GetMemento(currentMementoIndex); }

    [Header("References")]
    [SerializeField] int itemInventorySize = 12;
    [SerializeField] int weaponInventorySize = 12;
    [SerializeField] PlayerHealth playerHealth;

    ActionController actionController;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        actionController = GetComponent<ActionController>();

        currentWeaponIndex = -1;
        currentItemIndex = -1;
        currentAbilityIndex = -1;
        currentMementoIndex = -1;

        itemsInventory = new Item[itemInventorySize];
        weaponsInventory = new Weapon[weaponInventorySize];

        abilitiesInventory = new List<Ability>();
        mementosInventory = new List<MementoInfo>();
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
            else if (useable.GetType() == typeof(WeaponInfo))
            {
                remove = AddWeapon(new Weapon((WeaponInfo)useable));
            }
            if(remove) Destroy(collision.gameObject);
        }
    }

    public Weapon GetWeapon(int index)
    {
        return index >= 0 && (index < weaponsInventory.Length && index >= 0) ? weaponsInventory[index] : null;
    }
    public Item GetItem(int index)
    {
        return index >= 0 && (index < itemsInventory.Length && index >= 0) ? itemsInventory[index] : null;
    }
    public Ability GetAbility(int index)
    {
        return index >= 0 && (index < abilitiesInventory.Count && index >= 0) ? abilitiesInventory[index] : null;
    }
    public MementoInfo GetMemento(int index)
    {
        return index >= 0 && (index < mementosInventory.Count && index >= 0) ? mementosInventory[index] : null;
    }

    #region Equipping From Inventory
    public void EquipWeapon(int index)
    {
        Weapon weapon = GetWeapon(index);
        actionController.EquipWeapon(weapon);

        if (weapon != null || index < 0)
        {
            currentWeaponIndex = index;
        }

    }
    public void EquipItem(int index)
    {
        Item item = GetItem(index);

        if (item != null || index < 0)
        {
            currentItemIndex = index;
            currentAbilityIndex = -1;

            actionController.UnequipPlayerAbility();
        }
    }
    public void EquipPlayerAbility(int index)
    {
        Ability ability = index >= 0 && index < abilitiesInventory.Count ? abilitiesInventory[index] : null;

        if (ability != null || index < 0)
        {
            currentAbilityIndex = index;
            currentItemIndex = -1;
            actionController.EquipPlayerAbility(ability);
        }
        else
        {
            actionController.UnequipPlayerAbility();
        }
    }

    public void EquipMemento(int index)
    {
        MementoInfo mementos = index < mementosInventory.Count ? mementosInventory[index] : null;

        if (mementos != null)
        {
            currentMementoIndex = index;
            actionController.EquipSpecialAbility(mementos);
        }
    }
    #endregion

    #region Adding to Inventory
    public bool AddItem(Item item)
    {
        for (int i = 0; i < itemsInventory.Length; i++)
        {
            if (itemsInventory[i] == null)
            {
                itemsInventory[i] = item;
                return true;
            } 
            else if (itemsInventory[i].GetInfo().GetName() == item.GetInfo().GetName())
            {
                int newAmount = itemsInventory[i].AddAmount(item.CurrentAmount);
                if (newAmount > 0)
                {
                    item.SetCurrentAmount(newAmount);
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
    public bool AddPlayerAbility(Ability ability)
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
    #endregion


    public void UseItem(int index)
    {
        Item item = GetItem(index);
        if (item != null)
        {
            item.RemoveAmount(1);

            if(item.CurrentAmount <= 0)
            {
                itemsInventory[index] = null;

                if (index == currentItemIndex)
                    currentItemIndex = -1;
            }
        }
    }

    public Weapon[] GetWeapons()
    {
        return weaponsInventory;
    }

    public Item[] GetItems()
    {
        return itemsInventory;
    }

    public List<MementoInfo> GetMementos()
    {
        return mementosInventory;
    }

    public List<Ability> GetAbilities()
    {
        return abilitiesInventory;
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
