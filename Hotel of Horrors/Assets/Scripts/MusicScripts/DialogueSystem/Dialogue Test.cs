using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text textBox;
    Dialogue dialogue = new Dialogue();

    string[] lines;

    bool inCutscene = false;
    int currentLine = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCutScene("Tutorial", 0);
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
        if ((Input.GetKeyDown(KeyCode.Space)) && inCutscene)
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
            return;
        }
        else
        {
            textBox.text = lines[currentLine];
        }

        currentLine++;
    }
}




