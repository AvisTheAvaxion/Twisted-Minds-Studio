
using Unity.VisualScripting;

public class Dialogue
{
    public string[] getDialogue(string name)
    {
        switch (name)
        {
            #region Floor 0
            case "WakeUpTutorial":
                return WakeUpTutorial;
            case "SameRoomAgain":
                return SameRoomAgain;
            case "InventoryTutorial":
                return InventoryTutorial;
            case "CombatTutorial":
                return CombatTutorial;
            case "FirstMonster":
                return FirstMonster;
            case "VarrenEncounter":
                return VarrenEncounter;
            #endregion
            #region Floor 1
            case "DrHarrisQuest":
                return DrHarrisQuest;
            case "DrHarrisQuest2":
                return DrHarrisQuest2;
            case "DrHarrisQuest3":
                return DrHarrisQuest3;
            case "DrHarrisQuest4":
                return DrHarrisQuest4;
            case "SteinIntro":
                return SteinIntro;
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
        SameRoomAgain,
        InventoryTutorial,
        CombatTutorial,
        FirstMonster,
        VarrenEncounter,
        #endregion
        #region Floor 1
        DrHarrisQuest,
        DrHarrisQuest2,
        DrHarrisQuest3,
        DrHarrisQuest4,
        SteinIntro,
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
        "$Move(Main Camera,3,0)",
        "$Emote|Protag|Surprised",
        "Protag: Ughhh",
        "Protag: What? Where am I?",
        "Protag: *looks around* Is this a... barn?",
        "Protag: Well, just standing here isn't going to accomplish anything",
        "System: Use <b>WASD</b> to move",
        "Protag: I wonder if there is anything useful around here?"
    };

    string[] SameRoomAgain = new string[]
    {
        //Protag ends up in same room after leaving
        "Protag: Huh? I could've sworn I just left this room."
    };

    string[] InventoryTutorial = new string[]
    {
        "Protag: Nice, a different room.",
        "System: Press the <b>'TAB'</b> key to open your inventory.",
        "System: On the left side of the inventory are tabs that can be opened by <b>Left Clicking</b>",
        "System: These three tabs hold, from top to bottom <b><color=#936A00>Weapons</color>, <color=#5B100A>Items</color>, and <color=#507830>Special Abilities</color></b>",
        "System: As you explore you will find many things lying around, when you want to <b>equip</b> something just click the item and then the UI button labeled <b>'Equip'</b>",
        "Protag: I don't see anyone here either. Maybe, I'll find someone in the next room."
    };

    string[] FirstMonster = new string[]
    {
        //Protag sees a monster in a room
        "Protag: Nope."
        //Protage leaves
    };

    string[] CombatTutorial = new string[]
    {
        //See's Slime Lady tries to target, Slime Lady realizes he has no idea what he's doing and teaches the player basic combat controls
        "Protag: A monster!",
        "Protag: This is a good time to try testing my dagger",
        //Protag charges at the cleaning lady, who dodges the attack
        "Monster: Oh dear, I don't think you're going to hit anything if you charge at them like that",
        "Protag: You can talk?",
        "Monster: Why wouldn't I be able to? My name is Samantha, and yours?[1Protag1][2What's it matter to you2]",
        "Samantha: Well it's certainly nice to meet you Protag, but please do try to refrain from attacking me for no reason",
        "Samantha: One finds it helps to make friends",
    };

    

    string[] VarrenEncounter = new string[]
    {
        /*
        "Varren: Play some toons",
        "$PlaySong|KarrenTheme",
        "Varren: Here i come",
        "$PlayEffect|DoorOpen",
        "coming: hai",
        */
        "$PlaySong|StienTheme",
        "$Move(Varren,0,-48.32)",
        "Varren: Oh? And who might you be?|No one|AHHHHH$Prompt",
        "Protag: Who are you? Where did you come from?$OptionA",
        "Varren: Me? I'm a... resident of this lovely establishment.$OptionA",
        "Protag: A GHOST!!!$OptionB",
        "Varren: Calm down kid, I'm not a ghost.$OptionB",
        "Varren: I'm just another person trapped here like you.$OptionB",
        "$Emote|Protag|Angry",
        //Line above is a test line
        "Protag: Where are we?",
        "Varren: Hell of course! You died! Where else did you think you would end up?",
        "Protag: What do you mean? Isn't this a barn?",
        "$Emote|Protag|Surprised",
        //Line above is a test line
        "Varren: It's a hotel, and it's where you'll be spending the rest of your days, trapped here for eternity because of all of the terrible things you've done.|Are you pulling my leg?|What terrible things?$Prompt",
        "Protag: I think I'd remember dying.$OptionA",
        "Varren: Murder, theft, being an all around terrible person.$OptionB",
        "Varren: Whatever horrible deeds you've commited, you're now stuck here as punishment.$OptionB",
        "Protag: But why would I be sent to a hotel?$OptionB",
        "Varren: Haven't you noticed how this place seems off? The rooms shifting, the lack of windows, the monsters?",
        "Protag: Yes but-",
        "Varren: Welcome to hell. You died. You're stuck here for eternity to be forever haunted by those... things. There is no exit, no escape. The atrocities you've committed have condemned you here for eternity.",
        "Protag: But there has to be some kind of a mistake. I don't remember committing any atrocities or dying, maybe if I look around for long enough then I can find an exit, or the person who trapped us here?",
        "Varren: You're going to fail. I've searched every nook and cranny of this place. I'm sorry, but there's no way out kid.",
        "Protag: Maybe you missed a spot?",
        "Varren: No.",
        "Protag: Maybe there is a hidden room?",
        "$Pause",
        "$Emote|Varren|Angry",
        "Varren: <b>NO!!!</b>",
        "Varren: <i>(Sigh)</i>",
        "Varren: <i>(stubborn little ****)</i>",
        "Varren: There aren't any hidden rooms",
        "Varren: There is no one watching us",
        "Varren: And there certantly aren't any exits.",
        "Varren: Once you arrive, there is no way to leave. You're stuck here.",
        "Varren: Forever.",
        "Varren: <b>And Ever.</b>",
        "Varren: <b><u>And Ever...</b></u>",
        "Varren: ...",
        "$Emote|Varren|Neutral",
        "$Resume",
        "Varren: Since we're stuck here why don't you go ahead and tell me what it is you've done to end up here?",
        "Protag: I really don't remember. There has to be some sort of mix-up here. I'm going to find whoever put us here and prove that there is a way out.",
        "Varren: There's no convincing you is there kid?|Well just giving up and just sitting here isn't going to accomplish anything.|There's no way I can stay in this stinky place$Prompt",
        "Varren: Your stubbornness is quite amusing, take this",
        "$Pause",
        "$GiveWeapon(Dagger)",
        "System: You got a Dagger!",
        "$Resume",
        "Varren: You can't die here, but you still can feel pain. Let's see how far you get before you give up.",
         //"Varren: (disappears into smoke)",
        "Varren: Cya later kiddo",
        "$Move(Varren,-4,-48.32)",
        "Protag: Well, I guess I should get moving.",
        "$Kill(Varren)",
        "$SetQuest(0,0)"
    };
    #endregion
    #region Floor 1 Dialog
    #region Side Quest
    //(The player must defeat enemies using the scalpel, as enemies take bleeding DOT, the blood bag fills up with blood,
    //and once the bag is 100% full, the player can meet with the doctor again)

    string[] DrHarrisQuest = new string[]
    {
        //The player enters the outpatient room, Doctor Dr. Harris is standing inside with a clipboard
        "Dr. Harris: There you are! You must be my new assistant!",
        "Protag: Assistant?",
        "Dr. Harris: Yes. Come along now, we have a lot to do. We need to get everything ready for the surgery.",
        "Protag: Surgery?",
        "Dr. Harris: Did the nurse not tell you anything? I'm afraid the good doctor's son won't make it if we aren't on schedule.",
        "Dr. Harris: Let's see here, first off we need bandages and antiseptic, I'll handle that, you just collect some rootweed and numblumbitantrilia.",
        "Protag: Rootfeet and numblubi... what?",
        "Dr. Harris: Rootweed, and.. forget it. You should be able to find some from defeating monsters.",
        "Dr. Harris: Come find me when you've got at least 20 pieces of each, don't keep me waiting.",
        "Dr. Harris: Todaloo!",
        //Dr. Harris walks off into a different room
    };

    /*
    //Player after killing several enemies and collecting only emotional energy
    4|Protag: None of these monsters seem to be leaving anything besides this goop. Is this what he wants?


    //Player after killing several more enemies
    5|Protag: Still nothing but goop.
    */

    string[] DrHarrisQuest2 = new string[]
    {
        //After collecting enough emotional energy (the goop), the player walks into a room where the doctor is
        "Dr. Harris: Ah! Just in time. I was wondering what was taking you so long.",
        "Protag: I couldn't find anything but this goop.",
        "Dr. Harris: Such a hard worker too! This should be more than enough. Keep this up and I'll be sure to let the doctor know to give you a bonus.",
        " :*As you hand Dr. Harris the goop, it imidiately changes into a bundle of dried plant stems and a collection of purple flower heads*",
        "Protag: How did you do that?",
        "Dr. Harris: Do what?",
        "Protag: The goop! It turned into what we needed!",
        "Dr. Harris: Goop? I appreciate your hard work, but please make sure you are getting enough rest.",
        "Dr. Harris: If you would like I can talk to the doctor about prescribing you some sleep medications. We can't have you making mistakes on the job now, can we?",
        "Dr. Harris: The next thing on the list is rimblenut, I'll do that one, and O- Blood.",
        "Dr. Harris: For this one, you'll need something to collect the blood in, so let's see here.",
        //Dr. Harris digs through his bag, which holds a collection of perfectly normal medical instruments and the most bizarre of ingredients before pulling out a scalpel)
        "Dr. Harris: Ah, here we are. This should make it easier. When you encounter the monsters, use this scalpel. Severing the arteries should make collecting the blood we need a breeze.",
        "$Give(Scapel)",
        "$Give(BloodBag)",
        "Dr. Harris: As usual, come and find me when you're done.",
        "Protag: Wait!",
        "Dr. Harris: Sorry, no time for questions, we're on a tight schedule here.",
        "Dr. Harris: Todaloo!",
        //(Dr. Harris leaves the room)
    };

    string[] DrHarrisQuest3 = new string[]
    {
        //(Protag enters a room with Dr. Dr. Harris)
        "Dr. Harris: Ah just on time, I knew we could count on you!",
        "(Protag hands the doctor the blood bag and the scalpel)",
        "Dr. Harris: You can keep the scalpel, never know when you might need it",
        "Protag: So, what happened to the doctor's son?",
        "Dr. Harris: It was tragic, really. The poor boy only had 30 yards left to go, when he went down hard.",
        "Protag: What do you mean?",
        "Dr. Harris: A broken leg, a sprained wrist, and a concussion, just one week before the finals.",
        "Dr. Harris: But no matter, once we've got this cure finished he'll be back up and at it in no time, even better than he was before.",
        "Dr. Harris: Now for the final ingredient, well not really an ingredient, but we need his jersey. You should be able to find it in the good doctor's office.",
        "Dr. Harris: In the meantime, I shall start preparing the ingredients so that the doctor can do his work.",
        "Dr. Harris: Todaloo!",
    };

    //    (Protag finds his way to the doctor's office (outpatient tileset room) inside they can grab the jersey
    //    and possibly have a file with a list of some of the doctor's other patients including the experiments he did on them)

    string[] DrHarrisQuest4 = new string[]
    {
        //(Protag finds Dr Dr. Harris after grabbing the jersey)
        "Dr. Harris: Ah, the jersey.",
        "Dr. Harris: This should be all for now, I've prepared the ingredients for the doctor and he should be able to cure his son now. Thank you for all of your help, I shall let the doctor how helpful you've been.",
        "Dr. Harris: Todaloo!",
        //Boss Room should now be accessiable
    };
    #endregion
    #region Boss Fight
    string[] SteinIntro = new string[]
    {
        "Dr. Frank N. Stein: ...",
        "Dr. Frank N. Stein: Ah! You're here. And not a moment too soon.",
        "Dr. Frank N. Stein: You have my thanks. Dr. Harris let me know how usefull you've been.",
        "Dr. Frank N. Stein: Thanks to your efforts, the surgery was a sucess!",
        "Dr. Frank N. Stein: Behold the results of my work! He's even better than before!",
        "$ToggleUI(False)",
        "$Animate(FNSMonster_IntroCutscene,Electricute,true)",
        "$Timer(3)",
        "$FadeIn(255,255,255)",
        "$Kill(FNSMonster_IntroCutscene)",
        "$Tele(FrankNSteinMonster,18.31,-47.401)",
        "$Tele(FrankNStein,17.535,-45.04)",
        "$FadeOut(255,255,255)",
        "$ToggleUI(True)",
        "Protag: Eww. Gross.",
        "Dr. Frank N. Stein: DIE!",
    };
    #endregion
    #endregion
}
