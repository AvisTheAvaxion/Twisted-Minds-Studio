using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SerializationManager : MonoBehaviour
{
    string saveFile;
    PlayerInventory inventory;

    private void Start()
    {
        saveFile = Application.persistentDataPath + "/saveinformation.json";
        inventory = FindObjectOfType<PlayerInventory>();

        StartCoroutine(StartLoading());
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
        //print(json);
        File.WriteAllText(saveFile, json);
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
        //print(json);

        SerializedClass classToLoad = new SerializedClass(inventory);

        //read the data from file and set jsondata

        JsonUtility.FromJsonOverwrite(json, classToLoad);

        classToLoad.OverWriteData(inventory);

        print("Loaded");
    }

    private void Update()
    {
        #if UNITY_EDITOR
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
        #endif
    }

    //method used to load the data at the start of the game
    //i used a coroutine because the game didn't like doing it exactly at the start, so i gave it a slight delay
    IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(.1f);
        LoadData();
    }
}
