using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SerializationManager : MonoBehaviour
{
    string saveFile = Application.persistentDataPath + "/achievements.json";

    public void SaveData()
    {
        //check if file path exists

        //check if there is data in the file

        SerializedClass classToSave = new SerializedClass();

        List<string> jsonLines = new List<string>();
        jsonLines.Add(JsonUtility.ToJson(classToSave.level));
        jsonLines.Add(JsonUtility.ToJson(classToSave.emotionalEnergy));
        jsonLines.Add(JsonUtility.ToJson(classToSave.itemsInventory));
        jsonLines.Add(JsonUtility.ToJson(classToSave.weaponsInventory));
        jsonLines.Add(JsonUtility.ToJson(classToSave.mementosInventory));
        jsonLines.Add(JsonUtility.ToJson(classToSave.abilitiesInventory));
        jsonLines.Add(JsonUtility.ToJson(classToSave.currentWeapon));
        jsonLines.Add(JsonUtility.ToJson(classToSave.itemOne));
        jsonLines.Add(JsonUtility.ToJson(classToSave.currentMemento));

        //write JSON data to file
        File.WriteAllLines(saveFile, jsonLines);
    }

    public void loadData()
    {
        string[] jsonLines = File.ReadAllLines(saveFile);

        SerializedClass classToLoad = new SerializedClass();

        //read the data from file and set jsondata
        classToLoad.level = JsonUtility.FromJson<int>(jsonLines[0]);
        classToLoad.emotionalEnergy = JsonUtility.FromJson<int>(jsonLines[1]);
        classToLoad.itemsInventory = JsonUtility.FromJson<Item[]>(jsonLines[2]);
        classToLoad.weaponsInventory = JsonUtility.FromJson<WeaponInfo[]>(jsonLines[3]);
        classToLoad.mementosInventory = JsonUtility.FromJson<List<MementoInfo>>(jsonLines[4]);
        classToLoad.abilitiesInventory = JsonUtility.FromJson<List<AbilityInfo>>(jsonLines[5]);
        classToLoad.currentWeapon = JsonUtility.FromJson<WeaponInfo>(jsonLines[6]);
        classToLoad.itemOne = JsonUtility.FromJson<ItemInfo>(jsonLines[7]);
        classToLoad.currentMemento = JsonUtility.FromJson<MementoInfo>(jsonLines[8]);



        classToLoad.OverWriteData();
    }

    
}
