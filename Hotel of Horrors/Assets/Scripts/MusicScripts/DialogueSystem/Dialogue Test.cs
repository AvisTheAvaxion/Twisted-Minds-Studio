using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text textBox;
    [SerializeField] TMPro.TMP_Text buttonOneText;
    [SerializeField] TMPro.TMP_Text buttonTwoText;
    Dialogue dialogue = new Dialogue();

    string[] lines;

    bool inCutscene = false;
    bool isPicking = false;
    [SerializeField] int currentLine = 0;

    PlayerChoice currentChoice = PlayerChoice.None;

    // Start is called before the first frame update
    void Start()
    {
        StartCutScene("VarrenEncounter", 0);
    }

    public void StartCutScene(string dialogueName, int line)
    {
        lines = dialogue.getDialogue(dialogueName);
        currentLine = line;
        inCutscene = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space)) || Input.GetKeyDown(KeyCode.Mouse0) && inCutscene && !isPicking)
        {
            onDialogueUpdate();
        }
        
    }

    void onDialogueUpdate()
    {
        if (lines[currentLine].StartsWith("$"))
        {
            if (lines[currentLine].StartsWith("$Playsound"))
            {
                string sound = ""; //ignore this line for now
                currentLine++;
            }
            else if (lines[currentLine].StartsWith("$Prompt"))
            {
                isPicking = true;
                buttonOneText.transform.parent.gameObject.SetActive(true);
                buttonTwoText.transform.parent.gameObject.SetActive(true);
                string currentText = lines[currentLine].Replace("$Prompt", "");
                string[] splitString = currentText.Split('|');
                string promptText = splitString[0];
                string bOneText = splitString[1];
                string bTwoText = splitString[2];
                textBox.text = promptText;
                buttonOneText.text = bOneText;
                buttonTwoText.text = bTwoText;
            }
            else if(lines[currentLine].StartsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceOne)
            {
                textBox.text = lines[currentLine];
            }
            else if (lines[currentLine].StartsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                textBox.text = lines[currentLine];
            }
            else if (lines[currentLine].StartsWith("$Italic"))
            {
                textBox.fontStyle = TMPro.FontStyles.Italic;
                textBox.text = lines[currentLine];
            }
        }
        else
        {
            textBox.fontStyle = TMPro.FontStyles.Normal;
            currentChoice = PlayerChoice.None;
            textBox.text = lines[currentLine];
        }
        currentLine++;
    }

    public void ButtonOneSelect()
    {
        currentChoice = PlayerChoice.ChoiceOne;
        buttonOneText.transform.parent.gameObject.SetActive(false);
        buttonTwoText.transform.parent.gameObject.SetActive(false);
        onDialogueUpdate();
        /*while (lines[currentLine].Contains("$OptionB"))
        {
            currentLine++;
        }*/
        isPicking = false;
        
    }
    public void ButtonTwoSelect()
    {
        currentChoice = PlayerChoice.ChoiceTwo;
        buttonOneText.transform.parent.gameObject.SetActive(false);
        buttonTwoText.transform.parent.gameObject.SetActive(false);
        currentLine++;
        while (lines[currentLine].Contains("$OptionA"))
        {
            currentLine++;
        }
        onDialogueUpdate();
        isPicking=false;
    }

    enum PlayerChoice
    {
        None,
        ChoiceOne,
        ChoiceTwo,
    }
}




