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
    bool objectiveSet = false;

    DialogueManager dialogueManager;
    PlayerInventory inventory;
    public event EventHandler OnQuestUpdate;
    private void Awake()
    {
        objectives = new Objectives();
        dialogueManager = FindObjectOfType<DialogueManager>();
        inventory = FindObjectOfType<PlayerInventory>();
        inventory.OnItemCollect += ItemPickUpDetected;
        inventory.OnEnergyCollect += EnergyPickUpDetected;
    }

    // Start is called before the first frame update
    void Start()
    {
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

    private void Update()
    {
        if (dialogueManager.getInCutscene() == false && objectiveSet == false)
        {
            LoadObjective();
        }
    }

    


    public void QuestEvent(QuestEventType EventType, string objectName)
    {
        if(currentObjective == null)
        {
            Debug.LogWarning("Quest Event ERROR. CurrentObjective is currently NULL");
        }

        switch (EventType)
        {
            case QuestEventType.RoomEnter:
                if (currentObjective.GetType() == typeof(Traverse))
                {
                    Traverse traverse = (Traverse)currentObjective;
                    if (traverse.GetRoomName() == objectName)
                    {
                        Debug.Log("Room Get");
                        NextQuest();
                    }
                }
                break;
            case QuestEventType.EnemyDeath:
                if(currentObjective.GetType() == typeof(Kill))
                {
                    Kill kill = (Kill)currentObjective;
                    kill.IncrementAmountKilled();

                    if(kill.GetAmountKilled() >= kill.GetTotal())
                    {
                        NextQuest();
                    }
                    else
                    {
                        currentObjective = kill;
                    }
                }
                else if(currentObjective.GetType() == typeof(KillSpecific))
                {
                    KillSpecific killSpecific = (KillSpecific)currentObjective;
                    killSpecific.IncrementAmountKilled(objectName);

                    if (killSpecific.GetAmountKilled() >= killSpecific.GetTotal())
                    {
                        NextQuest();
                    }
                    else
                    {
                        currentObjective = killSpecific;
                    }
                }
                break;
            case QuestEventType.NpcInteraction:
                if (currentObjective.GetType() == typeof(Talk))
                {
                    Talk talk = (Talk)currentObjective;
                    if (talk.GetNPC() == objectName)
                    {
                        NextQuest();
                    }
                }
                break;
            case QuestEventType.ItemObtained:
                if (currentObjective.GetType() == typeof(FindObject))
                {
                    FindObject findObject = (FindObject)currentObjective;
                    if(findObject.GetObjectName() == objectName)
                    {
                        NextQuest();
                    }
                }
                else if (currentObjective.GetType() == typeof(FindMultiple))
                {
                    FindMultiple findMultiple = (FindMultiple)currentObjective;
                    bool allItemsObtained = false;
                    
                    int index = 0;
                    foreach(string item in findMultiple.GetObjectNames())
                    {
                        if (item.Equals(objectName))
                        {
                            findMultiple.CheckItems(index);
                        }
                        index++;
                    }
                    foreach(bool itemCheck in findMultiple.CheckItems(-1))
                    {
                        if(itemCheck == false)
                        {
                            allItemsObtained = false;
                            break;
                        }
                        else
                        {
                            allItemsObtained = true;
                        }
                    }

                    if(allItemsObtained == true)
                    {
                        NextQuest();
                    }
                    else
                    {
                        currentObjective = findMultiple;
                    }
                }
                break;
            case QuestEventType.EnergyCollected:
                if(currentObjective.GetType() == typeof(Collect))
                {
                    Collect collect = (Collect)currentObjective;
                    collect.IncrementAmountCollected(Int32.Parse(objectName));
                    if(collect.GetAmountCollected() >= collect.GetTotal())
                    {
                        NextQuest();
                    }
                    else
                    {
                        currentObjective = collect;
                    }
                }
                break;
        }
    }

    void LoadObjective()
    {
        string quest = objectives.getObjective(floor, objectiveNum);
        Debug.Log("Current Objective Type (Before): " + currentObjective?.GetType());
        currentObjective = ParseQuestString(quest);
        Debug.Log("Current Objective Type (After): " + currentObjective?.GetType());
        if (currentObjective != null)
        {
            OnQuestUpdate?.Invoke(currentObjective, EventArgs.Empty);
            Debug.Log("Current Obj: " + currentObjective.ToSaveString());
        }
    }

    QuestType ParseQuestString(string questString)
    {
        QuestType questType = null;
        string[] parts = questString.Split('|');
        switch (parts[0])
        {
            case "FindObject":
                questType = new FindObject(parts[1]);
                return questType;
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
                questType = new FindMultiple(objsArray);
                return questType;
            case "Talk":
                GameObject npc = GameObject.Find(parts[1]);
                Dialogue.Dialog talkDialog;
                Enum.TryParse(parts[2], out talkDialog);
                questType = new Talk(npc, talkDialog);
                return questType;
            case "Kill":
                questType = new Kill(Int32.Parse(parts[1]), Int32.Parse(parts[2]));
                return questType;
            case "KillSpecific":
                questType = new KillSpecific(Int32.Parse(parts[1]), Int32.Parse(parts[2]), parts[3]);
                return questType;
            case "Collect":
                questType = new Collect(Int32.Parse(parts[1]), Int32.Parse(parts[2]));
                return questType;
            case "Traverse":
                questType = new Traverse(parts[1]);
                return questType;
            case "SetCutscene":
                Dialogue.Dialog dialog;
                Enum.TryParse(parts[1], true, out dialog);
                dialogueManager.SetCutscene(dialog);

                objectiveNum++;
                questType = ParseQuestString(objectives.getObjective(floor, objectiveNum));

                return questType;
        }

        if (questType == null)
        {
            Debug.LogWarning("Quest Parsing ERROR. CurrentObjective is currently NULL");
        }
        else
        {
            objectiveSet = true;
        }
        return questType;
    }

    public void NextQuest()
    {
        objectiveNum++;
        LoadObjective();
    }
    public void SetQuest(int floor, int objective)
    {
        this.floor = floor;
        this.objectiveNum = objective;
    }

    public enum QuestEventType
    {
        RoomEnter = 0,
        EnemyDeath = 1,
        NpcInteraction = 2,
        ItemObtained = 3,
        EnergyCollected = 4
    }

    #region Inventory Event Detectors
    void ItemPickUpDetected(object sender, EventArgs e)
    {
        QuestEvent(QuestEventType.ItemObtained, (string)sender);
    }

    void EnergyPickUpDetected(object sender, EventArgs e)
    {
        QuestEvent(QuestEventType.EnergyCollected, (string)sender);
    }
    #endregion
}