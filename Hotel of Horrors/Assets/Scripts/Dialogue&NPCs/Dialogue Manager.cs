using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] Dialogue.Dialog cutscene;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] NewAudioManager AudioManager;
    [SerializeField] PlayerAudio playerAudio;
    [SerializeField] Image ProfilePic;
    [SerializeField] List<Sprite> CharacterPics;

    GameObject canvas;
    TMPro.TMP_Text textBox;
    TMPro.TMP_Text nameBox;
    TMPro.TMP_Text buttonOneText;
    TMPro.TMP_Text buttonTwoText;
    Dialogue dialogue = new Dialogue();

    string[] lines;

    bool inCutscene = false;
    bool isPicking = false;
    
    int currentLine = 0;

    #region AIVariables
    [SerializeField] AI characterAI;
    Vector2 targetPosition;
    bool characterMoving = false;
    Rigidbody2D characterRB2D;
    Animator characterAnimator;
    #endregion

    public enum PlayerChoice
    {
        None, ChoiceOne, ChoiceTwo,
    }

    PlayerChoice currentChoice = PlayerChoice.None;

    private void Awake()
    {
        //PlayerMovement movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        canvas = GameObject.Find("CanvasDialogue");
        textBox = GameObject.Find("TextBox").GetComponent<TMPro.TMP_Text>();
        nameBox = GameObject.Find("NameBox").GetComponent<TMPro.TMP_Text>();
        buttonOneText = GameObject.Find("ButtonOneText").GetComponent<TMPro.TMP_Text>();
        buttonTwoText = GameObject.Find("ButtonTwoText").GetComponent<TMPro.TMP_Text>();
        ButtonSwitch(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCutscene(Dialogue.Dialog.VarrenEncounter);
    }

    void StartCutScene(string dialogueName, int line)
    {
        movement.TogglePlayerControls(false);
        CanvasSwitch(true);
        lines = dialogue.getDialogue(dialogueName);
        currentLine = line;
        inCutscene = true;
        OnDialogueUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && inCutscene && !isPicking && !characterMoving)
        {
            OnDialogueUpdate();
        }
        else if (characterMoving)
        {
            MoveCharacter();
        }
    }

    void OnDialogueUpdate()
    {
        //While their are still lines of dialog left to read.
        if (currentLine < lines.Length)
        {
            //Parse the line for relevent information and do corresponding actions
            #region Audio Functions
            if (lines[currentLine].StartsWith("$PlayEffect"))
            {
                string[] sound = lines[currentLine].Split("|");
                AudioManager.PlayEffect(sound[1]);
                currentLine++;
                OnDialogueUpdate();
                currentLine--;

            }
            else if (lines[currentLine].StartsWith("$PlaySong"))
            {
                string[] sound = lines[currentLine].Split("|");
                AudioManager.PlaySong(sound[1]);
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].StartsWith("$Pause"))
            {
                AudioManager.Pause();
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].StartsWith("$Resume"))
            {
                AudioManager.Play();
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }

            #endregion
            #region ChoiceDialog
            else if (lines[currentLine].EndsWith("$Prompt"))
            {
                isPicking = true;
                ButtonSwitch(true);
                string startString = lines[currentLine].Replace("$Prompt", "");
                string[] splitString = startString.Split('|');
                string promptText = splitString[0];
                string bOneText = splitString[1];
                string bTwoText = splitString[2];
                string outputName = promptText.Split(':')[0];
                string outputText = promptText.Split(":")[1];
                textBox.text = outputText;
                nameBox.text = outputName;
                buttonOneText.text = bOneText;
                buttonTwoText.text = bTwoText;

                playerAudio.NextLine(currentLine);
                UpdateImage();

            }
            else if (lines[currentLine].EndsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceOne)
            {
                string optionAText = lines[currentLine].Replace("$OptionA", "");
                string outputText = optionAText.Split(":")[1];
                string outputName = optionAText.Split(':')[0];
                textBox.text = outputText;
                nameBox.text = outputName;

                playerAudio.NextLine(currentLine);
                UpdateImage();
            }
            else if (lines[currentLine].EndsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceOne)
            {
                while (lines[currentLine].Contains("$OptionB"))
                {
                    currentLine++;
                }
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].EndsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                string optionBText = lines[currentLine].Replace("$OptionB", "");
                string outputText = optionBText.Split(":")[1];
                string optionBName = optionBText.Split(':')[0];
                textBox.text = outputText;
                nameBox.text = optionBName;

                playerAudio.NextLine(currentLine);
                UpdateImage();
            }
            else if (lines[currentLine].EndsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                while (lines[currentLine].Contains("$OptionA"))
                {
                    currentLine++;
                }
                OnDialogueUpdate();
                currentLine--;
            }
            #endregion
            #region Cutscene Functions
            else if (lines[currentLine].EndsWith("$Move") || lines[currentLine].StartsWith("$Move"))
            {
                int moveStartIndex = lines[currentLine].IndexOf('(');
                int moveEndIndex = lines[currentLine].IndexOf(')');
                string moveInfo = lines[currentLine].Substring(moveStartIndex + 1, moveEndIndex - moveStartIndex - 1);
                string[] splitString = moveInfo.Split(',');
                Vector2 target = new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
                SetMoveCharacterInfo(splitString[0], 3, target);
            }
            else if (lines[currentLine].EndsWith("$Kill") || lines[currentLine].StartsWith("$Kill"))
            {
                int killStartIndex = lines[currentLine].IndexOf('(');
                int killEndIndex = lines[currentLine].IndexOf(')');
                string killInfo = lines[currentLine].Substring(killStartIndex + 1, killEndIndex - killStartIndex - 1);
                Destroy(GameObject.Find(killInfo));

            }
            #endregion
            //We gonna get rid of this is no use is found this week ;/
            else if (lines[currentLine].EndsWith("$Spin") || lines[currentLine].StartsWith("$Spin"))
            {

            }
            #region Invetory Functions
            else if (lines[currentLine].EndsWith("$GiveWeapon") || lines[currentLine].StartsWith("$GiveWeapon"))
            {
                WeaponInfo desiredWeaponInfo = new WeaponInfo();

                int giveStartIndex = lines[currentLine].IndexOf('(');
                int giveEndIndex = lines[currentLine].IndexOf(')');
                string giveInfo = lines[currentLine].Substring(giveStartIndex + 1, giveEndIndex - giveStartIndex - 1);

                foreach (WeaponInfo weapon in UsableDatabase.weaponInfos)
                {
                    if(weapon.GetName() == giveInfo)
                    {
                        desiredWeaponInfo = weapon;
                        break;
                    }
                }
                if (desiredWeaponInfo != null)
                {
                    Weapon desiredWeapon = new Weapon(desiredWeaponInfo);
                    inventory.AddWeapon(desiredWeapon);
                }
            }
            else if (lines[currentLine].EndsWith("$GiveItem") || lines[currentLine].StartsWith("$GiveItem"))
            {
                ItemInfo desiredItemInfo = new ItemInfo();

                int giveStartIndex = lines[currentLine].IndexOf('(');
                int giveEndIndex = lines[currentLine].IndexOf(')');
                string giveInfo = lines[currentLine].Substring(giveStartIndex + 1, giveEndIndex - giveStartIndex - 1);
                string[] splitString = giveInfo.Split(',');

                foreach (ItemInfo weapon in UsableDatabase.itemInfos)
                {
                    if (weapon.GetName() == splitString[0])
                    {
                        desiredItemInfo = weapon;
                        break;
                    }
                }
                if (desiredItemInfo != null)
                {
                    Item desiredItem = new Item(desiredItemInfo, Int32.Parse(splitString[1]));
                    inventory.AddItem(desiredItem);
                }
            }
            #endregion
            
            else
            {
                textBox.fontStyle = TMPro.FontStyles.Normal;
                currentChoice = PlayerChoice.None;
                nameBox.text = lines[currentLine].Split(':')[0];
                textBox.text = lines[currentLine].Split(':')[1];

                playerAudio.NextLine(currentLine);
                UpdateImage();
            }
        }
        //When there are no more lines of dialog to read.
        else if(currentLine > lines.Length)
        {
            CanvasSwitch(false);
            movement.TogglePlayerControls(true);
            inCutscene = false;
        }
        currentLine++;
    }

    #region CutsceneMovement
    void SetMoveCharacterInfo(string name, float speed, Vector2 target)
    {
        GameObject character = GameObject.Find(name);
        targetPosition = target;
        characterAI = character.GetComponent<AI>();
        characterRB2D = character.GetComponent<Rigidbody2D>();
        characterAnimator = character.GetComponent<Animator>();
        characterAI.SetSpeed(speed);
        characterAI.SetCanMove(true);
        characterMoving = true;
    }

    void MoveCharacter()
    {
        if (!characterAI.MoveTowardsTarget(targetPosition))
        {
            characterAI.SetCanMove(false);
            characterAI.SetTarget(null);
            characterMoving = false;
            characterAnimator.SetBool("isWalking", false);
            OnDialogueUpdate();
        }
        else
        {
            characterAnimator.SetBool("isWalking", true);
            characterAnimator.SetFloat("x", characterRB2D.velocity.x);
            characterAnimator.SetFloat("y", characterRB2D.velocity.y);
        }
    }
    #endregion

    #region GUI Behavior
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

    void CanvasSwitch(bool isCanvasOn)
    {
        canvas.SetActive(isCanvasOn);
    }

    void UpdateImage()
    {
        foreach (Sprite sprite in CharacterPics)
        {
            if (sprite.name == nameBox.text)
            {
                ProfilePic.sprite = sprite;
            }
        }
    }
    #endregion]

    public PlayerChoice GetPlayerChoice()
    {
        return currentChoice;
    }

    public void SetCutscene(Dialogue.Dialog newDialog)
    {
        cutscene = newDialog;
        StartCutScene(cutscene.ToString(), 0);
    }
}




