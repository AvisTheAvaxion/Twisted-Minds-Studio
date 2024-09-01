using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SerializedClass
{
    public ItemSave[] joes;
    public AbilitySave[] bills;
    public WeaponSave[] bobs;

    public int level;
    public int maxLevelAchieved;
    public int emotionalEnergy;

    //public Item[] itemsInventory;
    //public Weapon[] weaponsInventory;
    public List<MementoInfo> mementosInventory;
    //public List<Ability> abilitiesInventory;

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

        joes = new ItemSave[playerInventory.GetItems().Length];

        for(int i = 0; i < joes.Length; i++)
        {
            joes[i] = new ItemSave();
            joes[i].id = -1;

            if (playerInventory.GetItems()[i] == null)
                continue;

            
            joes[i].id = playerInventory.GetItems()[i].id;
            joes[i].currentAmount = playerInventory.GetItems()[i].currentAmount;
            joes[i].isFull = playerInventory.GetItems()[i].isFull;
        }

        bills = new AbilitySave[playerInventory.GetAbilities().Count];

        for (int i = 0; i < bills.Length; i++)
        {
            bills[i] = new AbilitySave();
            bills[i].id = -1;

            if (playerInventory.GetAbilities()[i] == null)
                continue;

            bills[i].id = playerInventory.GetAbilities()[i].id;

            bills[i].currentLevel = playerInventory.GetAbilities()[i].currentLevel;

            bills[i].cooldown = playerInventory.GetAbilities()[i].cooldown;
            bills[i].duration = playerInventory.GetAbilities()[i].duration;
            bills[i].damage = playerInventory.GetAbilities()[i].damage;
            bills[i].size = playerInventory.GetAbilities()[i].size;
            bills[i].range = playerInventory.GetAbilities()[i].range;

            bills[i].numberOfProjectiles = playerInventory.GetAbilities()[i].numberOfProjectiles;
            bills[i].deflectionResistance = playerInventory.GetAbilities()[i].deflectionResistance;
            bills[i].maxTargets = playerInventory.GetAbilities()[i].maxTargets;
            bills[i].goThroughWalls = playerInventory.GetAbilities()[i].goThroughWalls;

        }

        bobs = new WeaponSave[playerInventory.GetWeapons().Length];

        for (int i = 0; i < bobs.Length; i++)
        {
            bobs[i] = new WeaponSave();
            bobs[i].id = -1;

            if (playerInventory.GetWeapons()[i] == null)
                continue;

            bobs[i].id = playerInventory.GetWeapons()[i].id;
            bobs[i].currentLevel = playerInventory.GetWeapons()[i].currentLevel;
            bobs[i].autoAttack = playerInventory.GetWeapons()[i].autoAttack;
            bobs[i].attackSpeed = playerInventory.GetWeapons()[i].attackSpeed;
            bobs[i].damage = playerInventory.GetWeapons()[i].damage;
            bobs[i].deflectionStrength = playerInventory.GetWeapons()[i].deflectionStrength;
            bobs[i].knockback = playerInventory.GetWeapons()[i].knockback;
        }


        currentItemIndex = playerInventory.currentItemIndex;

        //weaponsInventory = playerInventory.GetWeapons();
        currentWeaponIndex = playerInventory.currentWeaponIndex;

        mementosInventory = playerInventory.GetMementos();
        currentMementoIndex = playerInventory.currentMementoIndex;

        //abilitiesInventory = playerInventory.GetAbilities();
        currentAbilityIndex = playerInventory.currentAbilityIndex;

        //quests = questManager.GetQuests();
    }

    public void OverWriteData(PlayerInventory inventory)
    {
        //set all the corresponding variables in their home scripts
        inventory.LoadInventory(this);

    }
}

[System.Serializable]
public class ItemSave{
    public int id;
    public int currentAmount;
    public bool isFull;
}

[System.Serializable]
public class AbilitySave
{
    public int id;

    public int currentLevel;

    public float cooldown;
    public float duration;
    public float damage;
    public float size;
    public float range;

    public int numberOfProjectiles;
    public float deflectionResistance;
    public int maxTargets;
    public bool goThroughWalls;

}

[System.Serializable]
public class WeaponSave
{
    public int id;
    public int currentLevel;
    public bool autoAttack;
    public float attackSpeed;
    public int damage;
    public float deflectionStrength;
    public float knockback;
}
