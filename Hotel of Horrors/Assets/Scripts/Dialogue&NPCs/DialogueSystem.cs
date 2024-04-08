using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

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
    int choiceNum;


    //Every time the Dialogue System is needed call this method
    //This method is broken down into bite sized methods
    public void StartDialogue(string fileName, int blockNum)
    {
        choices = new List<GameObject>();
        fileToRead = fileName;
        block = blockNum;
        //ReadBlock();
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
    void ReadBlock()
    {
        using (StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/" + fileToRead))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                //If the line is a standard line 
                if (line.Contains($":{block}|"))
                {
                    StandardLine standardLine = new StandardLine();
                    dialogueLines.Add(standardLine);
                }
                else if (line.Contains($":{block}#|"))
                {
                    ChoiceLine choiceLine = new ChoiceLine();
                    dialogueLines.Add(choiceLine);
                }
                //else if (line.Contains($":{block}a|"))
            }
        }
    }


}

//Base class that all dialogue is based on
abstract class DialogueLine
{
    string pureText;
    string speaker;

    public abstract void ParseAndStore(string line);
}

//A standard piece of dialogue that simply needs to be parsed and cleaned
class StandardLine : DialogueLine
{
    public override void ParseAndStore(string line)
    {
        throw new System.NotImplementedException();
    }
}

//Dialogue where the player must make a choice. Has choices that must be parsed and stored as well.
class ChoiceLine : DialogueLine
{
    List<Choice> choices;
    public override void ParseAndStore(string line)
    {
        throw new System.NotImplementedException();
    }
}

//The choices themselves. I'm decideing to do things this way for now but may regret later.
class Choice : DialogueLine
{
    List<DivergentLine> resultingLines;
    public override void ParseAndStore(string line)
    {
        throw new System.NotImplementedException();
    }
}

//Dialogue that results from a choice the player makes. depending on the choice the player may never even see the dialogue.
class DivergentLine : DialogueLine
{
    public override void ParseAndStore(string line)
    {
        throw new System.NotImplementedException();
    }
}
