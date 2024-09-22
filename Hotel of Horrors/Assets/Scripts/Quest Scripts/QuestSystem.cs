using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] int floor;
    [SerializeField] int objectiveNum;

    QuestType currentObjective;
    Objectives objectives;

    DialogueManager dialogueManager;

    private void Awake()
    {
        objectives = new Objectives();
        if (FindObjectOfType<DialogueManager>())
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadObjective();
        /*if (PlayerPrefs.HasKey("idkWhatWeAreSavingThisAs"))
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
        }*/
    }


    public void QuestEvent(QuestEventType EventType, string objectName)
    {
        switch (EventType)
        {

        }
    }

    void LoadObjective()
    {
        switch (floor)
        {
            case 0:
                string quest = objectives.getFloor(floor)[objectiveNum];
                Debug.Log(quest);
                currentObjective = ParseQuestString(quest);
                Debug.Log(currentObjective);
                break;
        }
    }

    QuestType ParseQuestString(string questString)
    {
        QuestType type = null;
        string[] parts = questString.Split('|');
        Debug.Log(Dialogue.Dialog.SameRoomAgain);
        switch (parts[0])
        {
            case "FindObject":
                type = new FindObject(parts[1]);
                break;
            case "FindMultiple":
                type = new FindMultiple();
                break;
            case "Talk":
                type = new Talk();
                break;
            case "Kill":
                type = new Kill();
                break;
            case "KillSpecific":
                type = new KillSpecific();
                break;
            case "Collect":
                type = new Collect();
                break;
            case "Traverse":
                type = new Traverse();
                break;
            case "SetCutsene":
                Dialogue.Dialog dialog;
                Enum.TryParse(parts[1], true, out dialog);
                dialogueManager.SetCutscene(dialog);
                break;
        }
        return type;
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
