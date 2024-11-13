using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class DialogueManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Dialogue.Dialog cutscene;
    [Space(2)]
    [Header("Functionality Plug-Ins")]
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] NewAudioManager AudioManager;
    [SerializeField] PlayerAudio playerAudio;
    [SerializeField] Image WhiteFade;
    [SerializeField] List<Sprite> CharacterPics;
    [Header("Visual Plug-Ins")]
    [SerializeField] GameObject playerGUI;
    [SerializeField] DialogueGUI dialogueGUI;

    Dialogue dialogue = new Dialogue();
    QuestSystem questSystem;

    string[] lines;

    int currentLine = 0;

    [SerializeField] float skipCooldown = 3f;
    float skipTimer = 5f;
    [SerializeField] bool instaSkip;
    [SerializeField] bool auto;

    [SerializeField] Dictionary<string, string> emotions = new Dictionary<string, string>();

    [Header("AI Variables")]
    #region AIVariables
    AI characterAI;
    Vector2 targetPosition;
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

    CutsceneState currentState = CutsceneState.None;

    private void Awake()
    {
        questSystem = FindObjectOfType<QuestSystem>();
        if (dialogueGUI)
        {
            dialogueGUI.ToggleButtons(false);
            dialogueGUI.ToggleGUI(false);
        }
    }

    void StartCutScene(string dialogueName, int line)
    {
        currentState = CutsceneState.Continue;
        movement.StartCutscene();
        lines = dialogue.getDialogue(dialogueName);
        currentLine = line;
        playerGUI.SetActive(false);

        auto = false;
        instaSkip = false;
        skipTimer = skipCooldown;

        if (dialogueGUI)
        {
            dialogueGUI.ToggleGUI(false);
            dialogueGUI.SetDialogue("", "");
        }
        OnDialogueUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)
            || (auto && skipTimer < 0) || instaSkip) &&
            currentState == CutsceneState.Continue)
        {
            skipTimer = skipCooldown;

            OnDialogueUpdate();
        }
        else if (currentState == CutsceneState.Moving)
        {
            MoveCharacter();
        }

        skipTimer -= Time.deltaTime;
    }

    void OnDialogueUpdate()
    {
        //While their are still lines of dialog left to read.
        if (currentLine < lines.Length)
        {
            if (lines[currentLine].EndsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceOne)
            {
                currentState = CutsceneState.Continue;

                while (currentLine < lines.Length && lines[currentLine].Contains("$OptionB"))
                {
                    //print("Skipping lines with OptionB");
                    currentLine++;
                }

                //Auto move to the next line
                //OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                currentState = CutsceneState.Continue;

                while (currentLine < lines.Length && lines[currentLine].Contains("$OptionA"))
                {
                    //print("Skipping lines with OptionA");
                    currentLine++;
                }

                //Auto move to the next line
                //OnDialogueUpdate();
            }

            string currentLineString = lines[currentLine];

            if (lines[currentLine].EndsWith("$OptionA") && currentChoice == PlayerChoice.ChoiceOne)
            {
                currentState = CutsceneState.Continue;

                currentLineString = lines[currentLine].Replace("$OptionA", "");
                //print(currentLineString);

                /*string outputText = optionAText.Split(":")[1];
                string outputName = optionAText.Split(':')[0];
                if (dialogueGUI)
                {
                    dialogueGUI.SetDialogue(outputName, outputText);
                }
                playerAudio.NextLine(currentLine);

                currentLine++;*/
            }
            else if (lines[currentLine].EndsWith("$OptionB") && currentChoice == PlayerChoice.ChoiceTwo)
            {
                currentState = CutsceneState.Continue;

                currentLineString = lines[currentLine].Replace("$OptionB", "");
                //print(currentLineString);

                /*string outputText = optionBText.Split(":")[1];
                string optionBName = optionBText.Split(':')[0];
                if (dialogueGUI)
                {
                    dialogueGUI.SetDialogue(optionBName, outputText);
                }

                playerAudio.NextLine(currentLine);

                currentLine++;*/
            }


            //Parse the line for relevent information and do corresponding actions
            #region Audio Functions
            if (lines[currentLine].StartsWith("$PlayEffect"))
            {
                currentState = CutsceneState.Waiting;

                string[] sound = lines[currentLine].Split("|");
                AudioManager.PlayEffect(sound[1]);

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();

            }
            else if (lines[currentLine].StartsWith("$PlaySong"))
            {
                currentState = CutsceneState.Waiting;

                string[] sound = lines[currentLine].Split("|");
                AudioManager.PlaySong(sound[1]);

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].StartsWith("$Pause"))
            {
                currentState = CutsceneState.Waiting;

                AudioManager.Pause();

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].StartsWith("$Resume"))
            {
                currentState = CutsceneState.Waiting;

                AudioManager.Play();

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }

            #endregion
            #region Choice Functions
            if (lines[currentLine].EndsWith("$Prompt"))
            {
                currentState = CutsceneState.Picking;

                string startString = lines[currentLine].Replace("$Prompt", "");
                string[] splitString = startString.Split('|');
                string promptText = splitString[0];
                string bOneText = splitString[1];
                string bTwoText = splitString[2];
                string outputName = promptText.Split(':')[0];
                string outputText = promptText.Split(":")[1];
                if (dialogueGUI)
                {
                    dialogueGUI.ToggleButtons(true);
                    dialogueGUI.ToggleGUI(true);
                    dialogueGUI.SetDialogue(outputName, outputText, bOneText, bTwoText);
                }

                playerAudio.NextLine(currentLine);

                currentLine++;
            }
            #endregion
            #region Cutscene Functions
            else if (lines[currentLine].EndsWith("$MoveViaLerp") || lines[currentLine].StartsWith("$MoveViaLerp"))
            {
                currentState = CutsceneState.Waiting;
                int moveStartIndex = lines[currentLine].IndexOf('(');
                int moveEndIndex = lines[currentLine].IndexOf(')');
                string moveInfo = lines[currentLine].Substring(moveStartIndex + 1, moveEndIndex - moveStartIndex - 1);
                string[] splitString = moveInfo.Split(',');

                Vector3 target = new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
                Transform objTransform = GameObject.Find(splitString[0]).transform;
                target.z = objTransform.position.z;
                StartCoroutine(MoveViaLerp(.05f, objTransform, target));
            }
            else if (lines[currentLine].EndsWith("$Move") || lines[currentLine].StartsWith("$Move"))
            {
                currentState = CutsceneState.Moving;
                int moveStartIndex = lines[currentLine].IndexOf('(');
                int moveEndIndex = lines[currentLine].IndexOf(')');
                string moveInfo = lines[currentLine].Substring(moveStartIndex + 1, moveEndIndex - moveStartIndex - 1);
                string[] splitString = moveInfo.Split(',');

                //print("Start character move for cutscene");
                Vector2 target = new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
                SetMoveCharacterInfo(splitString[0], 2, target);
            }
            else if (lines[currentLine].EndsWith("$Tele") || lines[currentLine].StartsWith("$Tele"))
            {
                currentState = CutsceneState.Waiting;
                int teleStartIndex = lines[currentLine].IndexOf('(');
                int teleEndIndex = lines[currentLine].IndexOf(')');
                string teleInfo = lines[currentLine].Substring(teleStartIndex + 1, teleEndIndex - teleStartIndex - 1);
                string[] splitString = teleInfo.Split(',');
                Vector2 target = new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
                GameObject.Find(splitString[0]).transform.position = target;

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$Kill") || lines[currentLine].StartsWith("$Kill"))
            {
                currentState = CutsceneState.Waiting;
                int killStartIndex = lines[currentLine].IndexOf('(');
                int killEndIndex = lines[currentLine].IndexOf(')');
                string killInfo = lines[currentLine].Substring(killStartIndex + 1, killEndIndex - killStartIndex - 1);
                Destroy(GameObject.Find(killInfo));

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$AfterImage") || lines[currentLine].StartsWith("$AfterImage"))
            {
                currentState = CutsceneState.Waiting;
                int imageStartIndex = lines[currentLine].IndexOf('(');
                int imageEndIndex = lines[currentLine].IndexOf(')');
                string imageInfo = lines[currentLine].Substring(imageStartIndex + 1, imageEndIndex - imageStartIndex - 1);
                string[] splitString = imageInfo.Split(",");
                GameObject character = GameObject.Find(splitString[0]);
                if (bool.Parse(splitString[1]) == true)
                {
                    character.transform.GetChild(0).GetComponent<AfterImage>().StartEffect();
                }
                else
                {
                    character.transform.GetChild(0).GetComponent<AfterImage>().StopEffect();
                }

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$BossPoof") || lines[currentLine].StartsWith("$BossPoof"))
            {
                currentState = CutsceneState.Waiting;
                int poofStartIndex = lines[currentLine].IndexOf('(');
                int poofEndIndex = lines[currentLine].IndexOf(')');
                string poofInfo = lines[currentLine].Substring(poofStartIndex + 1, poofEndIndex - poofStartIndex - 1);
                GameObject character = GameObject.Find(poofInfo);
                character.GetComponent<BossHelper>().BossDeathEffects();

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$Animate") || lines[currentLine].StartsWith("$Animate"))
            {
                currentState = CutsceneState.Waiting;
                int animateStartIndex = lines[currentLine].IndexOf('(');
                int animateEndIndex = lines[currentLine].IndexOf(')');
                string aniamateInfo = lines[currentLine].Substring(animateStartIndex + 1, animateEndIndex - animateStartIndex - 1);
                string[] splitString = aniamateInfo.Split(",");
                Animator animator = GameObject.Find(splitString[0]).GetComponent<Animator>();
                if (splitString.Length > 2)
                {
                    animator.SetBool(splitString[1], bool.Parse(splitString[2]));
                }
                else
                {
                    animator.SetTrigger(splitString[1]);
                }

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
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
                //Debug.Log("Fade In Test");
                currentState = CutsceneState.Waiting;
                int fadeInStartIndex = lines[currentLine].IndexOf('(');
                int fadeInEndIndex = lines[currentLine].IndexOf(')');
                string fadeInInfo = lines[currentLine].Substring(fadeInStartIndex + 1, fadeInEndIndex - fadeInStartIndex - 1);
                string[] splitString = fadeInInfo.Split(",");
                StartCoroutine(FadeIn(Int32.Parse(splitString[0]), Int32.Parse(splitString[1]), Int32.Parse(splitString[2])));
            }
            else if (lines[currentLine].EndsWith("$FadeOut") || lines[currentLine].StartsWith("$FadeOut"))
            {
                //Debug.Log("Fade Out Test");
                currentState = CutsceneState.Waiting;
                int fadeOutStartIndex = lines[currentLine].IndexOf('(');
                int fadeOutEndIndex = lines[currentLine].IndexOf(')');
                string fadeOutInfo = lines[currentLine].Substring(fadeOutStartIndex + 1, fadeOutEndIndex - fadeOutStartIndex - 1);
                string[] splitString = fadeOutInfo.Split(",");
                StartCoroutine(FadeOut(Int32.Parse(splitString[0]), Int32.Parse(splitString[1]), Int32.Parse(splitString[2])));
            }
            else if (lines[currentLine].EndsWith("$UnlockToBoss") || lines[currentLine].StartsWith("$UnlockToBoss"))
            {
                currentState = CutsceneState.Waiting;
                Floor floor = FindObjectOfType<Floor>();
                floor.UnlockToBossDoor();

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$UnlockFromBoss") || lines[currentLine].StartsWith("$UnlockFromBoss"))
            {
                currentState = CutsceneState.Waiting;
                Floor floor = FindObjectOfType<Floor>();
                floor.UnlockFromBossDoor();

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$ChronoStop") || lines[currentLine].StartsWith("$ChronoStop"))
            {
                int timeStartIndex = lines[currentLine].IndexOf('(');
                int timeEndIndex = lines[currentLine].IndexOf(')');
                string timeInfo = lines[currentLine].Substring(timeStartIndex + 1, timeEndIndex - timeStartIndex - 1);
                bool pauseBool = bool.Parse(timeInfo);
                Debug.Log(timeInfo + " " + pauseBool);
                if (pauseBool)
                {
                    GameTime.PauseTime(false);
                }
                else
                {
                    GameTime.UnpauseTime();
                }

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            #endregion
            #region Inventory Functions
            else if (lines[currentLine].EndsWith("$GiveWeapon") || lines[currentLine].StartsWith("$GiveWeapon"))
            {
                currentState = CutsceneState.Waiting;
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

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$GiveItem") || lines[currentLine].StartsWith("$GiveItem"))
            {
                currentState = CutsceneState.Waiting;
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

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].EndsWith("$GiveAbility") || lines[currentLine].StartsWith("$GiveAbility"))
            {
                currentState = CutsceneState.Waiting;
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

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            #endregion
            #region Quest Functions
            else if (lines[currentLine].EndsWith("$SetQuest") || lines[currentLine].StartsWith("$SetQuest"))
            {
                currentState = CutsceneState.Waiting;
                int moveStartIndex = lines[currentLine].IndexOf('(');
                int moveEndIndex = lines[currentLine].IndexOf(')');
                string questInfo = lines[currentLine].Substring(moveStartIndex + 1, moveEndIndex - moveStartIndex - 1);
                string[] splitString = questInfo.Split(',');
                questSystem.SetQuest(Int32.Parse(splitString[0]), Int32.Parse(splitString[1]));

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            #endregion
            #region UI Functions
            else if (lines[currentLine].StartsWith("$Emote") || lines[currentLine].EndsWith("$Emote"))
            {
                currentState = CutsceneState.Waiting;

                int emoteStartIndex = lines[currentLine].IndexOf('(');
                int emoteEndIndex = lines[currentLine].IndexOf(')');
                string emoteInfo = lines[currentLine].Substring(emoteStartIndex + 1, emoteEndIndex - emoteStartIndex - 1);
                Sprite emote = null;
                foreach(Sprite sprite in CharacterPics)
                {
                    if(sprite.name == emoteInfo)
                    {
                        emote = sprite;
                        break;
                    }
                }
                dialogueGUI.SetProfilePic(emote);
                /*try
                {
                    emotions.Add(lines[currentLine].Split("|")[1], lines[currentLine].Split("|")[2]);
                }
                catch (ArgumentException)
                {
                    emotions.Remove(lines[currentLine].Split("|")[1]);
                    emotions.Add(lines[currentLine].Split("|")[1], lines[currentLine].Split("|")[2]);
                }*/

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].StartsWith("$ToggleUI") || lines[currentLine].EndsWith("$ToggleUI"))
            {
                currentState = CutsceneState.Waiting;

                int toggleStartIndex = lines[currentLine].IndexOf('(');
                int toggleEndIndex = lines[currentLine].IndexOf(')');
                string toggleInfo = lines[currentLine].Substring(toggleStartIndex + 1, toggleEndIndex - toggleStartIndex - 1);

                if (dialogueGUI)
                {
                    dialogueGUI.ToggleGUI(bool.Parse(toggleInfo));
                }

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].StartsWith("$TogglePlayerUI") || lines[currentLine].EndsWith("$TogglePlayerUI"))
            {
                currentState = CutsceneState.Waiting;

                int toggleStartIndex = lines[currentLine].IndexOf('(');
                int toggleEndIndex = lines[currentLine].IndexOf(')');
                string toggleInfo = lines[currentLine].Substring(toggleStartIndex + 1, toggleEndIndex - toggleStartIndex - 1);

                playerGUI.SetActive(bool.Parse(toggleInfo));

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            else if (lines[currentLine].StartsWith("$ToggleInventoryUI") || lines[currentLine].EndsWith("$ToggleInventoryUI"))
            {
                currentState = CutsceneState.Waiting;

                int toggleStartIndex = lines[currentLine].IndexOf('(');
                int toggleEndIndex = lines[currentLine].IndexOf(')');
                string toggleInfo = lines[currentLine].Substring(toggleStartIndex + 1, toggleEndIndex - toggleStartIndex - 1);

                bool active = bool.Parse(toggleInfo);
                InventoryGUI inventoryGUI = FindObjectOfType<InventoryGUI>();
                if(active)
                {
                    inventoryGUI.inventoryUI.SetActive(true);
                    GameState.CurrentState = GameState.State.Inventory;
                }
                else
                {
                    inventoryGUI.inventoryUI.SetActive(false);
                    GameState.CurrentState = GameState.State.None;
                }
                playerGUI.SetActive(bool.Parse(toggleInfo));

                //Auto move to the next line
                currentLine++;
                OnDialogueUpdate();
            }
            #endregion
            else
            {
                currentState = CutsceneState.Continue;
                if (dialogueGUI)
                {
                    dialogueGUI.ToggleGUI(true);
                }
                //currentChoice = PlayerChoice.None;
                string[] dialogueLine = currentLineString.Split(':');
                if (dialogueGUI && dialogueLine.Length == 2)
                {
                    dialogueGUI.SetDialogue(dialogueLine[0], dialogueLine[1]);
                }

                playerAudio.NextLine(currentLine);

                currentLine++;
            }
        }
        //When there are no more lines of dialog to read.
        else if (currentLine > lines.Length - 1)
        {
            currentState = CutsceneState.None;
            if (dialogueGUI)
            {
                dialogueGUI.ToggleButtons(false);
                dialogueGUI.ToggleGUI(false);
            }
            playerGUI.SetActive(true);
            ResetCamera();
            movement.EndCutscene();
            instaSkip = false;
            auto = false;
            questSystem.QuestEvent(QuestSystem.QuestEventType.CutsceneEnd, cutscene.ToString());
        }
    }

    #region Cutscene Visual Effects
    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        //Auto move to the next line
        currentLine++;
        OnDialogueUpdate();
    }

    IEnumerator FadeIn(float r, float g, float b)
    {
        //Debug.Log("Test FadeIn");
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
        yield return new WaitForSeconds(.3f);

        //Auto move to the next line
        currentLine++;
        OnDialogueUpdate();
    }

    IEnumerator FadeOut(float r, float g, float b)
    {
        //Debug.Log("Test Fadeout");
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

        //Auto move to the next line
        currentLine++;
        OnDialogueUpdate();
    }
    #endregion

    #region CutsceneMovement
    IEnumerator MoveViaLerp(float time, Transform cameraTransform, Vector3 target)
    {
        float interpol = 0;
        while ((cameraTransform.position - target).sqrMagnitude > 0.001f)
        {
            //Debug.Log($"{interpol} | Camera:{cameraTransform.position} | target: {target}");
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, target, interpol);

            interpol += 0.01f;
            yield return new WaitForSeconds(time);
        }
        cameraTransform.position = target;

        //Auto move to the next line
        currentLine++;
        OnDialogueUpdate();
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
        if (character.name == "Player")
        {
            characterAI.enabled = true;
        }
        currentState = CutsceneState.Moving;
    }

    void MoveCharacter()
    {
        if (!characterAI.MoveTowardsTarget(targetPosition))
        {
            characterAI.SetCanMove(false);
            characterAI.SetTarget(null);
            characterAnimator.SetBool("isWalking", false);
            if (characterAI.gameObject.name == "Player")
            {
                characterAI.enabled = false;
            }

            //Auto move to the next line
            currentLine++;
            OnDialogueUpdate();
        }
        else if (characterAnimator != null)
        {
            //print("Moving character for cutscene");
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
        lastChoice = PlayerChoice.ChoiceOne;
        if (dialogueGUI)
        {
            dialogueGUI.ToggleButtons(true);
        }
        while (lines[currentLine].Contains("$OptionB"))
        {
            currentLine++;
        }
        OnDialogueUpdate();
        //currentState = CutsceneState.Continue;
    }
    public void ButtonTwoSelect()
    {
        currentChoice = PlayerChoice.ChoiceTwo;
        lastChoice = PlayerChoice.ChoiceTwo;
        if (dialogueGUI)
        {
            dialogueGUI.ToggleButtons(true);
        }
        while (lines[currentLine].Contains("$OptionA"))
        {
            currentLine++;
        }
        OnDialogueUpdate();
        //currentState = CutsceneState.Continue;
    }

    /*void UpdateImage(string name)
    {
        string myEmote = "";

        emotions.TryGetValue(name, out myEmote);

        if (myEmote == null) { myEmote = "Neutral"; }

        myEmote = name + myEmote;

        if (dialogueGUI)
            dialogueGUI.SetProfilePic(null);

        foreach (Sprite sprite in CharacterPics)
        {
            if (sprite.name == myEmote)
            {
                if (dialogueGUI)
                    dialogueGUI.SetProfilePic(sprite);
                else
                    dialogueGUI.SetProfilePic(null);
            }
        }
    }*/
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
        //Debug.Log("DialogManager: " + newDialog.ToString());
        cutscene = newDialog;
        StartCutScene(cutscene.ToString(), 0);
    }

    public void SkipCutscene()
    {
        auto = !auto;
        skipTimer = skipCooldown;
    }

    public void DevSkipCutscene()
    {
        instaSkip = !instaSkip;
    }

    public CutsceneState getCutsceneState()
    {
        return currentState;
    }
    #endregion
}




