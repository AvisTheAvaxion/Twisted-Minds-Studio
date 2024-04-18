using Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [Header("Items")]
    List<Useables> inventoryItems = new List<Useables>();
    List<ItemInstance> itemsInventory;
    List<MementoInstance> mementosInventory;
    List<AbilityInstance> abilitiesInventory;

    [SerializeField] public Weapon currentWeapon;
    [SerializeField] public Useables itemOne;
    [SerializeField] public Mementos currentMemento;

    [Header("References")]
    [SerializeField] UIDisplayContainer uiDisplay;
    [SerializeField] PlayerHealth playerHealth;
    ItemSlot weaponSlot;
    ItemSlot freeSlot;
    ItemSlot mementoSlot;
    ItemSlot referencedWeaponSlot;
    ItemSlot referencedFreeSlot;
    List<ItemSlot> slots;

    AttackController attackController;

    private void Awake()
    {
        if (uiDisplay == null) uiDisplay = FindObjectOfType<UIDisplayContainer>();
        if (uiDisplay == null) Debug.LogError("UI display container script not assigned and not found in scene (located on canvas UI prefab");

        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        uiDisplay.InventoryUI.SetActive(true);
        uiDisplay.InventoryUI.SetActive(false);
        attackController = GetComponent<AttackController>();

        slots = uiDisplay.InventoryUI.GetComponentsInChildren<ItemSlot>().ToList();
        foreach (ItemSlot slot in slots)
        {
            if (slot.IsWeaponSlot)
                weaponSlot = slot;
            else if (freeSlot == null && slot.IsFreeSlot)
                freeSlot = slot;
            else if (mementoSlot == null && slot.IsMementoSlot)
                mementoSlot = slot;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemData data = collision.GetComponent<ItemData>();

        if (data)
        {
            //Debug.Log("PickUp Get");
            Useables useable = data.GetItemData();
            int count = data.GetCount();
            if (useable.GetType() == typeof(Item))
            {
                //ItemInstance newItem = new ItemInstance((Item)useable, count);
            }
            else if (useable.GetType() == typeof(Mementos))
            {

            }
            else if (useable.GetType() == typeof(Abilities))
            {

            }
            //UseableInstance newItem = new UseableInstance(useable, count);
            //Debug.Log(useable.GetDescription());
            collision.gameObject.SetActive(false);
            inventoryItems.Add(useable);
            SlotInItem(useable);
        }
    }

    public void EquipWeapon(Weapon weapon, ItemSlot slotRef)
    {
        attackController.Equip(weapon);
        weaponSlot.itemHeld = currentWeapon = weapon;
        weaponSlot.UpdateImage();

        referencedWeaponSlot = slotRef;

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
    public void EquipFreeItem(Useables item, ItemSlot slotRef)
    {
        freeSlot.itemHeld = itemOne = item;
        freeSlot.UpdateImage();

        referencedFreeSlot = slotRef;

        if (item == null)
        {
            uiDisplay.FreeSlotHotbarImage.enabled = false;
        }
        else
        {
            uiDisplay.FreeSlotHotbarImage.enabled = true;
            uiDisplay.FreeSlotHotbarImage.sprite = item.GetSprite();
        }
    }

    public void SlotInItem(Useables item)
    {
        int skipThree = 0;
        foreach(ItemSlot slot in slots)
        {
            if(slot.itemHeld != null || skipThree < 3)
            {
                skipThree++;
                continue;
            }
            else
            {
                slot.itemHeld = item;
                break;
            }
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
        if(itemOne != null && itemOne.GetType() == typeof(Item))
        {
            UseItem((Item)itemOne, referencedFreeSlot);
        }
    }

    public void UseItem(Item itemToUse, ItemSlot slot)
    {
        Item item = itemToUse;
        foreach (EffectInfo effectInfo in item.GetEffectInfos())
        {
            Effect effect = new Effect(effectInfo, 1);
            playerHealth.InflictEffect(effect);
        }
        slot.UnequipItem();
        EquipFreeItem(null, null);
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

    public void AddAmount(int amount)
    {
        currentAmount += amount;
        currentAmount = (isFull = currentAmount > info.GetMaxStackAmount()) ? info.GetMaxStackAmount() : currentAmount;
    }
    public int RemoveAmount(int amount)
    {
        currentAmount -= amount;
        isFull = false;
        currentAmount = currentAmount < 0 ? 0 : currentAmount;
        return currentAmount;
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
