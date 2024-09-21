using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    int floor;
    int objectiveNum;

    QuestType currentObjective;


    [SerializeField] Objectives Objectives;


    

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("idkWhatWeAreSavingThisAs"))
            //This might be it's own function
            //depending on how Ben is doing data persistancy with multiple save files
        {
            try
            {
                floor = PlayerPrefs.GetInt("QuestFloor");
                objectiveNum = PlayerPrefs.GetInt("Objective");
            }
            catch (Exception e)
            {
                Debug.Log("[Quest System] No save found!");
                floor = 0;
                objectiveNum = 0;
            }
            LoadObjective();
        }
    }


    public void QuestEvent(QuestEventType EventType, string objectName)
    {
        switch (EventType)
        {

        }
    }

    void LoadObjective()
    {

    }


    public enum QuestEventType
    {
        RoomEnter = 0,
        EnemyDeath = 1,
        NpcInteraction = 2,
        ItemObtained = 3,
        EnergyCollected = 4
    }

}
