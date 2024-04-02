using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //Things to disable


    [SerializeField] GameObject DialogueBG;
    [SerializeField] List<string> dialogueLines;
    [SerializeField] List<string> choices = new List<string>();
    [SerializeField] Choice playerChoice;
    MusicManager musicManager;
    public List<int> choiceIndex;
    int flag;
    private void Awake()
    {
        DialogueBG = GameObject.Find("CanvasDialogue").transform.GetChild(0).gameObject;
        musicManager = FindAnyObjectByType<MusicManager>();
        choiceIndex = new List<int>();
    }

    
    // DFlag decides the opened txt and what lines are read (000 = 1st floor first dialogue. 110 = 2nd floor tenth dialogue) </param>
    void RetrieveDialogue(int DFlag)
    {
        choices = new List<string>();
        dialogueLines = new List<string>();
        string textFileName = "";
        flag = DFlag;
        
        switch (flag)
        {
            case int i when i < 099:
                textFileName = "F0D";
                break;
            case int i when i > 099 && i < 199:
                textFileName = "F1D";
                flag -= 100;
                break;
        }

        using(StreamReader reader = File.OpenText(Application.dataPath + "/Dialogue/" + textFileName + ".txt"))
        {
            string line;
            
            while ((line = reader.ReadLine()) != null)
            {
                //if line is a choice line
                if(line.Contains(":" + flag + "#|"))
                {
                    string removeString = ":" + flag + "#|";
                    choices.Add(ExtractChoices(line, @"\[1(.*?)1\]"));
                    choices.Add(ExtractChoices(line, @"\[2(.*?)2\]"));


                    string cleanLine = line.Remove(0, removeString.Length);
                    cleanLine = cleanLine.Replace(choices[0], "");
                    cleanLine = cleanLine.Replace("[11]", "");
                    cleanLine = cleanLine.Replace(choices[1], "");
                    cleanLine = cleanLine.Replace("[22]", "");
                    dialogueLines.Add(cleanLine);
                    choiceIndex.Add(dialogueLines.IndexOf(cleanLine));
                }
                //if line is a regular line
                else if(line.Contains(":" + flag + "|"))
                {
                    dialogueLines.Add(line.Substring(line.IndexOf('|') + 1));
                }
            }
        }

        ToggleDialogueBox();
        ChangeDialogue(0);
    }

    public void RetrieveChoiceDialogue(int increment)
    {
        string textFileName = "";
        dialogueLines = new List<string>();

        switch (flag)
        {
            case int i when i < 099:
                textFileName = "F0D";
                break;
            case int i when i > 099 && i < 199:
                textFileName = "F1D";
                flag -= 100;
                break;
        }

        using (StreamReader reader = File.OpenText(Application.dataPath + "/Dialogue/" + textFileName + ".txt"))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                //if line is a choice line
                if (line.Contains(":"+flag + "#|"))
                {
                    string removeString = ":"+flag + "#|";
                    choices.Add(ExtractChoices(line, @"\[1(.*?)1\]"));
                    choices.Add(ExtractChoices(line, @"\[2(.*?)2\]"));


                    string cleanLine = line.Remove(0, removeString.Length);
                    cleanLine = cleanLine.Replace(choices[0], "");
                    cleanLine = cleanLine.Replace("[11]", "");
                    cleanLine = cleanLine.Replace(choices[1], "");
                    cleanLine = cleanLine.Replace("[22]", "");
                    dialogueLines.Add(cleanLine);
                    choiceIndex.Add(dialogueLines.IndexOf(cleanLine));
                }
                //if line is a regular line
                else if (line.Contains(":" + flag + "|"))
                {
                    dialogueLines.Add(line.Substring(line.IndexOf('|')+1));
                }
                //read choice one specific dialogue
                else if (playerChoice == Choice.One && line.Contains(":" + flag + "a|"))
                {
                    if (line.Contains("$Play"))
                    {
                        string[] songName = line.Split('(', ')');
                        if (songName[0] == "StopAll")
                        {
                            AudioManager.Stop("all");
                        }
                        else
                        {
                            AudioManager.Play(songName[1]);
                        }
                    }
                    else
                    {
                        dialogueLines.Add(line.Substring(line.IndexOf('|') + 1));
                    }
                }
                //read choice two specific dialogue
                else if (playerChoice == Choice.Two && line.Contains(":" + flag + "b|"))
                {
                    dialogueLines.Add(line.Substring(line.IndexOf('|') + 1));
                }
            }
        }

        ChangeDialogue(increment);
    }

    void ToggleDialogueBox()
    {
        DialogueBG.SetActive(!DialogueBG.activeSelf);
        DialogueBG.GetComponent<DialogueBoxInteractions>().ResetIncrement();
        DialogueBG.transform.GetChild(1).GetComponent<DialogueBoxInteractions>().ResetIncrement();
        DialogueBG.transform.GetChild(2).GetComponent<DialogueBoxInteractions>().ResetIncrement();
        Cursor.visible = DialogueBG.activeSelf;
    }

    void ChangeDialogue(int index)
    {
        TextMeshProUGUI dialogueText = DialogueBG.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(index == choiceIndex[0])
        {
            TextMeshProUGUI choiceOneText = DialogueBG.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI choiceTwoText = DialogueBG.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
            dialogueText.transform.parent.GetComponent<DialogueBoxInteractions>().ToggleInteraction(false);
            choiceOneText.transform.parent.gameObject.SetActive(true);
            choiceTwoText.transform.parent.gameObject.SetActive(true);
            choiceOneText.text = choices[0];
            choiceTwoText.text = choices[1];
            choiceIndex.Remove(choiceIndex[0]);
        }
        else
        {
            DialogueBG.transform.GetChild(1).gameObject.SetActive(false);
            DialogueBG.transform.GetChild(2).gameObject.SetActive(false);
            dialogueText.transform.parent.GetComponent<DialogueBoxInteractions>().ToggleInteraction(true);
        }
        Debug.Log("index: "+index+" |count: "+dialogueLines.Count);
        if(index >= dialogueLines.Count)
        {
            ToggleDialogueBox();
            playerChoice = Choice.None;
            return;
        }
        dialogueText.text = dialogueLines[index];
    }

    public void TakePlayerChoice(Choice choice)
    {
        playerChoice = choice;
    }

    string ExtractChoices(string input, string pattern)
    {
        string result;
        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(input);

        result = matches[0].Groups[1].Value;
        return result;
    }
}
