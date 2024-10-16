using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Color = UnityEngine.Color;

public class DialogueManager : MonoBehaviour
{
    [Header("Plug-in Variables")]
    [SerializeField] Dialogue.Dialog cutscene;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] NewAudioManager AudioManager;
    [SerializeField] PlayerAudio playerAudio;
    [SerializeField] Image ProfilePic;
    [SerializeField] Image WhiteFade;
    [SerializeField] List<Sprite> CharacterPics;

    GameObject canvas;
    GameObject uiCanvas;
    GameObject dialogUI;
    TMPro.TMP_Text textBox;
    TMPro.TMP_Text nameBox;
    TMPro.TMP_Text buttonOneText;
    TMPro.TMP_Text buttonTwoText;
    Dialogue dialogue = new Dialogue();
    QuestSystem questSystem;

    bool skipCutscene = false;

    string[] lines;

    bool inCutscene = false;

    int currentLine = 0;

    [SerializeField] float skipCooldown = 5f;
    float skipTimer = 5f;
    [SerializeField] bool InstaSkip;

    [SerializeField] Dictionary<string, string> emotions = new Dictionary<string, string>();

    [Header("AI Variables")]
    #region AIVariables
    AI characterAI;
    Vector2 targetPosition;
    bool characterMoving = false;
    Rigidbody2D characterRB2D;
    Animator characterAnimator;
    #endregion

    public enum PlayerChoice
    {
        None, ChoiceOne, ChoiceTwo,
    }

    public enum CutsceneState
    {
        None, Continue, Skipping, Moving, Picking, Waiting
    }

    PlayerChoice currentChoice = PlayerChoice.None;
    PlayerChoice lastChoice = PlayerChoice.None;

    CutsceneState currentState = CutsceneState.Continue;

    private void Awake()
    {
        //PlayerMovement movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        canvas = GameObject.Find("CanvasDialogue");
        dialogUI = GameObject.Find("DialogeBG");
        uiCanvas = GameObject.Find("Player UI");
        textBox = GameObject.Find("TextBox").GetComponent<TMPro.TMP_Text>();
        nameBox = GameObject.Find("NameBox").GetComponent<TMPro.TMP_Text>();
        buttonOneText = GameObject.Find("ButtonOneText").GetComponent<TMPro.TMP_Text>();
        buttonTwoText = GameObject.Find("ButtonTwoText").GetComponent<TMPro.TMP_Text>();
        questSystem = FindObjectOfType<QuestSystem>();
        ButtonSwitch(false);
        CanvasSwitch(false);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetCutscene(Dialogue.Dialog.VarrenEncounter);
    }

    void StartCutScene(string dialogueName, int line)
    {
        movement.StartCutscene();
        CanvasSwitch(true);
        uiCanvas.SetActive(false);
        lines = dialogue.getDialogue(dialogueName);
        currentLine = line;
        inCutscene = true;
        OnDialogueUpdate();
    }

    // Update is called once per frame
    void Update()
    {

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && inCutscene && currentState == CutsceneState.Continue)
        {
            OnDialogueUpdate();
        }
        else if (currentState == CutsceneState.Moving)
        {
            MoveCharacter();
        }
        else if (currentState == CutsceneState.Skipping)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && inCutscene)
            {
                OnDialogueUpdate();
            }
            if (InstaSkip)
            {
                OnDialogueUpdate();
            }
            else
            {
                skipTimer = skipTimer - Time.deltaTime;
                if (skipTimer < 0)
                {
                    OnDialogueUpdate();
                    skipTimer = skipCooldown;
                }
            }
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
            #region Choice Functions
            else if (lines[currentLine].EndsWith("$Prompt"))
            {
                currentState = CutsceneState.Picking;
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
                lastChoice = PlayerChoice.ChoiceOne;
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
                lastChoice = PlayerChoice.ChoiceOne;
                while (lines[currentLine].Contains("$OptionB"))
                {
                    currentLine++;
                }
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].EndsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                lastChoice = PlayerChoice.ChoiceTwo;
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
                lastChoice = PlayerChoice.ChoiceTwo;
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
                currentState = CutsceneState.Waiting;
                int moveStartIndex = lines[currentLine].IndexOf('(');
                int moveEndIndex = lines[currentLine].IndexOf(')');
                string moveInfo = lines[currentLine].Substring(moveStartIndex + 1, moveEndIndex - moveStartIndex - 1);
                string[] splitString = moveInfo.Split(',');
                Vector3 target = new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
                playerAudio.NextLine(currentLine);
                if (splitString[0] == "Main Camera")
                {
                    Transform cameraTransform = GameObject.Find(splitString[0]).transform;
                    target.z = cameraTransform.position.z;
                    StartCoroutine(MoveCamera(.05f, cameraTransform, target));
                }
                else
                {
                    SetMoveCharacterInfo(splitString[0], 3, target);
                }
            }
            else if (lines[currentLine].EndsWith("$Tele") || lines[currentLine].StartsWith("$Tele"))
            {
                int teleStartIndex = lines[currentLine].IndexOf('(');
                int teleEndIndex = lines[currentLine].IndexOf(')');
                string teleInfo = lines[currentLine].Substring(teleStartIndex + 1, teleEndIndex - teleStartIndex - 1);
                string[] splitString = teleInfo.Split(',');
                Vector2 target = new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
                playerAudio.NextLine(currentLine);
                GameObject.Find(splitString[0]).transform.position = target;
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].EndsWith("$Kill") || lines[currentLine].StartsWith("$Kill"))
            {
                int killStartIndex = lines[currentLine].IndexOf('(');
                int killEndIndex = lines[currentLine].IndexOf(')');
                string killInfo = lines[currentLine].Substring(killStartIndex + 1, killEndIndex - killStartIndex - 1);
                Destroy(GameObject.Find(killInfo));
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].EndsWith("$AfterImage") || lines[currentLine].StartsWith("$AfterImage"))
            {
                int imageStartIndex = lines[currentLine].IndexOf('(');
                int imageEndIndex = lines[currentLine].IndexOf(')');
                string imageInfo = lines[currentLine].Substring(imageStartIndex + 1, imageEndIndex - imageStartIndex - 1);
                string[] splitString = imageInfo.Split(",");
                GameObject character = GameObject.Find(splitString[0]);
                if (bool.Parse(splitString[1]) == true)
                {
                    character.GetComponent<AfterImage>().StartEffect();
                }
                else
                {
                    character.GetComponent<AfterImage>().StopEffect();
                }
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if(lines[currentLine].EndsWith("$Animate") || lines[currentLine].StartsWith("$Animate"))
            {
                int animateStartIndex = lines[currentLine].IndexOf('(');
                int animateEndIndex = lines[currentLine].IndexOf(')');
                string aniamateInfo = lines[currentLine].Substring(animateStartIndex + 1, animateEndIndex - animateStartIndex - 1);
                string[] splitString = aniamateInfo.Split(",");
                Animator animator = GameObject.Find(splitString[0]).GetComponent<Animator>();
                if(splitString.Length > 2)
                {
                    animator.SetBool(splitString[1], bool.Parse(splitString[2]));
                }
                else
                {
                    animator.SetTrigger(splitString[1]);
                }
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].EndsWith("$Timer") || lines[currentLine].StartsWith("$Timer"))
            {
                currentState = CutsceneState.Waiting;
                int timerStartIndex = lines[currentLine].IndexOf('(');
                int timerEndIndex = lines[currentLine].IndexOf(')');
                string timerInfo = lines[currentLine].Substring(timerStartIndex + 1, timerEndIndex - timerStartIndex - 1);
                StartCoroutine(Timer(float.Parse(timerInfo)));
            }
            else if (lines[currentLine].EndsWith("$FadeIn") || lines[currentLine].StartsWith("$FadeIn"))
            {
                Debug.Log("Fade In Test");
                currentState = CutsceneState.Waiting;
                int fadeInStartIndex = lines[currentLine].IndexOf('(');
                int fadeInEndIndex = lines[currentLine].IndexOf(')');
                string fadeInInfo = lines[currentLine].Substring(fadeInStartIndex + 1, fadeInEndIndex - fadeInStartIndex - 1);
                string[] splitString = fadeInInfo.Split(",");
                StartCoroutine(FadeIn(Int32.Parse(splitString[0]), Int32.Parse(splitString[1]), Int32.Parse(splitString[2])));
            }
            else if (lines[currentLine].EndsWith("$FadeOut") || lines[currentLine].StartsWith("$FadeOut"))
            {
                Debug.Log("Fade Out Test");
                currentState = CutsceneState.Waiting;
                int fadeOutStartIndex = lines[currentLine].IndexOf('(');
                int fadeOutEndIndex = lines[currentLine].IndexOf(')');
                string fadeOutInfo = lines[currentLine].Substring(fadeOutStartIndex + 1, fadeOutEndIndex - fadeOutStartIndex - 1);
                string[] splitString = fadeOutInfo.Split(",");
                StartCoroutine(FadeOut(Int32.Parse(splitString[0]), Int32.Parse(splitString[1]), Int32.Parse(splitString[2])));
            }
            #endregion
            #region Invetory Functions
            else if (lines[currentLine].EndsWith("$GiveWeapon") || lines[currentLine].StartsWith("$GiveWeapon"))
            {

                WeaponInfo desiredWeaponInfo = null;

                int giveStartIndex = lines[currentLine].IndexOf('(');
                int giveEndIndex = lines[currentLine].IndexOf(')');
                string giveInfo = lines[currentLine].Substring(giveStartIndex + 1, giveEndIndex - giveStartIndex - 1);

                foreach (WeaponInfo weapon in UsableDatabase.weaponInfos)
                {
                    if (weapon.GetName() == giveInfo)
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
                ItemInfo desiredItemInfo = null;

                int giveStartIndex = lines[currentLine].IndexOf('(');
                int giveEndIndex = lines[currentLine].IndexOf(')');
                string giveInfo = lines[currentLine].Substring(giveStartIndex + 1, giveEndIndex - giveStartIndex - 1);
                string[] splitString = giveInfo.Split(',');

                foreach (ItemInfo item in UsableDatabase.itemInfos)
                {
                    if (item.GetName() == splitString[0])
                    {
                        desiredItemInfo = item;
                        break;
                    }
                }
                if (desiredItemInfo != null)
                {
                    Item desiredItem = new Item(desiredItemInfo, Int32.Parse(splitString[1]));
                    inventory.AddItem(desiredItem);
                }
            }
            else if (lines[currentLine].EndsWith("$GiveAbility") || lines[currentLine].StartsWith("$GiveAbility"))
            {
                AbilityInfo desiredAbilityInfo = null;

                int giveStartIndex = lines[currentLine].IndexOf('(');
                int giveEndIndex = lines[currentLine].IndexOf(')');
                string giveInfo = lines[currentLine].Substring(giveStartIndex + 1, giveEndIndex - giveStartIndex - 1);
                string[] splitString = giveInfo.Split(',');

                foreach (AbilityInfo ability in UsableDatabase.abilityInfos)
                {
                    if (ability.GetName() == splitString[0])
                    {
                        desiredAbilityInfo = ability;
                        break;
                    }
                }
                if (desiredAbilityInfo != null)
                {
                    Ability desiredAbility = new Ability(desiredAbilityInfo);
                    inventory.AddPlayerAbility(desiredAbility);
                }
            }
            #endregion
            #region Quest Functions
            else if (lines[currentLine].EndsWith("$SetQuest") || lines[currentLine].StartsWith("$SetQuest"))
            {
                int moveStartIndex = lines[currentLine].IndexOf('(');
                int moveEndIndex = lines[currentLine].IndexOf(')');
                string questInfo = lines[currentLine].Substring(moveStartIndex + 1, moveEndIndex - moveStartIndex - 1);
                string[] splitString = questInfo.Split(',');
                questSystem.SetQuest(Int32.Parse(splitString[0]), Int32.Parse(splitString[1]));
            }
            #endregion
            #region UI Functions
            else if (lines[currentLine].StartsWith("$Emote") || lines[currentLine].EndsWith("$Emote"))
            {
                try
                {
                    emotions.Add(lines[currentLine].Split("|")[1], lines[currentLine].Split("|")[2]);
                }
                catch(ArgumentException)
                {
                    emotions.Remove(lines[currentLine].Split("|")[1]);
                    emotions.Add(lines[currentLine].Split("|")[1], lines[currentLine].Split("|")[2]);
                }
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
            }
            else if (lines[currentLine].StartsWith("$ToggleUI") || lines[currentLine].EndsWith("$ToggleUI"))
            {
                int toggleStartIndex = lines[currentLine].IndexOf('(');
                int toggleEndIndex = lines[currentLine].IndexOf(')');
                string toggleInfo = lines[currentLine].Substring(toggleStartIndex + 1, toggleEndIndex - toggleStartIndex - 1);

                UISwitch(bool.Parse(toggleInfo));
                currentLine++;
                OnDialogueUpdate();
                currentLine--;
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
        else if (currentLine > lines.Length-1)
        {
            CanvasSwitch(false);
            uiCanvas.SetActive(true);
            ResetCamera();
            movement.EndCutscene();
            inCutscene = false;
            InstaSkip = false;
            currentLine = 0;
        }
        currentLine++;
    }

    #region Cutscene Visual Effects
    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        currentState = CutsceneState.Continue;
        playerAudio.NextLine(currentLine);
        OnDialogueUpdate();
    }

    IEnumerator FadeIn(float r, float g, float b)
    {
        Debug.Log("Test FadeIn");
        WhiteFade.gameObject.SetActive(true);
        r /= 255f;
        g /= 255f;
        b /= 255f;
        Color color = WhiteFade.color;
        while (WhiteFade.color.a < 1f)
        {
            yield return new WaitForSeconds(.1f);
            color.r = r; color.g = g; color.b = b;
            color.a += .1f;
            WhiteFade.color = color;
        }
        color.r = r; color.g = g; color.b = b;
        color.a = 1f;
        WhiteFade.color = color;
        currentState = CutsceneState.Continue;
        yield return new WaitForSeconds(.3f);
        OnDialogueUpdate();
    }

    IEnumerator FadeOut(float r, float g, float b)
    {
        Debug.Log("Test Fadeout");
        r /= 255f;
        g /= 255f;
        b /= 255f;
        Color color = WhiteFade.color;
        while (WhiteFade.color.a > 0f)
        {
            yield return new WaitForSeconds(.1f);
            color.r = r; color.g = g; color.b = b;
            color.a -= .1f;
            WhiteFade.color = color;
        }
        color.r = r; color.g = g; color.b = b;
        color.a = 0f;
        WhiteFade.color = color;
        WhiteFade.gameObject.SetActive(false);
        currentState = CutsceneState.Continue;
        OnDialogueUpdate();
    }
    #endregion

    #region CutsceneMovement
    IEnumerator MoveCamera(float time, Transform cameraTransform, Vector3 target)
    {
        
        float interpol = 0;
        while (cameraTransform.localPosition != target)
        {
            Debug.Log($"{interpol} | Camera:{cameraTransform.localPosition} | target: {target}");
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, target, interpol);
            
            interpol += 0.01f;
            yield return new WaitForSeconds(time);
        }
        playerAudio.NextLine(currentLine);
        OnDialogueUpdate();
        currentState = CutsceneState.Continue;
    }

    void ResetCamera()
    {
        GameObject.Find("Main Camera").transform.localPosition = new Vector3(0, 0, -7.5f);
    }

    void SetMoveCharacterInfo(string name, float speed, Vector2 target)
    {
        GameObject character = GameObject.Find(name);
        targetPosition = target;
        characterAI = character.GetComponent<AI>();
        characterRB2D = character.GetComponent<Rigidbody2D>();
        characterAnimator = character.GetComponent<Animator>();
        characterAI.SetSpeed(speed);
        characterAI.SetCanMove(true);
        if(character.name == "Player")
        {
            characterAI.enabled = true;
        }
        characterMoving = true;
    }

    void MoveCharacter()
    {
        if (!characterAI.MoveTowardsTarget(targetPosition))
        {
            characterAI.SetCanMove(false);
            characterAI.SetTarget(null);
            characterAnimator.SetBool("isWalking", false);
            if(characterAI.gameObject.name == "Player")
            {
                characterAI.enabled=false;
            }
            OnDialogueUpdate();
            currentState = CutsceneState.Continue;
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
        currentState = CutsceneState.Continue;
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
        currentState = CutsceneState.Continue;
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
    void UISwitch(bool isUIOn)
    {
        dialogUI.SetActive(isUIOn);
    }

    void UpdateImage()
    {
        string myEmote = "";

        emotions.TryGetValue(nameBox.text, out myEmote);

        if (myEmote == null) { myEmote = "Neutral"; }

        myEmote = nameBox.text + myEmote;


        foreach (Sprite sprite in CharacterPics)
        {
            if (sprite.name == myEmote)
            {
                ProfilePic.sprite = sprite;
            }
        }
    }
    #endregion]

    #region Public Methods
    public PlayerChoice GetPlayerChoice()
    {
        return currentChoice;
    }

    public PlayerChoice GetLastPlayerChoice()
    {
        return lastChoice;
    }
    
    public void SetCutscene(Dialogue.Dialog newDialog)
    {
        cutscene = newDialog;
        StartCutScene(cutscene.ToString(), 0);
    }

    public void SkipCutscene()
    {
        currentState = CutsceneState.Skipping;
    }

    public void DevSkipCutscene()
    {
        currentState = CutsceneState.Skipping;
        InstaSkip = true;
    }

    public bool getInCutscene()
    {
        return inCutscene;
    }
    #endregion
}




