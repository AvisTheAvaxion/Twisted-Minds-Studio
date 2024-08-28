using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SerializationManager : MonoBehaviour
{
    string saveFile;
    PlayerInventory inventory;
    QuestManager quests;

    private void Start()
    {
        saveFile = Application.persistentDataPath + "/savedata.json";
        inventory = FindObjectOfType<PlayerInventory>();
        //quests = FindObjectOfType<QuestManager>();
    }

    public void SaveData()
    {
        //clear the file before writing to it
        File.Delete(saveFile);

        if(inventory == null)
        {
            print("Inventory not found...exiting");
            return;
        }

        //if (quests == null)
        //{
        //    print("Quests not found...exiting");
        //    return;
        //}

        SerializedClass classToSave = new SerializedClass(inventory);

        string json = JsonUtility.ToJson(classToSave);
        print(json);
        File.WriteAllText(saveFile, json);

        //List<string> jsonLines = new List<string>();
        //jsonLines.Add(JsonUtility.ToJson(classToSave.level));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.emotionalEnergy));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.itemsInventory));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.weaponsInventory));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.mementosInventory));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.abilitiesInventory));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.currentWeaponIndex));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.currentItemIndex));
        //jsonLines.Add(JsonUtility.ToJson(classToSave.currentMementoIndex));

        //write JSON data to file
        //File.WriteAllLines(saveFile, jsonLines);

        string[] linesSaved = File.ReadAllLines(saveFile);
        print("Saved " + linesSaved.Length +"Lines to " + saveFile);
    }

    public void LoadData()
    {
        //check if file path exists
        if (!File.Exists(saveFile))
        {
            print("File not found");
            return;
        }

        string json = File.ReadAllText(saveFile);
        print(json);

        SerializedClass classToLoad = new SerializedClass(inventory);

        //read the data from file and set jsondata

        JsonUtility.FromJsonOverwrite(json, classToLoad);

        //classToLoad.level = JsonUtility.FromJson<int>(jsonLines[0]);
        //classToLoad.emotionalEnergy = JsonUtility.FromJson<int>(jsonLines[1]);
        //classToLoad.itemsInventory = JsonUtility.FromJson<Item[]>(jsonLines[2]);
        //classToLoad.weaponsInventory = JsonUtility.FromJson<Weapon[]>(jsonLines[3]);
        //classToLoad.mementosInventory = JsonUtility.FromJson<List<MementoInfo>>(jsonLines[4]);
        //classToLoad.abilitiesInventory = JsonUtility.FromJson<List<Ability>>(jsonLines[5]);
        //classToLoad.currentWeaponIndex = JsonUtility.FromJson<int>(jsonLines[6]);
        //classToLoad.currentItemIndex = JsonUtility.FromJson<int>(jsonLines[7]);
        //classToLoad.currentMementoIndex = JsonUtility.FromJson<int>(jsonLines[8]);

        classToLoad.OverWriteData(inventory);

        print("Loaded");
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.S))
        {
            print("Saving...");
            SaveData();
        }
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.L))
        {
            print("Loading...");
            LoadData();
        }
    }
}
