using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{

    public void SaveData()
    {
        //check if file path exists

        //check if there is data in the file

        SerializedClass classToSave = new SerializedClass();
        string jsonData = JsonUtility.ToJson(classToSave);
        
        //write JSON data to file
        
    }

    public void loadData()
    {
        string jsonData = "";

        //read the data from file and set jsondata

        SerializedClass classToLoad = new SerializedClass();

        JsonUtility.FromJsonOverwrite(jsonData, classToLoad);

        classToLoad.OverWriteData();
    }

    
}
