using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SerializedClass
{
    public int level;
    public int maxLevelAchieved;
    public int emotionalEnergy;

    public Item[] itemsInventory;
    public Weapon[] weaponsInventory;
    public List<MementoInfo> mementosInventory;
    public List<Ability> abilitiesInventory;

    public int currentWeaponIndex;
    public int currentItemIndex;
    public int currentAbilityIndex;
    public int currentMementoIndex;

    //public List<Quest> quests;

    public SerializedClass(PlayerInventory inventory)
    {
        level = SceneManager.GetActiveScene().buildIndex; 
        maxLevelAchieved = -1; //initalizing to -1 until evelator is implemented

        PlayerInventory playerInventory = inventory;
        emotionalEnergy = playerInventory.emotionalEnergy;

        itemsInventory = playerInventory.GetItems();
        currentItemIndex = playerInventory.currentItemIndex;

        weaponsInventory = playerInventory.GetWeapons();
        currentWeaponIndex = playerInventory.currentWeaponIndex;

        mementosInventory = playerInventory.GetMementos();
        currentMementoIndex = playerInventory.currentMementoIndex;

        abilitiesInventory = playerInventory.GetAbilities();
        currentAbilityIndex = playerInventory.currentAbilityIndex;

        //quests = questManager.GetQuests();
    }

    public void OverWriteData(PlayerInventory inventory)
    {
        //set all the corresponding variables in their home scripts
        inventory.LoadInventory(this);

    }
}
