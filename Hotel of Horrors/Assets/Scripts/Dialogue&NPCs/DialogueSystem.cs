using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class DialogueSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject canvasPrefab;
    NPCInteraction npcInteraction;
    GameObject mainBG;
    TextMeshProUGUI mainText;
    TextMeshProUGUI nameText;
    List<GameObject> choices;

    //file that will be read. Should include the extentsion
    string fileToRead;
    //file should be blocked out into numbered sections
    int block;
    List<DialogueLine> dialogueLines;
    int choiceNum;

    private void Awake()
    {
        npcInteraction = FindObjectOfType<NPCInteraction>();
        npcInteraction.OnPlayerTalk += NpcInteraction_OnPlayerTalk;
        //This is just here for testing. Get Rid of it if you want.
        //FindObjectOfType<DialogueSystem>().StartDialogue("F0D.txt", 0);
    }

    private void NpcInteraction_OnPlayerTalk(object sender, System.EventArgs e)
    {
        NPCArgs args = (NPCArgs)e;
        StartDialogue(args.GetFile(), args.GetBlock());
    }

    //Every time the Dialogue System is needed call this method
    //This method is broken down into bite sized methods
    //Im probs gonna have to do events for this...FUUUUUUUUUUUUUUUUUUUUUUUUU
    public void StartDialogue(string fileName, int blockNum)
    {
        choices = new List<GameObject>();
        dialogueLines = new List<DialogueLine>();
        fileToRead = fileName;
        block = blockNum;
        Debug.Log(fileName + " | " +  blockNum);
        ParseBlock();
        CreateBaseVisuals();
        PauseGame();
        UpdateVisuals(dialogueLines[0]);

        //THIS IS DEBUG STUFF. DONT WORRY ABOUT IT!!!11!!
        for(int i = 0; i < dialogueLines.Count; i++)
        {
            DialogueLine line = dialogueLines[i];
            Debug.Log(line.speaker + ": " + line.pureText);
            if (line.GetType() == typeof(ChoiceLine))
            {
                ChoiceLine cLine = (ChoiceLine)line;
                cLine.SetChoiceResults(1, ref dialogueLines);
            }
        }
    }

    //This method deals with displaying the visuals and storing for future use.
    //If you want to add more visual bs put it here
    void CreateBaseVisuals()
    {
        GameObject canvas = Instantiate(canvasPrefab);
        mainBG = canvas.transform.GetChild(0).gameObject;
        mainText = mainBG.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText = mainBG.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        GameObject choiceObj = canvas.transform.GetChild(1).GetChild(0).gameObject;
        choices.Add(choiceObj);
        for(int i = 1; i < choiceNum; i++)
        {
            choices.Add(Instantiate(choiceObj, choiceObj.transform.parent));
        }
    }

    //This method deals with the actual text being displayed on top of the dialogue GUI
    //If you want to mess with the TextMeshProUGUI stuff do so here
    //Takes in a DialogueLine from the dialogueLines List
    void UpdateVisuals(DialogueLine line)
    {
        if(line.GetType() != typeof(ChoiceLine))
        {
            foreach(GameObject choiceBox in choices){
                choiceBox.SetActive(false);
            }
        }
        else
        {
            ChoiceLine choiceLine = (ChoiceLine)line;
            for(int i = 0;  i < choices.Count; i++)
            {
                choices[i].SetActive(true);
                TextMeshProUGUI choiceTextBox = choices[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                choiceTextBox.text = choiceLine.GetChoice(i).pureText;
            }
        }

        nameText.text = line.speaker;
        mainText.text = line.pureText;
    }

    //The game gets paused here
    //If you want to add a sound or smth do so here
    void PauseGame()
    {
        Time.timeScale = 0;
    }

    //The block holding the desired text in the desired file will be located and shoved into a list of strings
    //No idea what you would want to put in here since this is backend stuff related to file reading, encoding, interpreting, and all that jazz.
    void ParseBlock()
    {
        //Streaming Assets is a file in Assets and should also carry over in builds. This hopefully will allow for builds to work.
        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/" + fileToRead))
        {
            string line;
            int lineNumber = 0;
            int aNum, bNum, cNum, dNum, eNum;
            while ((line = reader.ReadLine()) != null)
            {
                //If the line is a standard line 
                if (line.Contains($":{block}|"))
                {
                    Debug.Log($"Standard line {lineNumber}: "+line);
                    StandardLine standardLine = new StandardLine();
                    standardLine.ParseAndStore(line);
                    dialogueLines.Add(standardLine);
                    lineNumber++;
                }
                //If the line contains a choice
                else if (line.Contains($":{block}#|"))
                {
                    Debug.Log($"Standard line  {lineNumber}: " + line);
                    ChoiceLine choiceLine = new ChoiceLine();
                    choiceLine.ParseAndStore(line);
                    choiceNum = choiceLine.ChoiceAmount();
                    dialogueLines.Add(choiceLine);

                    aNum = lineNumber;
                    bNum = lineNumber;
                    cNum = lineNumber;
                    dNum = lineNumber;
                    eNum = lineNumber;
                    
                    //If lines after the choice line contain divergent lines. Can hold up to 5 choices.
                    while ((line = reader.ReadLine()) != null && !line.Contains($":{block}#|"))
                    {
                        lineNumber++;
                        DivergentLine divergent = new DivergentLine();
                        switch (line)
                        {
                            case string a when a.Contains($":{block}a|"):
                                aNum++;
                                Debug.Log($"Divergent A line {aNum}: " + line);
                                divergent.ParseAndStore(line);
                                divergent.SetLineNumber(aNum);
                                choiceLine.AssignDivergentToChoice(divergent, 0);
                               break;
                            case string b when b.Contains($":{block}b|"):
                                bNum++;
                                Debug.Log($"Standard line {bNum}: " + line);
                                divergent.ParseAndStore(line);
                                divergent.SetLineNumber(bNum);
                                choiceLine.AssignDivergentToChoice(divergent, 1);
                                break;
                            case string c when c.Contains($":{block}c|"):
                                cNum++;
                                Debug.Log($"Standard line {cNum} : " + line);
                                divergent.ParseAndStore(line);
                                divergent.SetLineNumber(cNum);
                                choiceLine.AssignDivergentToChoice(divergent, 2);
                                break;
                            case string d when d.Contains($":{block}d|"):
                                dNum++;
                                Debug.Log($"Standard line {dNum} : " + line);
                                divergent.ParseAndStore(line);
                                divergent.SetLineNumber(dNum);
                                choiceLine.AssignDivergentToChoice(divergent, 3);
                                break;
                            case string e when e.Contains($":{block}e|"):
                                eNum++;
                                Debug.Log($"Divergent E line {eNum}: " + line);
                                divergent.ParseAndStore(line);
                                divergent.SetLineNumber(eNum);
                                choiceLine.AssignDivergentToChoice(divergent, 4);
                                break;
                        }
                    }
                }
            }
        }
    }

    void OnPlayerTalk()
    {

    }
}

