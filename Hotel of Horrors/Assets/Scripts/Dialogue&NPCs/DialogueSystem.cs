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
    GameObject mainBG;
    TextMeshProUGUI mainText;
    TextMeshProUGUI nameText;
    List<GameObject> choices;

    //file that will be read. Should include the extentsion
    string fileToRead;
    //file should be blocked out into numbered sections
    int block;
    List<DialogueLine> dialogueLines;
    int choiceNum = 5;


    //Every time the Dialogue System is needed call this method
    //This method is broken down into bite sized methods
    public void StartDialogue(string fileName, int blockNum)
    {
        choices = new List<GameObject>();
        dialogueLines = new List<DialogueLine>();
        fileToRead = fileName;
        block = blockNum;
        ParseBlock();
        CreateVisual();
        PauseGame();
    }

    //This method deals with displaying the visuals and storing for future use.
    //If you want to add more visual bs put it here
    void CreateVisual()
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

    //The game gets paused here
    //If you want to add a sound or smth do so here
    void PauseGame()
    {
        Time.timeScale = 0;
    }

    //The block holding the desired text in the desired file will be located and shoved into a list of strings
    //No idea what you would want to put in here since this is backend stuff related to file reading.
    void ParseBlock()
    {
        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/" + fileToRead))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                //If the line is a standard line 
                if (line.Contains($":{block}|"))
                {
                    StandardLine standardLine = new StandardLine();
                    standardLine.ParseAndStore(line);
                    dialogueLines.Add(standardLine);
                }
                else if (line.Contains($":{block}#|"))
                {
                    ChoiceLine choiceLine = new ChoiceLine();
                    choiceLine.ParseAndStore(line);
                    int choiceLineNum = lineNumber;
                    dialogueLines.Add(choiceLine);

                    while ((line = reader.ReadLine()) != null && !line.Contains($":{block}#|"))
                    {
                        DivergentLine divergent = new DivergentLine();
                        switch (line)
                        {
                            case string a when a.Contains($":{block}a|"):
                               divergent.ParseAndStore(line);
                               choiceLine.AssignDivergentToChoice(divergent, 0);
                               break;
                            case string b when b.Contains($":{block}b|"):
                                divergent.ParseAndStore(line);
                                choiceLine.AssignDivergentToChoice(divergent, 1);
                                break;
                            case string c when c.Contains($":{block}c|"):
                                divergent.ParseAndStore(line);
                                choiceLine.AssignDivergentToChoice(divergent, 2);
                                break;
                            case string d when d.Contains($":{block}d|"):
                                divergent.ParseAndStore(line);
                                choiceLine.AssignDivergentToChoice(divergent, 3);
                                break;
                            case string e when e.Contains($":{block}e|"):
                                divergent.ParseAndStore(line);
                                choiceLine.AssignDivergentToChoice(divergent, 4);
                                break;
                        }

                    }
                }
                    lineNumber++;
            }
        }
    }


}

//Base class that all dialogue is based on
abstract class DialogueLine
{
    public string pureText;
    public string speaker;

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
            choice.ParseAndStore(ExtractChoices(line, @$"\[{i}(.*?){i}\]"));
        }

        string[] split = cleanString.Split(':');
        speaker = split[0];
        pureText = split[1];
    }

    string ExtractChoices(string input, string pattern)
    {
        string result;
        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(input);

        result = matches[0].Groups[1].Value;
        return result;
    }

    public void AssignToChoice(Choice choice)
    {
        choices.Add(choice);
    }

    public void AssignDivergentToChoice(DivergentLine divergent, int associatedChoice)
    {
        choices[associatedChoice].AddToResulting(divergent);
    }
}

//The choices themselves. They should be shown inside the buttons.
//I'm decideing to do things this way for now but may regret later.
class Choice : DialogueLine
{
    List<DivergentLine> resultingLines;
    public override void ParseAndStore(string line)
    {
        pureText = line;
        speaker = "CRINGE";
    }

    public void AddToResulting(DivergentLine divergentLine)
    {
        resultingLines.Add(divergentLine);
    }
}

//Dialogue that results from a choice the player makes.
//depending on the choice the player may never even see specific dialogue.
class DivergentLine : DialogueLine
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
