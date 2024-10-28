using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueGUI : MonoBehaviour
{
    [SerializeField] Transform choiceTransform;
    [SerializeField] GameObject profilePicObj;
    [SerializeField] GameObject GUI;
    [SerializeField] Image profilePic;
    [SerializeField] TMP_Text buttonOneText;
    [SerializeField] TMP_Text buttonTwoText;
    [SerializeField] TMP_Text textBox;
    [SerializeField] TMP_Text nameBox;
    [SerializeField] Vector2 choiceOffsetFromPlayer = new Vector2(0, 0.5f);

    GameObject playerObj;

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        profilePicObj.SetActive(false);
    }

    public void SetDialogue(string nameText, string dialogueText)
    {
        textBox.fontStyle = TMPro.FontStyles.Normal;
        nameBox.text = nameText;
        textBox.text = dialogueText;

        choiceTransform.gameObject.SetActive(false);
    }
    public void SetDialogue(string nameText, string dialogueText, string button1, string button2)
    {
        textBox.fontStyle = TMPro.FontStyles.Normal;
        nameBox.text = nameText;
        textBox.text = dialogueText;

        choiceTransform.gameObject.SetActive(true);
        buttonOneText.text = button1;
        buttonTwoText.text = button2;

        Vector3 worldPos = playerObj.transform.position + (Vector3)choiceOffsetFromPlayer;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        choiceTransform.position = screenPos;
    }

    public void ToggleButtons(bool isButtonsOn)
    {
        buttonOneText.gameObject.SetActive(isButtonsOn);
        buttonTwoText.gameObject.SetActive(isButtonsOn);
    }

    public void ToggleGUI(bool isGUIOn)
    {
        GUI.SetActive(isGUIOn);
    }

    public void SetProfilePic(Sprite picture)
    {
        if(picture != null)
        {
            profilePicObj.SetActive(true);
            profilePic.sprite = picture;
        }
        else
        {
            profilePicObj.SetActive(false);
        }
    }
}