//Base class that all dialogue is based on
abstract class DialogueLine
{
    public string pureText;
    public string speaker;

    //This method will read the string from the string and remove encoding
    public abstract void ParseAndStore(string line);
}

//A standard piece of dialogue that simply needs to be parsed and cleaned
class StandardLine : DialogueLine
{
    public override void ParseAndStore(string line)
    {
        string cleanString;
        cleanString = line.Substring(line.IndexOf('|') + 1);
        string[] split = cleanString.Split(':');
        speaker = split[0];
        pureText = split[1];
    }
}

//Dialogue where the player must make a choice.
//This is the line in the big text box during choices. Has choices that must be parsed and stored as well.
class ChoiceLine : DialogueLine
{
    List<Choice> choices = new List<Choice>();
    public override void ParseAndStore(string line)
    {
        string cleanString;
        cleanString = line.Substring(line.IndexOf('|') + 1);

        int numberOfChoices = 0;
        switch (line)
        {
            case string a when a.Contains($"5]"):
                numberOfChoices = 5;
                break;
            case string b when b.Contains($"4]"):
                numberOfChoices = 4;
                break;
            case string c when c.Contains($"3]"):
                numberOfChoices = 3;
                break;
            case string d when d.Contains($"2]"):
                numberOfChoices = 2;
                break;
            case string e when e.Contains($"1]"):
                numberOfChoices = 1;
                break;
        }

        for(int i=1; i <= numberOfChoices; i++)
        {
            Choice choice = new Choice();
            Debug.Log("Extracted Choice: " + ExtractChoices(line, @$"\[{i}(.*?){i}\]"));
            choice.ParseAndStore(ExtractChoices(line, @$"\[{i}(.*?){i}\]"));
            AssignToChoice(choice);
        }

        string[] split = cleanString.Split(':');
        cleanString = split[1].Split('[')[0];
        speaker = split[0];
        pureText = cleanString;
    }

    //After the player makes a choice this should be called to update the dialogue List with the relevent contextual dialogue
    public void SetChoiceResults(int choiceChosen, ref List<DialogueLine> dialogue)
    {
        foreach(DivergentLine dLine in choices[choiceChosen].GetResultLines())
        {
            dialogue.Insert(dLine.GetLineNumber(), dLine);
        }
    }

    public int ChoiceAmount()
    {
        return choices.Count;
    }

    public Choice GetChoice(int choiceIndex)
    {
        return choices[choiceIndex];
    }

    //Uses Regex to get the different choices the player can make out of the encoding pattern
    string ExtractChoices(string input, string pattern)
    {
        string result;
        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(input);

        result = matches[0].Groups[1].Value;
        return result;
    }

    //Adds a Choice to the choices List
    public void AssignToChoice(Choice choice)
    {
        choices.Add(choice);
    }

    //Adds a Divergent Line to a specific Choice in the choices List
    public void AssignDivergentToChoice(DivergentLine divergent, int associatedChoice)
    {
        choices[associatedChoice].AddToResulting(divergent);
    }
}

//The choices themselves. They should be shown inside the buttons.
//I'm decideing to do things this way for now but may regret later.
class Choice : DialogueLine
{
    List<DivergentLine> resultingLines = new List<DivergentLine>();
    public override void ParseAndStore(string line)
    {
        pureText = line;
    }

    //Adds a Divergent Line directly into the resultingLines List
    public void AddToResulting(DivergentLine divergentLine)
    {
        resultingLines.Add(divergentLine);
    }

    //Retrieves the resultingLines List
    public List<DivergentLine> GetResultLines()
    {
        return resultingLines;
    }
}

//Dialogue that results from a choice the player makes.
//depending on the choice the player may never even see specific dialogue.
class DivergentLine : DialogueLine
{
    int lineNumber = 0;
    public override void ParseAndStore(string line)
    {
        string cleanString;
        cleanString = line.Substring(line.IndexOf('|') + 1);
        string[] split = cleanString.Split(':');
        speaker = split[0];
        pureText = split[1];
    }

    //Sets the line number. The line number should represent the index this specific piece of contextual dialogue is inside the dialogue List
    public void SetLineNumber(int lineNumber)
    {
        this.lineNumber = lineNumber;
    }

    //Sets the line number. Should be called when about to merge this specific piece of contextual dialogue into the dialogue List
    public int GetLineNumber()
    {
        return lineNumber;
    }
}