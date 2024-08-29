using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] Dialogue.Dialog cutscene;

    TMPro.TMP_Text textBox;
    TMPro.TMP_Text buttonOneText;
    TMPro.TMP_Text buttonTwoText;
    Dialogue dialogue = new Dialogue();

    string[] lines;

    bool inCutscene = false;
    bool isPicking = false;
    int currentLine = 0;

    PlayerChoice currentChoice = PlayerChoice.None;

    private void Awake()
    {
        textBox = GameObject.Find("TextBox").GetComponent<TMPro.TMP_Text>();
        buttonOneText = GameObject.Find("ButtonOneText").GetComponent<TMPro.TMP_Text>();
        buttonTwoText = GameObject.Find("ButtonTwoText").GetComponent<TMPro.TMP_Text>();
        ButtonSwitch(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCutScene(cutscene.ToString(), 0);
    }

    public void StartCutScene(string dialogueName, int line)
    {
        lines = dialogue.getDialogue(dialogueName);
        currentLine = line;
        inCutscene = true;
        OnDialogueUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0) && inCutscene && !isPicking))
        {
            OnDialogueUpdate();
        }
    }

    void OnDialogueUpdate()
    {
        //While their are still lines of dialog left to read.
        if (currentLine < lines.Length)
        {
            //Parse the line for relevent information and do corresponding actions
            if (lines[currentLine].StartsWith("$Playsound"))
            {
                string sound = ""; //ignore this line for now
                currentLine++;
            }
            else if (lines[currentLine].StartsWith("$Prompt"))
            {
                isPicking = true;
                ButtonSwitch(true);
                string currentText = lines[currentLine].Replace("$Prompt", "");
                string[] splitString = currentText.Split('|');
                string promptText = splitString[0];
                string bOneText = splitString[1];
                string bTwoText = splitString[2];
                textBox.text = promptText;
                buttonOneText.text = bOneText;
                buttonTwoText.text = bTwoText;
            }
            else if (lines[currentLine].StartsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceOne)
            {
                string optionAText = lines[currentLine].Replace("$OptionA", "");
                textBox.text = optionAText;
            }
            else if (lines[currentLine].StartsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceOne)
            {
                while (lines[currentLine].Contains("$OptionB"))
                {
                    currentLine++;
                }
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].StartsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                string optionBText = lines[currentLine].Replace("$OptionB", "");
                textBox.text = optionBText;
            }
            else if (lines[currentLine].StartsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                while (lines[currentLine].Contains("$OptionA"))
                {
                    currentLine++;
                }
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].StartsWith("$Italic"))
            {
                textBox.fontStyle = TMPro.FontStyles.Italic;
                textBox.text = lines[currentLine];
            }
            else
            {
                textBox.fontStyle = TMPro.FontStyles.Normal;
                currentChoice = PlayerChoice.None;
                textBox.text = lines[currentLine];
            }
            currentLine++;
        }
        //When there are no more lines of dialog to read.
        else if(currentLine > lines.Length)
        {
            //This happens when the last line of dialog is on screen .
            //Any cleaning up that should happen at the end of the cutscene should be here.
            //e.g. closing the text box, turning general GUI back on, returning control to the player, etc.
        }
    }

    #region Button Behavior
    public void ButtonOneSelect()
    {
        currentChoice = PlayerChoice.ChoiceOne;
        ButtonSwitch(false);
        while (lines[currentLine].Contains("$OptionB"))
        {
            currentLine++;
        }
        OnDialogueUpdate();
        isPicking = false;
    }
    public void ButtonTwoSelect()
    {
        currentChoice = PlayerChoice.ChoiceTwo;
        ButtonSwitch(false);
        while (lines[currentLine].Contains("$OptionA"))
        {
            currentLine++;
        }
        OnDialogueUpdate();
        isPicking=false;
    }

    void ButtonSwitch(bool isButtonOn)
    {
        buttonOneText.transform.parent.gameObject.SetActive(isButtonOn);
        buttonTwoText.transform.parent.gameObject.SetActive(isButtonOn);
    }
    #endregion

    enum PlayerChoice
    {
        None,
        ChoiceOne,
        ChoiceTwo,
    }
}




