
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Dialogue
{
    public string[] getDialogue(string name)
    {
        switch (name)
        {
            #region Floor 0
            case "WakeUpTutorial":
                return WakeUpTutorial;
            case "SlimeLadyReplacement":
                return SlimeLadyReplacement;
            case "MeetingTheSlimeLady":
                return MeetingTheSlimeLady;
            case "InventoryIntro":
                return InventoryIntro;
            case "InventroyOpening":
                return InventroyOpening;
            case "InventoryExplaination":
                return InventoryExplaination;
            case "StrangeRoom":
                return StrangeRoom;
            case "CombatIntro":
                return CombatIntro;
            case "CombatDodging":
                return CombatDodging;
            case "CombatDodgingDone":
                return CombatDodgingDone;
            case "CombatTableBreaker":
                return CombatTableBreaker;
            case "CombatEnemyInbound":
                return CombatEnemyInbound;
            case "TutorialOver":
                return TutorialOver;
            case "VarrenEncounter":
                return VarrenEncounter;
            #endregion
            #region Floor 1
            case "DrSteinBossDoor":
                return DrSteinBossDoor;
            case "DrSteinBossDoor1":
                return DrSteinBossDoor1;
            case "DrSteinBossDoor2":
                return DrSteinBossDoor2;
            case "AStrangeMan":
                return AStrangeMan;
            case "DrHarrisQuestIntro":
                return DrHarrisQuestIntro;
            case "DrHarrisIntroReplacement":
                return DrHarrisIntroReplacement;
            case "DrHarrisQuest1":
                return DrHarrisQuest1;
            case "DrHarrisQuest2":
                return DrHarrisQuest2;
            case "DrHarrisQ2Replacement":
                return DrHarrisQ2Replacement;
            case "DrHarrisQuest3":
                return DrHarrisQuest3;
            case "DrHarrisQuest4":
                return DrHarrisQuest4;
            case "DrHarrisQ4Replacement":
                return DrHarrisQ4Replacement;
            case "DrHarrisQuest5":
                return DrHarrisQuest5;
            case "SteinIntro":
                return SteinIntro;
            case "SteinScene1":
                return SteinScene1;
            case "SteinEnd":
                return SteinEnd;
            #endregion
            #region Floor 2
            case "KarenIntro":
                return KarenIntro;
            case "KarenInter1":
                return KarenInter1;
            case "KarenInter2":
                return KarenInter2;
            case "KarenEnd":
                return KarenEnd;
            #endregion
            case "Empty":
                return Empty;
        }
        return null;
    }

    public enum Dialog
    {
        None,
        Empty,
        #region Floor 0
        WakeUpTutorial,
        SlimeLadyReplacement,
        MeetingTheSlimeLady,
        InventoryIntro,
        InventroyOpening,
        InventoryExplaination,
        StrangeRoom,
        CombatIntro,
        CombatDodging,
        CombatDodgingDone,
        CombatTableBreaker,
        CombatEnemyInbound,
        TutorialOver,
        VarrenEncounter,
        #endregion
        #region Floor 1
        DrSteinBossDoor,
        DrSteinBossDoor1,
        DrSteinBossDoor2,
        AStrangeMan,
        DrHarrisQuestIntro,
        DrHarrisIntroReplacement,
        DrHarrisQuest1,
        DrHarrisQuest2,
        DrHarrisQ2Replacement,
        DrHarrisQuest3,
        DrHarrisQuest4,
        DrHarrisQ4Replacement,
        DrHarrisQuest5,
        SteinIntro,
        SteinScene1,
        SteinEnd,
        #endregion
        #region Floor 2
        KarenIntro,
        KarenInter1,
        KarenInter2,
        KarenEnd,
        #endregion
    }

    /// <summary>
    /// Below here is all of the dialog in the game. It is contained within string arrays and utilizes a fusion of rich text and custom commands.
    ///                             The current Text commands will be put below in as Name | Syntax | Use Case:
    ///                                      
    ///                                       AUDIO FUNCTIONS
    /// $PlayEffect   |   $PlayEffect|{SFX Name}                    |         This command plays sound effects. 
    ///       ONLY AT START OF LINE                                           What SFX is available depends on what AudioClips are listed
    ///                                                                       on the MusicBox Gameobject's Audio Sounds
    ///                                                         
    /// $PlaySong     |   $PlaySong|{Song Name}                     |         This command plays music. 
    ///       ONLY AT START OF LINE                                           What music is available depends on what AudioClips are listed
    ///                                                                       on the MusicBox Gameobject's Audio Sounds
    ///                                                         
    /// $Pause        |   $Pause                                    |         This command stops all music and ambience.
    ///       ONLY AT START OF LINE
    /// 
    /// $Resume       |   $Resume                                   |         This command restarts all music and ambience.
    ///       ONLY AT START OF LINE
    ///             
    ///                                        CHOICE FUNCTIONS
    /// $Prompt       |   |{Choice A}|{Choice B}$Prompt             |         This command causes buttons to appear that the player pick from. 
    ///     ONLY AT END OF LINE                                               Choice A & Choice B will be the choices the player must pick from.
    ///     
    /// $ChoiceA      |   $ChoiceA                                  |         This command marks which dialog will only be available to
    ///     ONLY AT END OF LINE                                               those that chose Choice A during the most recent prompt. 
    ///                                                                       
    /// $ChoiceB      |   $ChoiceB                                  |         This command marks which dialog will only be available to
    ///     ONLY AT END OF LINE                                               those that chose Choice B during the most recent prompt.
    ///     
    ///  $Move        |   $Move(name,x,y,z)                         |         This command moves a gameobject the inputted world coordinates.
    ///     ONLY IN OWN LINE                                                  This can only move gameobjects with the AI Component (Excluding the Main Camera).
    ///      
    ///  $Tele        |   $Tele(name,x,y,z)                         |         This command teleports a gameobject to the inputted world coordinates.
    ///     ONLY IN OWN LINE
    ///     
    ///                                        CUTSCENE FUNCTIONS
    ///  $Kill        |   $Kill(name)                               |         This command destroys the inputted gameobject
    ///     ONLY IN OWN LINE                                                  
    ///     
    ///  $Animate     |   $Animate(name,variable,?bool)             |         This command sets a variable of a gameobjects animator to play animations.
    ///     ONLY IN OWN LINE                                                  This can only modify triggers or booleans. Leave the bool out for triggers.
    ///     
    ///  $AfterImage  |   $AfterImage(name,bool)                    |         This command turns a Gameobject's AfterImage component on and off.
    ///     ONLY IN OWN LINE 
    ///     
    ///  $FadeIn      |   $FadeIn(r,g,b)                            |         This command causes a fade in of a select color to play.
    ///     ONLY IN OWN LINE                                                  This is usally used for transitions or hiding $Kill & $Tele.
    ///     
    ///  $FadeOut     |   $FadeOut(int or float)                    |         This command causes a fade out of a select color to play.
    ///     ONLY IN OWN LINE                                                  This is used after $FadeIn.
    ///
    ///                                        Inventory FUNCTIONS
    ///  $GiveWeapon  |   $GiveWeapon(name)                         |         This command places a select weapon in the player's inventory.
    ///     ONLY IN OWN LINE    
    ///     
    ///  $GiveItem    |   $GiveItem(name)                           |         This command places a select item in the player's inventory.
    ///     ONLY IN OWN LINE   
    ///     
    ///  $GiveAbility |   $GiveAbility(name)                        |         This command places a select ability in the player's inventory.
    ///     ONLY IN OWN LINE    
    ///
    ///                                        Quest FUNCTIONS
    ///  $SetQuest    |   $SetQuest(name)                           |         This command sets the current quest of the player.
    ///     ONLY IN OWN LINE   
    ///
    ///                                        UI FUNCTIONS
    ///  $Emote       |   $Emote|character|emotion                  |         This command sets the current portrait.
    ///     ONLY IN OWN LINE                                                  Avaliable emotes are determined by the Dialogue Manager's Character Pics Array.
    ///     
    ///  $ToggleUI    |   $ToggleUI(bool)                           |         This command sets the visibility of the Dialog GUI.
    ///     ONLY IN OWN LINE       
    /// </summary>

    string[] Empty = new string[]
    {
        ".:."
    };

    #region Tutorial Dialog
    string[] WakeUpTutorial = new string[]
    {
        "$PlaySong|Tutorial",
        "$Emote(Blaze_Shocked)",
        "Blaze: Ughhh",
        "Blaze: What? Where am I?",
        "Blaze: *looks around* Is this a... barn?",
        "$Emote(Blaze_Angry)",
        "Blaze: Well, just standing here isn't going to accomplish anything",
        "$Emote(Blaze_Neutral)",
        "Blaze: I wonder if there is anything useful around here?",
        "$Emote(Null)",
        " : Use <b>WASD</b> to move",
    };

    //This dialog should allow the player to skip to the VarrenEncounter
    string[] MeetingTheSlimeLady = new string[]
    {
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_Meeting,Talk,true)",
        "???: Oh, dear, a new guest.",
        "$Emote(Blaze_Neutral)",
        "$Animate(SlimeLady_Meeting,Talk,false)",
        "Blaze: Guest?",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_Meeting,Talk,true)",
        "???: Don't worry, deary, all will be explained in due time.",
        "???: My name is Samantha. Just go out the door to the right and I'll show you the ropes!",
        "Samantha: Or are you confident you don't need my tutoring?|Teach me please|No thank you $Prompt",
        "$Emote(Blaze_Neutral) $OptionA",
        "Blaze: I don't really get what is going on but sure! $OptionA",
        "$Emote(SlimeLady) $OptionA",
        "Samantha: Alright, deary, come along now. $OptionA",
        "$Emote(Blaze_Neutral) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose1,false) $OptionB",
        "Blaze: No thank you, I'm sure I can figure stuff out on my own $OptionB",
        "$Emote(SlimeLady) $OptionB",
        "$Animate(SlimeLady_Meeting,Talk,true) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose2,true) $OptionB",
        "Samantha: Alright deary, just go through the door on the right and you're free to go whenever $OptionB",
        "$GiveWeapon(Dagger) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose1,false) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose2,false) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose3,true) $OptionB",
        "Samantha: Oh...and take this before you go $OptionB",
        " : You got <color=#507830><b>Dagger</b></color>! $OptionB",
        "$Animate(SlimeLady_Meeting,Pose1,true) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose3,false) $OptionB",
        "$Animate(SlimeLady_Meeting,Pose1,false) $OptionA",
        "$Animate(SlimeLady_Meeting,Talk,false)",
        "$Move(SlimeLady_Meeting,-11.215,-14.09) $OptionA",
        "$Kill(SlimeLady_Meeting) $OptionA",
        "$Animate(SlimeLady_InventoryIntro,Pose1,false)",
        "$Animate(SlimeLady_InventoryExplanation,Pose1,false)",
        "$Animate(SlimeLady_CombatIntro,Pose1,false)",
        "$Animate(SlimeLady_CombatExplanation,Pose1,false)"
    };

    string[] SlimeLadyReplacement = new string[]
    {
        "$Emote(SlimeLady)",
        "Samantha: Hurry along now, dear."
    };

    //This cutscene should allow the player to skip to StrangeRoom
    string[] InventoryIntro = new string[]
    {
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_InventoryIntro,Talk,true)",
        "Samantha: First things first, the <b>inventory</b>.",
        "Samantha: Do you know how to use the inventory or should we just move on?|What inventory|Just move on $Prompt",
        "$Emote(Blaze_Neutral)",
        "$Animate(SlimeLady_InventoryIntro,Talk,false)",
        "Blaze: Inventory? $OptionA",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_InventoryIntro,Talk,true)",
        "Samantha: Oh dear, you don't know? $OptionA",
        "Samantha: Don't worry, honey, let's move on to the next room, and I'll show you what's what! $OptionA",
        "$Emote(Blaze_Happy)",
        "$Animate(SlimeLady_InventoryIntro,Talk,false)",
        "Blaze: Of course I know about the inventory! $OptionB",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_InventoryIntro,Talk,true)",
        "Samantha: Okay, deary, I'm just making sure. $OptionB",
        "Samantha: Let's move on to the next room, deary. I got more stuff to show you still. $OptionB",
        "$Animate(SlimeLady_InventoryIntro,Talk,false)",
        "$Move(SlimeLady_InventoryIntro,2.991,-1.261)",
        "$Kill(SlimeLady_InventoryIntro)"
        //2.991, -1.261
    };

    string[] InventroyOpening = new string[]
    {
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_InventoryExplanation,Talk,true)",
        "Samantha: First things first. Press <b>tab</b> for me, would you, honey.",
        "$Animate(SlimeLady_InventoryExplanation,Talk,false)",
    };

    string[] InventoryExplaination = new string[]
    {
        "$TogglePlayerUI(True)",
        "$Emote(SlimeLady)",
        "Samantha: This here is your inventory! Everything you can use will end up here.",
        "Samantha: If there's ever something you want to equip, you just have to select it and then click the <b>equip button</b>.",
        "Samantha: And if there's something you don't want, just select it and press that <b>red trash button</b>.",
        "Samantha: The same logic applies across every tab, so now you know how to use your inventory!",
        "Samantha: Now, go into the next room, deary. I got more stuff set up there.",
        "$ToggleInventoryUI(False)",
        //"$ChronoStop(False)",
        "$Move(SlimeLady_InventoryExplanation,4.6,-14.184)",
        "$Kill(SlimeLady_InventoryExplanation)"
    };

    string[] StrangeRoom = new string[]
    {
        "$Emote(Blaze_Surprised)",
        "Blaze: Where's the green lady? Shouldn't she be here?",
        "Blaze: This room...",
        "Blaze: It feels... Strange.",
        "$Emote(Blaze_Shocked_Eyeless)",
        " : You feel a strange pull towards the chair.",
        " : Maybe something will happen if you interact with it with the <b>'F'</b> key."
    };

    //This should allow the player to skip to TutorialOver
    string[] CombatIntro = new string[]
    {
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatIntro,Talk,true)",
        "Samantha: Next up, <b>combat</b>.",
        "Samantha: Do you know how to fight or should we just move on?|I'm no fighter|Just move on $Prompt",
        "$Emote(Blaze_Neutral)",
        "$Animate(SlimeLady_CombatIntro,Talk,false)",
        "Blaze: I'm not much of a fighter. $OptionA",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatIntro,Talk,true)",
        "Samantha: Oh deary, you gotta fight down here to stay safe. $OptionA",
        "Samantha: I'll train you a bit so that you're a good fighter. Let's move to the next room, and I'll show you! $OptionA",
        "$Emote(Blaze_Happy) $OptionB",
        "$Animate(SlimeLady_CombatIntro,Talk,false) $OptionB",
        "Blaze: I'm sure I can handle myself! $OptionB",
        "$Emote(SlimeLady) $OptionB",
        "$Animate(SlimeLady_CombatIntro,Talk,true) $OptionB",
        "Samantha: Ok, dear, how dependable. $OptionB",
        "Samantha: Then you are good to go. $OptionB",
        "$Animate(SlimeLady_CombatIntro,Talk,false)",
        "$Animate(SlimeLady_CombatIntro,Pose1,true) $OptionB",
        "$Move(SlimeLady_CombatIntro,22.701,-13.762) $OptionA",
        "$Kill(SlimeLady_CombatIntro) $OptionA"
    };

    string[] CombatDodging = new string[]
    {
        "$Emote(SlimeLady)",
        "Samantha: In this room, bullets will shoot towards you.",
        "$Emote(Blaze_Shocked)",
        "Blaze: WHAT?!",
        "$Emote(SlimeLady)",
        "Samantha: Don't worry, sweetie. As long as you dash at the right time, you'll be fine.",
        "$Emote(Blaze_Surprised)",
        "Blaze: Dash?",
        "$Emote(SlimeLady)",
        "Samantha: Yes, pressing <b> Shift, Space, or Right Clicking</b> will give you a burst of speed!",
        "Samantha: You will also be safe from getting hit for a small window at the start of a dash!",
        "Samantha: Try it out!!",
    };

    string[] CombatDodgingDone = new string[]
    {
        "$Emote(SlimeLady)",
        "Samantha: Good job! Move on now. I still got a little more to show you.",
    };

    string[] CombatTableBreaker = new string[]
    {
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatExplanation,Talk,true)",
        "Samantha: Now it is time to teach you how to fight.",
        "Samantha: Close your eyes for just a moment. I need to get something out.",
        "$Animate(SlimeLady_CombatExplanation,Talk,false)",
        "ToggleUI(False)",
        "$FadeIn(0,0,0)",
        "$Tele(Table1,40.14,0.8)",
        "$Tele(Table2,40.84,0.1199999)",
        "$Tele(Table3,41.879,0.83)",
        "$Timer(1)",
        "$ToggleUI(True)",
        "$Animate(SlimeLady_CombatExplanation,Talk,true)",
        "Samantha: Now open your eyes!",
        "$FadeOut(0,0,0)",
        "Samantha: I have placed a couple tables over there. Go break them.",
        "$Emote(Blaze_Neutral)",
        "$Animate(SlimeLady_CombatExplanation,Talk,false)",
        "Blaze: What's the point of doing this?",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatExplanation,Talk,true)",
        "Samantha: Dearie, all the best stuff is in tables. Just <b>Left Click</b> on them. Try it and see!",
        "$Animate(SlimeLady_CombatExplanation,Talk,false)",
    };

    string[] CombatEnemyInbound = new string[]
    {
        "$Emote(Blaze_Surprised)",
        "Blaze: Whoa, why is this stuff <b>in</b> a table?",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatExplanation,Pose2,true)",
        "$Animate(SlimeLady_CombatExplanation,Talk,true)",
        "Samantha: Nobody knows. That's just how it is.",
        "Samantha: Anyway, now that you have yourself some nice loot...",
        "Samantha: Now is the time for you to make use of it!",
        "Samantha: Just so you know, this is the real deal. If you are in danger, I won't help you.",
        "$Animate(SlimeLady_CombatExplanation,Pose2,false)",
        "$Animate(SlimeLady_CombatExplanation,Pose3,true)",
        "Samantha: Oh, and take this.",
        "$Pause",
        "$GiveAbility(Multishot)",
        "$Emote(Blaze_Happy)",
        " : You got <color=#507830><b>Multishot</b></color>!",
        "$Resume",
        "$Emote(SlimeLady)",
        "Samantha: Be sure to equip this. You'll need it.",
        "$Animate(SlimeLady_CombatExplanation,Pose3,false)",
        "$Animate(SlimeLady_CombatExplanation,Talk,false)",
    };

    string[] TutorialOver = new string[]
    {
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatExplanation,Talk,true)",
        "Samantha: I have nothing left to teach you.",
        "Samantha: I'm sure that you'll be safe out there, but still, be careful.",
        "Samantha: Varren has been waiting for you since you got here. Don't keep him waiting any longer.",
        "$Emote(Blaze_Angry)",
        "$Animate(SlimeLady_CombatExplanation,Talk,false)",
        "Blaze: Varren? Does he know me? Did he bring me here?",
        "$Emote(SlimeLady)",
        "$Animate(SlimeLady_CombatExplanation,Talk,true)",
        "$Animate(SlimeLady_CombatExplanation,Pose2,true)",
        "Samantha: You'll understand the situation soon enough. Go out the door on the right and best of luck.",
        "$Animate(SlimeLady_CombatExplanation,Talk,false)",
        "$Animate(SlimeLady_CombatExplanation,Pose2,false)",
    };

    string[] VarrenEncounter = new string[]
    {
        "$Tele(Varren,43.462,-15.558)",
        "$Move(Varren,39.21,-15.66)",
        "$Emote(Varren_Surprised)",
        "$Animate(Varren,Talk,true)",
        "???: Oh? And who might you be?|Hide my name|HOLY SH- $Prompt",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: No one.$OptionA",
        "Blaze: Who are you? Where did you come from? $OptionA",
        "$Emote(Varren_EvilSmile)",
        "$Animate(Varren,Talk,true)$OptionA",
        "???: Me? I'm Varren, a... resident of this lovely establishment. $OptionA",
        "$Emote(Blaze_Shocked)",
        "Blaze: AAAHHHHHHHHH. A GHOST!!! $OptionB",
        "$Emote(Varren_MildlyAnnoyed)",
        "$Animate(Varren,Pose1,true)$OptionB",
        "$Animate(Varren,Talk,true)$OptionB",
        "???: Calm down kid, i'm not a ghost.",
        "My name is Varren, i'm just another person trapped here like you. $OptionB",
        "$Animate(Varren,Pose1,false)$OptionB",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: Where are we?",
        "$Emote(Varren_MildlyAnnoyed)",
        "$Animate(Varren,Talk,true)",
        "Varren: Hell, of course! You died! Where else did you think you would end up?",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: What do you mean? Isn't this a barn?",
        "$Emote(Varren_EvilSmile)",
        "$Animate(Varren,Talk,true)",
        "Varren: It's a hotel, and it's where you'll be trapped for eternity because of all of the terrible things you've done.|He must be joking|What things $Prompt",
        "$Emote(Blaze_Surprised)",
        "$Animate(Varren,Talk,false)",
        "Blaze: Are you pulling my leg?$OptionA",
        "Blaze: I think I'd remember dying.$OptionA",
        "$Emote(Blaze_Surprised)",
        "Blaze: What terrible things?$OptionB",
        "$Emote(Varren_Neutral)",
        "$Animate(Varren,Talk,true) $OptionB",
        "Varren: Murder, theft, being an all around terrible person.$OptionB",
        "Varren: Whatever horrible deeds you've committed, you're now stuck here as punishment.$OptionB",
        "$Emote(Blaze_Surprised)",
        "$Animate(Varren,Talk,false) $OptionB",
        "Blaze: But why would I be sent to a hotel?$OptionB",
        "$Emote(Varren_MildlyAnnoyed)",
        "$Animate(Varren,Talk,true)",
        "Varren: Haven't you noticed how this place seems off? The rooms shifting, the lack of windows, the monsters?",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: Yes, but...",
        "$Emote(Varren_EvilSmile)",
        "$Animate(Varren,Talk,true)",
        "Varren: Welcome to hell. You died. You're stuck here to be forever haunted by those... things. There is no exit, no escape. Your atrocities have condemned you to this place.",
        "$Emote(Blaze_Surprised)",
        "$Animate(Varren,Talk,false)",
        "Blaze: But there has to be some kind of mistake. I don't remember committing any atrocities or dying; maybe if I look around for long enough then I can find an exit, or the person who trapped us here?",
        "$Emote(Varren_MildlyAnnoyed)",
        "$Animate(Varren,Talk,true)",
        "Varren: You're going to fail. I've searched every nook and cranny of this place. I'm sorry, but there's no way out, kid.",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: Maybe you missed a spot?",
        "$Emote(Varren_Neutral)",
        "$Animate(Varren,Talk,true)",
        "Varren: No.",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: Maybe there is a hidden room?",
        "$Pause",
        "$Animate(Varren,Pose1,true)",
        "$Animate(Varren,Talk,true)",
        "$Emote(Varren_Angry)",
        "Varren: <b>NO!!!</b>",
        "$Emote(Varren_MildlyAnnoyed)",
        "Varren: <i>*Sigh*</i>",
        "Varren: <i>stubborn little...</i>",
        "$Emote(Varren_Neutral)",
        "Varren: There aren't any hidden rooms.",
        "Varren: There is no one watching us.",
        "Varren: And there certainly aren't any exits.",
        "Varren: Once you arrive, there is no way to leave. You're stuck here.",
        "Varren: Forever.",
        "Varren: <b>And Ever.</b>",
        "Varren: <b><u>And Ever...</b></u>",
        "Varren: ...",
        "$Resume",
        "$Emote(Varren_SoftSmiles)",
        "$Animate(Varren,Pose1,false)",
        "Varren: Since we're stuck here, why don't you go ahead and tell me what it is you've done to end up here?",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: I really don't remember. There has to be some sort of mix-up here.",
        "Blaze: I'm going to find whoever put us here and prove that there is a way out.",
        "$Emote(Varren_MildlyAnnoyed)",
        "$Animate(Varren,Talk,true)",
        "Varren: There's no convincing you, is there, kid?|Giving up does nothing|I refuse to stay here $Prompt",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: Well, just giving up and just sitting here isn't going to accomplish anything.$OptionA",
        "Blaze: There's no way I can stay in this stinky place.$OptionB",
        "$Emote(Varren_EvilSmile)",
        "$Animate(Varren,Talk,true)",
        "Varren: Your stubbornness is quite amusing...",
        "$Animate(Varren,Pose2,true)",
        "Varren: You can't die here, but you can still feel pain. Let's see how far you get before you give up.",
        "Varren: Cya later kiddo",
        "$Animate(Varren,Pose2,false)",
        "$Animate(Varren,Talk,false)",
        "$Move(Varren,43.462,-15.558)",
        "$Emote(Blaze_Neutral)",
        "Blaze: Well, I guess I should get moving.",
        "$Kill(Varren)"
    };

    #endregion
    #region Floor 1 Dialog
    #region Side Quest
    //(The player must defeat enemies using the scalpel, as enemies take bleeding DOT, the blood bag fills up with blood,
    //and once the bag is 100% full, the player can meet with the doctor again)


    //If the player tries to enter the boss room before finishing Harris's quest
    string[] DrSteinBossDoor = new string[]
    {
        " : You can hear the sounds of drilling and hammering coming from behind the door.",
        "Dr. Stein: I won't fail again...",
        "Dr. Stein: I'll save you this time, Victor"
    };

    string[] DrSteinBossDoor1 = new string[]
    {
        "Dr. Stein: NO! NO! NO!",
        "Dr. Stein: Don't you die on me!"
    };

    string[] DrSteinBossDoor2 = new string[]
    {
        " : Muffled sobbing can be heard from behind the door.",
        "Dr. Stein: I failed...",
        "Dr. Stein: Sniffle..."
    };

    string[] AStrangeMan = new string[]
    {
        "Blaze: Some strange guy? And is this an office?",
        " : Press <b>F</b> to interact with him."
    };

    string[] DrHarrisQuestIntro = new string[]
    {
        //The player enters the outpatient room, Doctor Dr. Harris is standing inside with a clipboard
        "$Animate(DrHarris_Intro,Talk1,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: There you are! You must be my new assistant!",
        "$Emote(Blaze_Surprised)",
        "Blaze: Assistant?",
        "$Emote(DrHarris)",
        "Dr. Harris: Yes. Come along now; we have lots to do. We need to get everything ready for the surgery.",
        "$Emote(Blaze_Shocked)",
        "Blaze: Surgery?",
        "$Emote(DrHarris)",
        "Dr. Harris: Did the nurse not tell you anything? I'm afraid the good doctor's son won't make it if we aren't on schedule.",
        "$Emote(Blaze_Neutral)",
        "Blaze: His son? What happened to him?",
        "$Animate(DrHarris_Intro,Talk1,false)",
        "$Animate(DrHarris_Intro,Talk2,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: A horrendous accident! The poor lad took a hard hit during the semifinals.",
        "$Emote(Blaze_Surprised)",
        "Blaze: The semifinals?",
        "$Emote(DrHarris)",
        "Dr. Harris: Indeed, but don't fret, my boy. I am fully confident in the abilities of Dr. Stein, and that's where we come in.",
        "Dr. Harris: Now let's see here; first off, we need bandages and antiseptics. I'll handle that, you just collect some rootweed and numblumbitantrilia.",
        "$Animate(DrHarris_Intro,Talk2,false)",
        "$Animate(DrHarris_Intro,Talk1,true)",
        "$Emote(Blaze_Surprised)",
        "Blaze: Rootfeet and numblubi... what?",
        "$Emote(DrHarris)",
        "Dr. Harris: Rootweed, and.. forget it. You should be able to find some from defeating monsters.",
        "Dr. Harris: Come find me when you've got at least 20 pieces of each; don't keep me waiting.",
        "Dr. Harris: Toodaloo!",
        "$Animate(DrHarris_Intro,Talk1,false)"
        //Dr. Harris walks off into a different room
        //Collect quest where the player needs to get emotional energy
    };

    string[] DrHarrisIntroReplacement = new string[]
    {
        "$Emote(DrHarris)",
        "Dr. Harris: What are you waiting for, boy?",
        "Dr. Harris: Herbs don't collect themselves; get going!"
    };

    //Once the player has enough emotional energy
    string[] DrHarrisQuest1 = new string[]
    {
        "$ChronoStop(True)",
        "$Emote(Blaze_Angry)",
        "Blaze: I've defeated all of these monsters but found nothing but goop.",
        "Blaze: Hopefully this is what he's talking about.",
        "$ChronoStop(False)",
    };

    string[] DrHarrisQuest2 = new string[]
    {
        //After collecting enough emotional energy (the goop), the player walks into a room where the doctor is
        "$Animate(DrHarris_Q2,Talk1,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: Ah! Just in time. I was wondering what was taking you so long.",
        "$Emote(Blaze_Angry)",
        "Blaze: I couldn't find anything but this goop.",
        "$Emote(DrHarris)",
        "Dr. Harris: Such a hard worker too! This should be more than enough. Keep this up, and I'll be sure to put in a good word with the doctor.",
        "$Emote(Null)",
        " : *As you hand Dr. Harris the goop, it immediately changes into a bundle of dried plant stems and a collection of purple flower heads*",
        "$Emote(Blaze_Surprised)",
        "Blaze: How did you do that?",
        "$Emote(DrHarris)",
        "Dr. Harris: Do what?",
        "$Emote(Blaze_Shcoked)",
        "Blaze: The goop! It turned into what we needed!",
        "$Emote(DrHarris)",
        "Dr. Harris: Goop? I appreciate your hard work, but please do be sure you are getting enough rest.",
        "Dr. Harris: If you would like, I can talk to the doctor about prescribing you some sleep medications.",
        "Dr. Harris: We can't have you making mistakes on the job now, can we?|No way|Yes please|$Prompt",
        "$Emote(Blaze_Neutral) $OptionA",
        "Blaze: No Thanks.$OptionA",
        "$Emote(DrHarris) $OptionA",
        "Dr. Harris: Suit yourself.$OptionA",
        "$Emote(Blaze_Happy) $OptionB",
        "Blaze: That would be nice.$OptionB",
        "$Emote(DrHarris) $OptionB",
        "Dr. Harris: I will be sure to get you some.$OptionB",
        "$Animate(DrHarris_Q2,Talk1,false)",
        "$Animate(DrHarris_Q2,Talk2,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: The next thing on the list is rimblenut; I'll do that one, and O- Blood.",
        "Dr. Harris: For this one, you'll need something to collect the blood in, so let's see here.",
        "$Emote(Null)",
        " : Dr. Harris digs through his bag, which holds a collection of perfectly normal medical instruments...",
        " : and the most bizarre of ingredients...",
        " : before pulling out a scalpel.",
        "$Emote(DrHarris)",
        "Dr. Harris: Ah, this should make it easier. When you encounter the monsters, use this scalpel.",
        "Dr. Harris: Severing the arteries should make collecting the blood we need a breeze.",
        "Dr. Harris: As usual, come and find me when you're done.",
        "$Animate(DrHarris_Q2,Talk2,false)",
        "$Animate(DrHarris_Q2,Talk1,true)",
        "$Emote(Blaze_Neutral)",
        "Blaze: Why do we need all this blood",
        "$Emote(DrHarris)",
        "Dr. Harris: Victor, the good doctor's son, lost a lot of blood after the accident",
        "Dr. Harris: We will need to replinish it if he hopes to play in the finals.",
        "Dr. Harris: That's enough questions for now. We must hurry if we want to save our star athlete in time!",
        "Dr. Harris: Toodaloo!",
        "$Animate(DrHarris_Q2,Talk1,false)",
        //(Dr. Harris leaves the room)
        //Player must kill enemies using the scapel
    };

    string[] DrHarrisQ2Replacement = new string[]
    {
        "$Emote(DrHarris)",
        "Dr. Harris: What are you waiting for, boy?",
        "Dr. Harris: The patient needs blood, so go fetch some!"
    };

    string[] DrHarrisQuest3 = new string[]
    {
        "$ChronoStop(True)",
        "$Emote(Blaze_Happy)",
        "Blaze: The blood bag is full now",
        "$Emote(Blaze_Neutral)",
        "Blaze: I should head back",
        "$ChronoStop(False)",
    };

    string[] DrHarrisQuest4 = new string[]
    {
        //(Blaze enters a room with Dr. Dr. Harris)
        "$Animate(DrHarris_Q4,Talk1,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: Ah, good, you're here.",
        "$Emote(Blaze_Neutral)",
        "Blaze: Is everything okay?",
        "$Animate(DrHarris_Q4,Talk1,false)",
        "$Animate(DrHarris_Q4,Talk2,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: I'm afraid the good doctor's son isn't doing as well as we hoped.",
        "$Emote(Blaze_Surprised)",
        "Blaze: Did the rootfeet and numblistuff not work?",
        "$Emote(DrHarris)",
        "Dr. Harris: The treatment wasn't as effective as we had hoped.",
        "Dr. Harris: In fact, I was only barely able to deliver the ingredients before the patient died.",
        "$Emote(Blaze_Shocked)",
        "Blaze: He died?",
        "$Animate(DrHarris_Q4,Talk2,false)",
        "$Animate(DrHarris_Q4,Talk1,true)",
        "$Emote(DrHarris)",
        "Dr. Harris: ...",
        "Dr. Harris: Nonsense. Not if I have anything to say about it.",
        "Dr. Harris: I'm sure the good doctor has everything under control.",
        "Dr. Harris: You've been a great help. I'll be sure to inform the doctor of your exceptional performance.",
        "Dr. Harris: However, there is one thing that remains.",
        "Dr. Harris: Please fetch our young patient's jersy from the good doctor's office.",
        "Dr. Harris: He will need it for his big day!",
        "Dr. Harris: I'll be waiting for you in the operating room.",
        "$Emote(Blaze_Surprised)",
        "Blaze: Wait, your scapel",
        "$Emote(DrHarris)",
        "Dr. Harris: Hold onto it, my dear boy. Our job is not yet complete",
        "Dr. Harris: Toodaloo!",
        "$Animate(DrHarris_Q4,Talk1,false)",
        //The player needs to go to Dr. Stein's office and grab the jersey
    };

    string[] DrHarrisQ4Replacement = new string[]
    {
        "$Emote(DrHarris)",
        "Dr. Harris: The patient died? What nonsense!",
        "Dr. Harris: The good doctor says he can save him, so he shall!",
        "Dr. Harris: Get the patient's jersey. He'll need it for his big day!"
    };

    //    (Blaze finds his way to the doctor's office (outpatient tileset room) inside they can grab the jersey
    //    and possibly have a file with a list of some of the doctor's other patients including the experiments he did on them)

    string[] DrHarrisQuest5 = new string[]
    {
        //(Blaze finds the jersy
        "$Emote(Blaze_Neutral)",
        "Blaze: This looks like the jersy.",
        "Phone: Bzzz",
        "$Emote(Blaze_Surprised)",
        "Blaze: Wait, a phone, and it has service!",
        "Blaze: That creepy vampire was wrong! I can call someone and get out of here",
        "$Emote(Null)",
        " : You try to call home, but instead of the phone ringing, a pre-recorded message plays",
        "Victor: Dad, I can't wait for you to see me at the semi finals!",
        "Dr. Stein: Yeah...",
        "Victor: It sucks you missed our game against the Panthers, we absolutely crushed them.",
        "Dr. Stein: I'm sorry I missed it, Victor.",
        "Victor: It's okay. I know you're really busy with work, plus you promised you'd make it to this one!",
        "Victor: Especially with James out, I'll be the lead running back, and the coach told me several scouts would be coming!",
        "Dr. Stein: ...",
        "Victor: Dad?",
        "Dr. Stein: That's good, son.",
        "Victor: You can't make it... Can you?",
        "Dr. Stein: I'm sorry, I-... Something came up... I won't be able to make it.",
        "Victor: But you promised! This could be the biggest game of my college career!",
        "Dr. Stein: I know. I'm sorry. I'll make it up to you. I promise.",
        " : The line disconnects as Victor hangs up.",
        "Dr. Stein: Victor?",
        "Dr. Stein: Victor???",
        "Dr. Stein: ...",
        "Dr. Stein: I'm so sorry",
        " : The recording ends as Dr. Stein hangs up.",
        " : Looking around, you can see several copies of Victor's medicine chart.",
        " : A few of them have words written in large red ink on them:",
        " : <color=red><b>FAILURE</b></color>",
        " : <color=red><b>DECEASED</b></color>",
        " : <color=red><b>I'M SORRY</b></color>"
        //Player now needs to head to the boss room
    };
    #endregion
    #region Boss Fight
    string[] SteinIntro = new string[]
    {
        "$Emote(FrankNStein_Crazed)",
        "$Animate(FrankNStein_Intro,Talk,true)",
        "Dr. Frank N. Stein: ...",
        "Dr. Frank N. Stein: Ah! You're here. And not a moment too soon.",
        "Dr. Frank N. Stein: You have my thanks. Dr. Harris, let me know how useful you've been.",
        "Dr. Frank N. Stein: Thanks to your efforts, the surgery was a sucess!",
        "$Animate(FrankNStein_Intro,Talk,false)",
        "$Emote(Blaze_Surprised)",
        "Blaze: Wait, it was?",
        "$Emote(FrankNStein_Crazed)",
        "$Animate(FrankNStein_Intro,Talk,true)",
        "$Animate(FrankNStein_Intro,Pose1,true)",
        "Dr. Frank N. Stein: Of course it was. I, the great Dr. Frank N. Stein, have successfully cured death!",
        "Dr. Frank N. Stein: Behold the results of my work!",
        "Dr. Frank N. Stein: My son.",
        "Dr. Frank N. Stein: No...",
        "Dr. Frank N. Stein: My creation!",
        "Dr. Frank N. Stein: He's even better than before!",
        "Dr. Frank N. Stein: He's Alive!",
        "$ToggleUI(False)",
        "$Animate(FNSMonster_IntroCutscene,Electricute,true)",
        "$Timer(3)",
        "$FadeIn(255,255,255)",
        "$Kill(FNSMonster_IntroCutscene)",
        "$Tele(FrankNSteinMonster,18.31,-47.401)",
        "$Tele(FrankNStein,16.466,-44.5)",
        "$Kill(FrankNStein_Intro)",
        "$FadeOut(255,255,255)",
        "$ToggleUI(True)",
        "$Emote(Blaze_Shocked_Eyeless)",
        "Blaze: Eww. Gross.",
        "$Emote(FrankNStein_Crazed)",
        "Dr. Frank N. Stein: How dare you insult my son!",
        "Dr. Frank N. Stein: <b>DIE!</b>",
    };

    string[] SteinScene1 = new string[]
    {
        "$Animate(FrankNSteinMonster,Intermediate,True)",
        "$Animate(FrankNSteinMonster,StartIntermediate)",
        "$Emote(Blaze_Neutral)",
        "Blaze: You have to accept your son is dead.",
        "$Animate(FrankNStein,Talk,true)",
        "$Emote(FrankNStein_Crazed)",
        "Dr. Frank N. Stein: No! He's right in front of me!",
        "Dr. Frank N. Stein: Can't you see.|This isn't right|It's a monster|$Prompt",
        "$Animate(FrankNStein,Talk,false)",
        "$Emote(Blaze_Neutral) $OptionA",
        "Blaze: He wouldn't have wanted this.$OptionA",
        "$Animate(FrankNStein,Talk,true) $OptionA",
        "$Emote(FrankNStein_Crazed) $OptionB",
        "Dr. Frank N. Stein: No... you're wrong... $OptionA",
        "$Emote(Blaze_Angry) $OptionB",
        "Blaze: That's not him.$OptionB",
        "$Animate(FrankNStein,Talk,true) $OptionB",
        "$Emote(FrankNStein_Crazed) $OptionB",
        "Dr. Frank N. Stein: Nonsense! $OptionB",
        "$Animate(FrankNSteinMonster,Intermediate,False)",
        "$Animate(FrankNStein,Talk,false)"
        //"Need to have him enrage if option b is chosen
    };

    string[] SteinScene2 = new string[]
    {
        "$Animate(FrankNSteinMonster,Intermediate,True)",
        "$Emote(Blaze_Angry)",
        "Blaze: Can't you see he's in pain? Please, stop this.",
        "$Emote(FrankNStein_Crazed)",
        "Dr. Frank N. Stein: He... He's fine. He's healthy!",
        "Dr. Frank N. Stein: He's never been this fit in his life!",
        "Dr. Frank N. Stein: He looks weak to me|Stop this|$Prompt",
        "$Emote(Blaze_Angry) $OptionA",
        "Blaze: You have to stop this.$OptionA",
        "$Emote(FrankNStein_Crazed) $OptionA",
        "Dr. Frank N. Stein: Than let me show you his true power! $OptionA",
        "$Emote(Blaze_Neutral) $OptionB",
        "Blaze: He doesn't resent you for missing his game.$OptionB",
        "$Emote(FrankNStein_Crazed) $OptionB",
        "Dr. Frank N. Stein: No! I'm finally with my son again! $OptionB",
        //"Enrage if option A or B is chosen
    };

    string[] SteinEnd = new string[]
    {
        "$Emote(Null)",
        "$ToggleUI(False)",
        "$FadeIn(255,255,255)",
        "$Kill(FrankNSteinMonster)",
        "$Kill(FrankNStein)",
        "$Tele(FNSMonster_EndCutscene,20.328,-50.425)",
        "$Tele(Player,18.74,-50.49)",
        "$Tele(FrankNStein_Final,19.6,-47.46)",
        "$FadeOut(255,255,255)",
        "$MoveViaLerp(Main Camera,19.592,-50.49)",
        "$Move(FrankNStein_Final,20.18,-50.47)",
        "$Kill(FrankNStein_Final)",
        "$Animate(FNSMonster_EndCutscene,Stage2)",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: Noooo!",
        "Dr. Frank N. Stein: Victor...",
        "Dr. Frank N. Stein: I- I'm here... I'm sorry",
        "Dr. Frank N. Stein: I'm so sorry.",
        "$Emote(Null)",
        "Victor: d- d- dad...",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: I'm here, Victor. I- Don't go",
        "$Emote(Null)",
        "Victor: y- you",
        "Victor: s- s- save p- people",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: Yes, Victor! Stay with me! I can save you too!",
        "Dr. Frank N. Stein: HARRIS!",
        "$Emote(Null)",
        "Victor: y- you",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: Yes, Victor. I- I'm going to save you.",
        "Dr. Frank N. Stein: HARRIS PLEASE! WE NEED MORE BLOOD!",
        "$Emote(Null)",
        " : Dr. Harris is nowhere to be seen.",
        "Victor: you... already d-did.",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: No-No-No-No-No... Don't go",
        "$Animate(FNSMonster_EndCutscene,Stage3)",
        "Dr. Frank N. Stein: He- He's gone.",
        "Dr. Frank N. Stein: I... I promised him... I'd be there...",
        "$Emote(Blaze_Neutral)",
        "Blaze: I... I think he understood.",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: Understood what?",
        "$Emote(Blaze_Neutral)",
        "Blaze: You. You did everything you could to help him.",
        "Blaze: Just like you did everything you could to help all of your patients.",
        "Blaze: That's why you missed the game, isn't it?",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: There... There was a girl... Burst appendix.",
        "Dr. Frank N. Stein: We had to operate the next day, or she wouldn't have...",
        "Dr. Frank N. Stein: I should have called off.",
        "Dr. Frank N. Stein: If I had been there-",
        "$Emote(Blaze_Neutral)",
        "Blaze: There isn't anything different you could have done.",
        "Blaze: He still would have taken that hit.",
        "Blaze: You were where you needed to be, and I think he understood that.",
        "Blaze: You tried to cure him, but his injuries were too severe, and that's not your fault.",
        "Blaze: You did the best you could for him, and he knew that",
        "$Emote(FrankNStein_Sad)",
        "Dr. Frank N. Stein: ...",
        "Dr. Frank N. Stein: Maybe...",
        "Dr. Frank N. Stein: Maybe. You're right.",
        "Dr. Frank N. Stein: Thank you... for helping me to see things differently.",
        "Dr. Frank N. Stein: I'm sorry, Victor... I'll be with you soon...",
        "$UnlockFromBoss",
        "$ToggleUI(False)",
        "$Animate(FNSMonster_EndCutscene,Stage4)",
        "$BossPoof(FNSMonster_EndCutscene)",
        "$Timer(6)",
        "$Kill(FNSMonster_EndCutscene)",
        " : All that remains is a helmet.",
        "$Tele(Varren,18.39,-52.89)",
        "$Move(Varren,20.28,-50.79)",
        "$Animate(Varren,Talk,true)",
        "$Emote(Varren_Neutral)",
        "Varren: Huh.|HOLY F-|Ugh $Prompt",
        "$Emote(Blaze_Shocked) $OptionA",
        "$Animate(Varren,Pose1,true)",
        "$Animate(Varren,Talk,false) $OptionA",
        "Blaze: AHHHH THE GHOST!$OptionA",
        "$Emote(Varren_Angry) $OptionA",
        "$Animate(Varren,Talk,true) $OptionA",
        "Varren: I'm not a ghost. $OptionA",
        "$Emote(Blaze_Angry) $OptionB",
        "$Animate(Varren,Talk,false) $OptionB",
        "Blaze: You Again?$OptionB",
        "$Emote(Varren_EvilSmile) $OptionB",
        "$Animate(Varren,Talk,true) $OptionB",
        "Varren: Who else would it be? $OptionB",
        "$Emote(Varren_Neutral)",
        "$Animate(Varren,Talk,true)",
        "Varren: You managed to defeat Stein. I'm impressed.",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: He just seemed like he was being too hard on himself.",
        "Blaze: He blamed himself for the death of his son.",
        "$Emote(Varren_Neutral)",
        "$Animate(Varren,Talk,true)",
        "Varren: I see...",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: What's with the helmet?",
        "Blaze: Normally when monsters are defeated, they just drop goop, and sometimes items.",
        "$Emote(Varren_MildyAnnoyed)",
        "$Animate(Varren,Talk,true)",
        "$Animate(Varren,Pose1,false)",
        "$Animate(Varren,Pose2,true)",
        "Varren: That \"goop\" is called Emotional Energy, and it's a little bit different with people",
        "Varren: When a person passes on from here, they sometimes leave behind a memento.",
        "$Emote(Blaze_Neutral)",
        "$Animate(Varren,Talk,false)",
        "Blaze: A memento?",
        "$Emote(Varren_Neutral)",
        "$Animate(Varren,Talk,true)",
        "Varren: Yes, it's like a condensed fragment of the emotions they felt; when used, it's similar to an ability but stronger.",
        "$Emote(Blaze_Surprised)",
        "$Animate(Varren,Talk,false)",
        "$Animate(Varren,Pose2,false)",
        "Blaze: Oh.",
        "Blaze: WAIT!",
        "$Emote(Varren_Neutral)",
        "$Animate(Varren,Talk,true)",
        "Varren: What?",
        "$Emote(Blaze_Surprised)",
        "$Animate(Varren,Talk,false)",
        "Blaze: You said it's left behind when someone passes on!",
        "Blaze: So there IS a way out!",
        "$Emote(Varren_MildyAnnoyed)",
        "$Animate(Varren,Talk,true)",
        "$Animate(Varren,Pose1,true)",
        "Varren: Technically, but don't expect to return the real world alive.|...|I will... $Prompt",
        "$Emote(Blaze_Neutral) $OptionA",
        "$Animate(Varren,Talk,false)",
        "Blaze: Oh...$OptionA",
        "$Animate(Varren,Talk,true) $OptionA",
        "Varren: Well, good luck with your futile attempt to leave this place! $OptionA",
        "$Emote(Blaze_Angry) $OptionB",
        "Blaze: I will find a way out.$OptionB",
        "$Emote(Varren_EvilSmile) $OptionB",
        "$Animate(Varren,Talk,true) $OptionB",
        "Varren: Your resolve still isn't broken? $OptionB",
        "$Emote(Blaze_Angry)",
        "$Animate(Varren,Talk,false)",
        "Blaze: It's going to take more than that to discourage me!",
        "$Emote(Varren_EvilSmile)",
        "$Animate(Varren,Talk,true)",
        "$Animate(Varren,Pose1,false)",
        "Varren: Well, it only gets harder from here.",
        "Varren: Come find me at an elevator when you're ready to go to floor 2.",
        "Varren: I can't wait to see you fail.",
        "$Animate(Varren,Talk,false)",
        "$Animate(Varren,Pose1,false)",
        "$Animate(Varren,Pose2,false)",
        "$Move(Varren,18.39,-52.89)",
        "$Kill(Varren)",
        "$MoveViaLerp(Main Camera,18.74,-50.49)",
    };
    #endregion
    #endregion
    #region Floor 2 Dialog
    #region Side Quest


    string[] Floor2Intro = new string[]
    {
        "Varren: About time, I was just about to leave you.",
        "Varren: Floor 2 is next, I hope you've prepared, the werewolf won't go down as easily as Stein did.",
        "Blaze: Werewolf?",
        "Varren: Yes.Keep up. You'll have to beat her if you want to proceed.",
        "Blaze: Why can't we just go to the lobby?",
        "Varren: We could but the doors are sealed.",
        "Varren: You didn't think you just could walk out the front doors did you? |Yes|No $Prompt",
        "Varren: Then you're dumber than I gave you credit for. $OptionA",
        "Varren: Then why would you want to go to the lobby?. $OptionB",
        "Blaze: Why do I need to defeat a werewolf?",
        "Varren: For her memento.The one you got from Stein is stronger than any ability i've ever seen.",
        "Varren: It's a long shot, but if you can get more of them, we might just be able to break the seal and leave this place.",
        "Blaze: So there is a-",
        "Varren: Don't get your hopes up.There's no guarantee that we'll even end up in the real world, and that's assuming the mementos are strong enough to break the seal.",
        "Varren: Here we are, floor 2. Don't come back until you've got that memento."
    };

    string[] TwinEncounter1 = new string[]
    {
        "Katie: Ooh, a newcomer!",
        "Kady: We haven't seen him here before, have we Katie?",
        "Katie: No, we haven't.",
        "Katie: Do you have candy?|No.|Who are you? $Prompt",
        "Blaze: No, I don't. $OptionA",
        "Katie: Awww. $OptionA",
        "Kady: I want Candy $OptionA",
        "Katie: I am Katie.",
        "Kady: And I am Kady.",
        "Kady: Who are you?|Blaze.|No one. $Prompt",
        "Blaze: My name is Blaze. $OptionA",
        "Blaze: No, I'm just passing by. $OptionB",
        "Blaze: Do you know where I could find a werewolf?",
        "Katie: A werewolf?",
        "Kady: What's that? |A big scary monster.|A person covered in fur. $Prompt",
        "Blaze: It's like a big scary monster with sharp teeth and lots of hair. $OptionA",
        "Kady: That's scary. $OptionA",
        "Katie: It sounds like a monster from one of the bedtime stories mom would read us $OptionA",
        "Blaze: It's like a person covered in fur with dog ears. $OptionB",
        "Katie: That sounds like mom. $OptionB",
        "Blaze: Where is your mom?",
        "Kady: She left to go take care of some things.",
        "Katie: She said that someone was going to come hang out with us until she came back.",
        "Kady: And that it wouldn't be dad.",
        "Katie: No.Dad is bad.",
        "Kady: Are you going to hang out with us?",
        "Katie: Of course he is. Mom said he would.",
        "Kady: Does this mean we can get candy? Will you get us some candy? |Yes.|No. $Prompt",
        "Blaze: Yes. $OptionA",
        "Twins: Yay! $OptionA",
        " : The twins look at you intently, their big innocent eyes tearing up. $OptionB",

        "Kady: Will you get us some candy? |Yes.|Yes. $Prompt $OptionB",     	//Hopefully this doesn't cause issues (if it does lmk - Avis)
        "Blaze: Where is the candy?",
        "Katie: The monsters have them, you have to get it from them.",
        "Kady: We will wait here for you to come back. Please don't take too long.",
        //Kills monsters for candy
    };

    //If you talk to the twins before you finish getting the candy
    string[] NotEnoughCandy = new string[]
    {
        "Katie: The monsters have the candy, you have to get it from them.",
        "Kady: Please hurry, we are getting hungry."
    };

    //When Blaze gets the first candy
    string[] TryCandy = new string[]
    {
        "Blaze: This monster dropped a candy, is this edible?",
        " : The candy is pink and blue in color, and has a sweet smell. A thin layer of emotional energy coats the surface.|Try it.|Don't try it. $Prompt.",
        " : The candy is extremely sweet, and dissolves in your mouth with a fizzing sensation. $OptionA",
        " : You put the candy into your inventory. $OptionB",
    };

    string[] EnoughCandy = new string[]
    {
        "Blaze: This should be enough candy for now.",
        "Blaze: I should head back.",
    };


    string[] TwinEncounter2 = new string[]
    {
        "Twins: Blaze!",
        "Kady: Did you bring candy |Yes |No $Prompt",
        "Twins: Yay! $OptionA",
        "Kady: He's much better than dad! $OptionA",
        "Katie: I can smell it, mom said lying is bad! $OptionB",
        "Kady: Yeah. Only meanies like dad lie. $OptionB",
        "Blaze: What happened to your dad?",
        "Kady: That meanie lied to us.",
        "Kady: He said he'd get us candy.",
        "Katie: And then he tried to take us away.",
        "Blaze: Take you away?",
        "Kady: Mhm, mom and him got into a big argument.",
        "Katie: And he said he was gonna get us candy, but mom told us to go get in the car.",
        "Kady: We left with mom and then dad and some other people caught up with us.",
        "Katie: The red lights right?",
        "Kady: I thought it was blue lights?",
        "Blaze: What happened after that?",
        "Katie: Dad was still mad at mom, some people took her away.",
        "Kady: And dad never got us our candy.",
        " : bzzt.",
        " : Katie pulls out her phone.",
        "Katie: It's mom, she says that she needs help.",
        "Kady: Mom is good, how do we help her?",
        "Katie: She left <color=##ff0000><b>some paperwork</b></color> around the barn.",
        "Blaze: The barn? (I thought this was a hotel?)",
        //The barn-city mention here is intentional
        "Katie: Yes, the city. The first one is at her lawyer's office.",
        "Kady: Please go get the evidence so we can be with our mom again.",
        "Blaze: You've got it.",
    };

    string[] Evidence1 = new string[]
    {
        " : You come across a piece of paper sitting on a desk.",
        "Blaze: This must be the first piece of evidence.",
        " : The paper is a newspaper cutout depicting a photo of the twins, a young woman and a young man.",
        " : The family is smiling while standing outside of a candy store, the man's eyes are blacked out with a marker. An article below the image reads \"Family wins millions following candy competition!",
        " : After an extremely intense candy eating competition hosted by Dulce Candy Co. New York's very own twin sisters, Katie and Kady Smith would come out victorious, winning the grand prize of a lifetime's supply of candy and 10% of the company's stock, valued at 1.5 Million Dollars!\"",
    };

    string[] Evidence2 = new string[]
    {
        " : You come across several pieces of paper sitting on a desk.",
        "Blaze: This looks like another piece of evidence.",
        " : The papers appear to be a collage of images depicting the family outside various houses and tourist destinations. As you flip through the pages, the images appear to be more faded and the family's smiles become less genuine.",
        " :The last image has been torn and taped back together, with the line separating Karren from the rest of her family.",
    };

    string[] Evidence3 = new string[]
    {
        " : You come across a piece of paper sitting on a desk",
        "Blaze: The last piece of evidence.",
        " : The paper appears to be a letter. It is unfinished, and the ink has bled in several spots where tears fell onto the paper. The recipients name is redacted, but the note appears to be apology from Karren to her husband and kids.",
    };


    //This probably won't be used.
    string[] TwinEncounter3 = new string[]
    {
        "Kady: Blaze!",
        "Katie: You're back!",
        "Kady: Did you get the paperwork?|Yes|No $Prompt",
        "Blaze: I did $OptionA",
        "Blaze: I'm not sure if this is all of it. $OptionB",
        "Katie: I think that's everything. $OptionB",
        "Kady: I knew we could count on you!",
        "Katie: Please hurry, our mom is just ahead."
    };

    #endregion Side Quest


    #region Boss Fight
    string[] KarenIntro =
    {
        "Judge: And for the crime of being a terrible mother and wife. The jury finds the defendant.",
        "Judge: ...",
        "Judge: GUILTY!!!",
        "Karren: No! I'm a good mother. I swear!",
        "Karren: That rat of a husband is the bad one! The kids should be with me!",
        "Karren: You can't do this!",
        "Karren: YOU!",
        "Blaze: Me?",
        "Karren: Yes you! You should have been here hours ago with the evidence!",
        "Karren: Where have you been?! This is all your fault!",
        "$Animate(Karen_Intro,Transform)",
        "$Timer(1)",
        "$CameraShake(2,1)",
        "$Timer(2.5)",
        "$FadeIn(255,255,255)",
        "$Kill(Karen_Intro)",
        "$Tele(KarenWerewolf,100.6,-97.29)",
        "$FadeOut(255,255,255)",
    };
    string[] KarenInter1 =
    {
        "$Animate(KarenWerewolf,StartIntermediate)",
        "$Animate(KarenWerewolf,Intermediate,True)",
        "Karren: If only you had gotten here earlier. Then I would still have my kids!|You wouldn't have lost them if you had treated them better|They are better off with their dad $Prompt",
        "Blaze: They only left because of how you treated them. $OptionA",
        "Karren: No! You're wrong! $OptionB",
        "Blaze: They're better off with their dad than they are with you. $OptionB",
        "Karren: Who are you to tell me what's best for my kids! $OptionB",
        //Enrage OptionB

        "$Animate(KarenWerewolf,Intermediate,False)",
    };

    string[] KarenInter2 =
    {
        "$Animate(KarenWerewolf,StartIntermediate)",
        "$Animate(KarenWerewolf,Intermediate,True)",
        "Karren: This is all your fault! None of this would have happened if you got here earlier! |You're right, I'm sorry|You can only blame yourself for this $Prompt",
        "Blaze: You're right $OptionA",
        "Blaze: I should have gotten here sooner. $OptionA",
        "Karren: So you understand. $OptionA",
        "Karren: Now die for your insubordination! $OptionA",
        //Enrage OptionA
        "Blaze: You have no one to blame but yourself. $OptionB",
        "Blaze: Blaming others won't help. $OptionB",
        "Karren:... $OptionB",

        "$Animate(KarenWerewolf,Intermediate,False)",
    };

    string[] KarenEnd =
    {
        "$FadeIn(255,255,255)",
        "$Tele(Karen_Final,102.44,-94.44)",
        "$Tele(PinkTwin,102.5,-99.758)",
        "$Tele(BlueTwin,101.96,-99.798)",
        "$Tele(Player,102.38,-95.84)",
        "$Kill(KarenWerewolf)",
        "$FadeOut(255,255,255)",
        "Karren: No. This can't be the end!",
        "Karren: I have to keep going. My kids!",
        "Karren: Katie... Kady...",
        "$Animate(Karen_Final,Stage2)",
        "$MoveTwo(PinkTwin,BlueTwin,102.73,-95.79,102,-95.79)",
        "$Animate(Karen_Final,Stage3)",
        "$MoveTwo(PinkTwin,BlueTwin,102.63,-94.50,102.23,-94.50)",
        "$Tele(PinkTwin,112,-96)",
        "$Tele(BlueTwin,112,-96)",
        "$Animate(Karen_Final,Stage4)",
        "$Timer(0.5)",
        "$Animate(Karen_Final,Stage5)",
        "Twins: We're here.",
        "Karren: Don't ever leave me.",
        "Twins: We won't.",
        "Blaze: They aren't real.",
        "Karren: They're as real as they need to be.",
        "Karren: I have my family again.",
        "Blaze: Maybe.",
        "Blaze: But the rest of your family is out there waiting for you.",
        "Karren: They all hate me.",
        "Blaze: Perhaps.",
        "Blaze: But you can show them that you've changed.",
        "Blaze: You've grown. I saw the letter. You want to make things right.",
        "Karren: And if they don't let me? |It beats sitting in this courtroom all day|You won't know unless you try $Prompt",
        "Karren: What if I just stay here, like this. $OptionA",
        "Katie: You know you can't. White hair will just reset the room. $OptionA",
        "Kady: You have to move on, mom. $OptionA",
        "Kady: Don't worry, we'll be cheering for you mom! $OptionB",
        "Katie: Yeah! And make sure you bring us candy! $OptionB",
        "Karren: I will. $OptionB",
        "Karren: Thank you, Blaze",
        "$Animate(Karen_Final,Stage6)",
        "$Tele(PinkTwin,102.63,-94.50)",
        "$Tele(BlueTwin,102.23,-94.50)",
        "$BossPoof(Karen_Final)",
        "$MoveTwo(PinkTwin,BlueTwin,102.47,-95.4,102.2,-95.4)",
        "Katie: Thank you for helping us Blaze",
        "Kady: It means alot to us",
        "Katie: I hope we see you on the other side!",
        "Kady: Please bring more candy.",
        "$BossPoof(PinkTwin)",
        "$BossPoof(BlueTwin)"
    };
    #endregion
    #endregion
}
