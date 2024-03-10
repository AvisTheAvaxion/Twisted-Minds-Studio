using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //Things to disable


    [SerializeField] GameObject DialogeBG;
    [SerializeField] List<string> dialogueLines;
    private void Awake()
    {
        DialogeBG = GameObject.Find("CanvasDialoge").transform.GetChild(0).gameObject;
    }

    void ToggleDialogueBox()
    {
        DialogeBG.SetActive(!DialogeBG.activeSelf);
        Cursor.visible = DialogeBG.activeSelf;
    }


    // DFlag decides the opened txt and what lines are read (000 = 1st floor first dialogue. 110 = 2nd floor tenth dialogue) </param>
    void RetrieveDialogue(int DFlag)
    {
        string textFileName = "";
        int flag = DFlag;
        switch (flag)
        {
            case int i when i < 099:
                textFileName = "F1D";
                break;
            case int i when i > 099 && i < 199:
                textFileName = "F2D";
                flag -= 100;
                break;
        }

        using(StreamReader reader = File.OpenText(Application.dataPath + "/Dialogue/" + textFileName + ".txt"))
        {
            string line;
            
            while ((line = reader.ReadLine()) != null)
            {
                if(line.Contains(flag + "|"))
                {
                    string removeString = flag + "|";

                    string cleanLine = line.Remove(0, removeString.Length);
                    dialogueLines.Add(cleanLine);
                }
            }
        }

        ChangeDialogue(0);
    }

    void ChangeDialogue(int index)
    {
        TextMeshProUGUI dialogueText = DialogeBG.GetComponentInChildren<TextMeshProUGUI>();
        if(index >= dialogueLines.Count)
        {
            ToggleDialogueBox();
            return;
        }
        dialogueText.text = dialogueLines[index];
    }
}
