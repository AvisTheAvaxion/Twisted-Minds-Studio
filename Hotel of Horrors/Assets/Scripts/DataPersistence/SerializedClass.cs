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
    public Weapon[] weaponsInventory;
    public List<MementoInfo> mementosInventory;
    public List<Ability> abilitiesInventory;

    public int currentWeaponIndex;
    public int currentItemIndex;
    public int currentAbilityIndex;
    public MementoInfo currentMemento;

    public List<Quest> quests;

    public SerializedClass()
    {
        level = 1; //initalizing to 1 every time for now because i dont know how we plan on tracking this in the future
        emotionalEnergy = FindObjectOfType<EmotionalEnergy>().GetEmotionalEnergy();

        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        itemsInventory = playerInventory.GetItems();
        currentItemIndex = playerInventory.currentItemIndex;

        weaponsInventory = playerInventory.GetWeapons();
        currentWeaponIndex = playerInventory.currentWeaponIndex;

        mementosInventory = playerInventory.GetMementos();
        currentMemento = playerInventory.CurrentMemento;

        abilitiesInventory = playerInventory.GetAbilities();
        currentAbilityIndex = playerInventory.currentAbilityIndex;

        quests = FindObjectOfType<QuestManager>().GetQuests();
    }

    public void OverWriteData()
    {
        //set all the corresponding variables in their home scripts
    }
}
