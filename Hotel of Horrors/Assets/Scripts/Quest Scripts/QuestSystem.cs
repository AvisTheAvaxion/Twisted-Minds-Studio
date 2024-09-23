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
            case QuestEventType.RoomEnter:
                break;
            case QuestEventType.EnemyDeath:
                break;
            case QuestEventType.NpcInteraction:
                break;
            case QuestEventType.ItemObtained:
                break;
            case QuestEventType.EnergyCollected:
                break;
        }
    }

    void LoadObjective()
    {
        string quest = objectives.getObjective(floor, objectiveNum);
        currentObjective = ParseQuestString(quest);
        Debug.Log("Current Obj: " + currentObjective.ToSaveString());
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
                List<string> objs = new List<string>();
                int index = 0;
                foreach(var part in parts)
                {
                    if (index == 0)
                    {
                        continue;
                    }
                    objs.Add(part);
                    index++;
                }
                string[] objsArray = objs.ToArray();
                type = new FindMultiple(objsArray);
                break;
            case "Talk":
                GameObject npc = GameObject.Find(parts[1]);
                Dialogue.Dialog talkDialog;
                Enum.TryParse(parts[2], out talkDialog);
                type = new Talk(npc, talkDialog);
                break;
            case "Kill":
                type = new Kill(Int32.Parse(parts[1]), Int32.Parse(parts[2]));
                break;
            case "KillSpecific":
                type = new KillSpecific(Int32.Parse(parts[1]), Int32.Parse(parts[2]), parts[3]);
                break;
            case "Collect":
                type = new Collect(Int32.Parse(parts[1]), Int32.Parse(parts[2]));
                break;
            case "Traverse":
                type = new Traverse(parts[1]);
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