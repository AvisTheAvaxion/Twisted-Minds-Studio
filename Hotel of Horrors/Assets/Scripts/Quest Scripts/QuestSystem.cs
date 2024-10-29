using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] int floor;
    [SerializeField] int objectiveNum;

    public int FloorNum { get => floor; }
    public int ObjectiveNum { get => objectiveNum; }

    QuestType currentObjective;
    Objectives objectives;
    [SerializeField] bool objectiveSet = false;
    [SerializeField] bool floorCleared = false;

    DialogueManager dialogueManager;
    PlayerInventory inventory;
    Floor currentFloor;
    QuestGUI questGUI;
    private void Awake()
    {
        objectives = new Objectives();
        dialogueManager = FindObjectOfType<DialogueManager>();
        inventory = FindObjectOfType<PlayerInventory>();
        currentFloor = FindObjectOfType<Floor>();
        questGUI = FindObjectOfType<QuestGUI>();
        inventory.OnItemCollect += ItemPickUpDetected;
        inventory.OnEnergyCollect += EnergyPickUpDetected;
        floorCleared = false;
    }

    private void Update()
    {
        if (dialogueManager.getCutsceneState() == DialogueManager.CutsceneState.None && objectiveSet == false && floorCleared == false)
        {
            LoadObjective();
        }
    }

    //This method should be called whenever a relevant game event occurs and see if quest conditions have been fulfilled
    public void QuestEvent(QuestEventType EventType, string objectName)
    {
        if(currentObjective == null)
        {
            Debug.LogWarning("Quest Event ERROR. CurrentObjective is currently NULL");
            return;
        }

        switch (EventType)
        {
            case QuestEventType.RoomEnter:
                if (currentObjective.GetType() == typeof(Traverse))
                {
                    Traverse traverse = (Traverse)currentObjective;
                    if (traverse.GetRoomName() == objectName)
                    {
                        //Debug.Log("Room Get");
                        NextQuest();
                    }
                }
                break;
            case QuestEventType.EnemyDeath:
                if (currentObjective.GetType() == typeof(KillSpecific))
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
                else if (currentObjective.GetType() == typeof(Kill))
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
            case QuestEventType.CutsceneEnd:
                if (currentObjective.GetType() == typeof(SetCutscene))
                {
                    SetCutscene cutscene = (SetCutscene)currentObjective;
                    if (cutscene.GetCutsceneName() == objectName)
                    {
                        //Debug.Log("Cutscene " + objectName + " Over");
                        NextQuest();
                    }
                }
                break;
            case QuestEventType.LootableBroken:
                if (currentObjective.GetType() == typeof(BreakLootable))
                {
                    BreakLootable qEvent = (BreakLootable)currentObjective;
                    qEvent.Increment();
                    if (qEvent.ConditionReached())
                    {
                        //Debug.Log("Cutscene " + objectName + " Over");
                        NextQuest();
                    }
                }
                break;
            case QuestEventType.TripTrigger:
                if(currentObjective.GetType() == typeof(TripTrigger))
                {
                    TripTrigger trigger = (TripTrigger)currentObjective;
                    if(trigger.GetExpectedTriggerName() == objectName)
                    {
                        //Debug.Log("Quest " + objectName + " Happening");
                        NextQuest();
                    }
                }
                break;
            case QuestEventType.InventoryOpened:
                if (currentObjective.GetType() == typeof(OpenInventory))
                {
                    NextQuest();
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
                    collect.IncrementAmountCollected(int.Parse(objectName));
                    print("Energy Collected for quest: " + objectName + ", " + collect.GetAmountCollected());
                    if (collect.GetAmountCollected() >= collect.GetTotal())
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
        SetRequiredGameState(floor, objectiveNum);
    }

    //This method is called to set the variable, currentObjective, to a QuestType with the helper method ParseQuestString
    //Should be called whenever the user wants to set the currentObjective after changing the floor and ObjectiveNum accordingly
    void LoadObjective()
    {
        string quest = objectives.getObjective(floor, objectiveNum);
        currentObjective = ParseQuestString(quest);
        objectiveSet = true;
    }

    //Helper method for LoadObjective. Takes in a string and returns the relevent QuestType.
    //Can also start cutscenes, Although it is a bit of a recursion mess.
    QuestType ParseQuestString(string questString)
    {
        
        QuestType questType = null;
        string[] parts = questString.Split('|');
        switch (parts[0])
        {
            #region Quest Type Functions
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
                if(parts.Length > 2)
                    currentFloor.AddToGuaranteeRooms(parts[2]);
                questType = new Talk(npc);
                return questType;
            case "TripTrigger":
                questType = new TripTrigger(parts[1]);
                return questType;
            case "Kill":
                questType = new Kill(int.Parse(parts[1]));
                return questType;
            case "KillSpecific":
                questType = new KillSpecific(int.Parse(parts[1]), parts[2]);
                return questType;
            case "Collect":
                questType = new Collect(int.Parse(parts[1]));
                return questType;
            case "BreakLootable":
                questType = new BreakLootable(int.Parse(parts[1]));
                return questType;
            case "Traverse":
                questType = new Traverse(parts[1]);
                currentFloor.AddToGuaranteeRooms(parts[1]);
                return questType;
            #endregion
            #region Auxilary Functions
            case "SetCutscene":
                Dialogue.Dialog dialog;
                Enum.TryParse(parts[1], true, out dialog);
                //Debug.Log("QuestSystem: " + dialog.ToString() + " String: " + questString + " Part: " + parts[1] + " ObjectiveNum: " + objectiveNum);
                questType = new SetCutscene(dialog.ToString());
                dialogueManager.SetCutscene(dialog);
                break;
            case "OpenInventory":
                questType = new OpenInventory();
                break;
            case "ClearFloor":
                floorCleared = true;
                SetRequiredGameState(floor, objectiveNum);
                break;
             #endregion
        }

        if (questType == null)
        {
            objectiveSet = false;
        }
        else
        {
            objectiveSet = true;
        }

        return questType;
    }

    void SetRequiredGameState(int floor, int objective)
    {
        int playerChoice = 0;
        if (floor == 0)
        {
            switch(objective)
            {
                case 0:
                    SetQuestTitle("Explore");
                    break;
                case 3:
                    playerChoice = (int)dialogueManager.GetLastPlayerChoice();
                    if (playerChoice == 1) //Player wants to do tutorial
                    {
                        SetQuestTitle("Continue");
                        SetQuestDesc("Follow the green lady");
                        TeleportGameobject("East Door To Varren Intro", new Vector2(6.971862f, -6.34f), false);
                        TeleportGameobject("East Door To InventoryTut", new Vector2(6.971862f, -0.6041539f), false);
                    }
                    else if(playerChoice == 2) // Player wants to skip tutorial
                    {
                        SetQuest(0, 13);
                        LoadObjective();
                        //Do nothing with doors, door to varren is in accessable place from start
                        //SetQuest to the last quest
                    }
                    break;
                case 5:
                    playerChoice = (int)dialogueManager.GetLastPlayerChoice();
                    
                    if (playerChoice == 1) //Player wants to do Inventroy Tutorial
                    {
                        TeleportGameobject("South Door To InventoryTutorial", new Vector2(-0.009889603f, -1.899884f), false);
                        TeleportGameobject("South Door To MindRoom", new Vector2(5.14f, -1.899884f), false);
                    }
                    else if (playerChoice == 2) // Player wants to move to next tutorial
                    {
                        SetQuest(0, 8);
                        LoadObjective();
                        //Do nothing with doors, door to mindroom is in accessable place from start
                        //Set quest to the Talk to SlimeLady(CombatIntro)
                    }
                    break;
                case 10:
                    playerChoice = (int)dialogueManager.GetLastPlayerChoice();
                    if (playerChoice == 1) //Player wants to do Combat Tutorial
                    {
                        TeleportGameobject("East Door To Varren Intro2", new Vector2(6.971862f, -6.34f), false);
                        TeleportGameobject("East Door To CombatTut", new Vector2(6.971862f, -0.6041539f), false);
                        //Activate Shooters in the room somehow
                    }
                    else if (playerChoice == 2) // Player wants to move to end tutorial
                    {
                        SetQuestTitle("Explore");
                        SetQuestDesc("Seek a way out of this place");
                        //Do nothing with doors, door to varren is in accessable place from start
                        //SetQuest to the last quest
                    }
                    break;
                case 13: //Spawn Enemies for combat tutorial
                    ActivateSpawner("Combat Tutorial");
                    break;
            }
        }
        else if(floor == 1)
        {
            switch (objective)
            {
                case 0:
                    SetQuestTitle("Explore Floor 1");
                    TeleportGameobject("DrHarris(Intro)", new Vector2(55.19f, -27.79f), true);
                    break;
                case 1:
                    Collect collect = (Collect)currentObjective;
                    SetQuestTitle("Herb Collecting");
                    SetQuestDesc($"Herbs Collected: {collect.GetAmountCollected()}/{collect.GetTotal()}");
                    break;
                case 3:
                    SetQuestTitle("Herb Delivery");
                    SetQuestDesc("Return to Dr. Harris");
                    TeleportGameobject("DrHarris(Intro)", new Vector2(51f, -27f), true);
                    TeleportGameobject("DrHarris(Q2)", new Vector2(55.19f, -27.79f), true);
                    break;
                case 4:
                    Kill kill = (Kill)currentObjective;
                    SetQuestTitle("Blood Donation");
                    SetQuestDesc($"Enemies Defeated: {kill.GetAmountKilled()}/{kill.GetTotal()}");
                    break;
                case 6:
                    SetQuestTitle("Blood Delivery");
                    SetQuestDesc("Return to Dr. Harris");
                    TeleportGameobject("DrHarris(Q2)", new Vector2(51f, -27f), true);
                    TeleportGameobject("DrHarris(Q3)", new Vector2(55.19f, -27.79f), true);
                    break;
                case 7:
                    SetQuestTitle("The Big Day");
                    SetQuestDesc("Get the patient's jersey");
                    TeleportGameobject("FrankJersey(Quest)", new Vector2(63.3892f, -28.0588f), true);
                    break;
                case 8:
                    SetQuestTitle("Confrontation");
                    SetQuestDesc("");
                    TeleportGameobject("FrankJersey(Quest)", new Vector2(55.19f, -27.79f), true);
                    currentFloor.UnlockToBossDoor();
                    break;
                case 9:
                    SetQuestTitle("The Second Floor");
                    SetQuestDesc("Go to the elevator");
                    break;
            }
        }
    }

    void TeleportGameobject(string desiredObjectName, Vector2 position, bool globalCords)
    {
        if (globalCords)
        {
            GameObject.Find(desiredObjectName).transform.position = position;
        }
        else
        {
            GameObject.Find(desiredObjectName).transform.localPosition = position;
        }
    }

    void ActivateSpawner(string roomName)
    {
        EnemySpawner spawner = GameObject.Find(roomName).transform.GetComponentInChildren<EnemySpawner>();
        spawner.isActiviated = true;
    }

    void SetQuestTitle(string newTitle)
    {
        questGUI.SetQuestTitle(newTitle);
    }

    void SetQuestDesc(string newDescription)
    {
        questGUI.SetQuestDesc(newDescription);
    }

    //Call when the player has fulfilled the conditions to move to the next objective
    public void NextQuest()
    {
        objectiveNum++;
        objectiveSet = false;
        LoadObjective();
    }

    public void NextFloor()
    {
        objectiveNum = 0;
        floor++;
    }

    //Call to set the quest
    public void SetQuest(int floor, int objective)
    {
        this.floor = floor;
        this.objectiveNum = objective;
    }

    //
    public enum QuestEventType
    {
        RoomEnter = 0,
        EnemyDeath = 1,
        NpcInteraction = 2,
        ItemObtained = 3,
        EnergyCollected = 4,
        TripTrigger = 5,
        CutsceneEnd = 6,
        InventoryOpened = 7,
        LootableBroken = 8,
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

    public void LoadQuest(SerializedClass saveData)
    {
        if (saveData.questSaves != null)
        {
            QuestSave currentSave = saveData.questSaves[floor - 1];
            if (floor == currentSave.floorNum)
            {
                objectiveNum = currentSave.objectiveNum;
                currentFloor.ClearGuaranteeRooms();
                LoadObjective();
            }
        }
    }
}

