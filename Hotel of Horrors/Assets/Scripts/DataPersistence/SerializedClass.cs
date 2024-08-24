using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SerializedClass : MonoBehaviour
{
    public int level;
    public int emotionalEnergy;

    public Item[] itemsInventory;
    public WeaponInfo[] weaponsInventory;
    public List<MementoInfo> mementosInventory;
    public List<AbilityInfo> abilitiesInventory;

    public WeaponInfo currentWeapon;
    public UseableInfo itemOne;
    public MementoInfo currentMemento;

    public List<Quest> quests;

    public SerializedClass()
    {
        level = 1; //initalizing to 1 every time for now because i dont know how we plan on tracking this in the future
        emotionalEnergy = FindObjectOfType<EmotionalEnergy>().GetEmotionalEnergy();

        Inventory playerInventory = FindObjectOfType<Inventory>();
        itemsInventory = playerInventory.GetItems();
        itemOne = playerInventory.itemOne;

        weaponsInventory = playerInventory.GetWeapons();
        currentWeapon = playerInventory.currentWeapon;

        mementosInventory = playerInventory.GetMementos();
        currentMemento = playerInventory.currentMemento;

        abilitiesInventory = playerInventory.GetAbilities();

        quests = FindObjectOfType<QuestManager>().GetQuests();
    }

    public void OverWriteData()
    {
        //set all the corresponding variables in their home scripts
    }
}
