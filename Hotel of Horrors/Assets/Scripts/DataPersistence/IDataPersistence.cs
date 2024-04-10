using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    //Note I followed this youtube tutorial to get this set up - if there are any issues refer back to it -Ben
    //https://www.youtube.com/watch?v=aUi9aijvpgs

    void LoadData(GameData data);
    void SaveData(ref GameData data);


}
