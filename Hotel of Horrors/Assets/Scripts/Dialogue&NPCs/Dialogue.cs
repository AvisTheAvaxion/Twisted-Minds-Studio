
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
            case "SameRoomAgain":
                return SameRoomAgain;
            case "InventoryTutorial":
                return InventoryTutorial;
            case "MindRoom":
                return MindRoom;
            case "CombatTutorial":
                return CombatTutorial;
            case "FirstMonster":
                return FirstMonster;
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
            case "DrHarrisQuestIntro":
                return DrHarrisQuestIntro;
            case "DrHarrisQuest1":
                return DrHarrisQuest1;
            case "DrHarrisQuest2":
                return DrHarrisQuest2;
            case "DrHarrisQuest3":
                return DrHarrisQuest3;
            case "DrHarrisQuest4":
                return DrHarrisQuest4;
            case "DrHarrisQuest5":
                return DrHarrisQuest5;
            case "SteinIntro":
                return SteinIntro;
            case "SteinScene1":
                return SteinScene1;
            case "SteinEnd":
                return SteinEnd;
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
        MindRoom,
        CombatTutorial,
        FirstMonster,
        VarrenEncounter,
        #endregion
        #region Floor 1
        DrSteinBossDoor,
        DrSteinBossDoor1,
        DrSteinBossDoor2,
        DrHarrisQuestIntro,
        DrHarrisQuest1,
        DrHarrisQuest2,
        DrHarrisQuest3,
        DrHarrisQuest4,
        DrHarrisQuest5,
        SteinIntro,
        SteinScene1,
        SteinEnd,
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
        "$Emote|Protag|Surprised",
        "Blaze: Ughhh",
        "Blaze: What? Where am I?",
        "Blaze: *looks around* Is this a... barn?",
        "Blaze: Well, just standing here isn't going to accomplish anything",
        " : Use <b>WASD</b> to move",
        "Blaze: I wonder if there is anything useful around here?"
    };

    string[] SameRoomAgain = new string[]
    {
        //Blaze ends up in same room after leaving
        "Blaze: Huh? I could've sworn I just left this room."
    };

    string[] InventoryTutorial = new string[]
    {
        "Blaze: Nice, a different room.",
        " : Press the <b>'TAB'</b> key to open your inventory.",
        " : On the left side of the inventory are tabs that can be opened by <b>Left Clicking</b>",
        " : These three tabs hold, from top to bottom <b><color=#936A00>Weapons</color>, <color=#5B100A>Items</color>, and <color=#507830>Special Abilities</color></b>",
        " : As you explore you will find many things lying around, when you want to <b>equip</b> something just click the item and then the UI button labeled <b>'Equip'</b>",
        " : I don't see anyone here either. Maybe, I'll find someone in the next room.",
        " : This dagger should be useful for defending myself, I should keep it out, just in case",
        " : Press tab to open your inventory.",
        " : Use the headers on the left side to equip the dagger.",
        " : You can also equip items and view abilities in the inventory"
    };

    string[] FirstMonster = new string[]
    {
        //Blaze sees a monster in a room
        "Blaze: Nope."
        //Blazee leaves
    };

    string[] MindRoom = new string[]
    {
        "Blaze: This room.",
        "Blaze: It feels... Strange.",
        " : You feel a strange pull towards the chair.",
        " : Maybe something will happen if you interact with it."
    };

    string[] CombatTutorial = new string[]
    {
        //See's Slime Lady tries to target, Slime Lady realizes he has no idea what he's doing and teaches the player basic combat controls
        "$Move(Player,20.265,2.898)",
        "Blaze: A monster!",
        "Blaze: This is a good time to try testing my dagger",
        //Blaze charges at the cleaning lady, who dodges the attack
        "$Move(Player,20.265,2.282)",
        "$Tele(SlimeLady,20.26,3.09)",
        "Monster: Oh dear, I don't think you're going to hit anything if you charge at them like that",
        "Blaze: You can talk?",
        "Monster: Why wouldn't I be able to? My name is Samantha, and yours?|Blaze|What's it matter to you|$Prompt",
        "Samantha: Well it's certainly nice to meet you, but please do try to refrain from attacking me for no reason$OptionA",
        "Samantha: One finds it helps to make friends$OptionB",
        "Samantha: Now, before you get going, perhaps I should show you how to properly fight.",
        "Samantha: To start, just try attacking with the dagger",
        " : Press left-click repeatadly to complete an attack combo",
        "Samantha: Good!",
        "Samantha: Now, let's try using your dash to quickly move about",
        " : Press shift to dash",
        "Samantha: Excellent, dashing can also be used to avoid attacks and quickly get behind enemies.",
        "Samantha: Now, let's test out that ability of yours.",
        " : Press E to use your ability.",
        "Samantha: Amazing! If you would like to change abilities or upgrade weapons, you can do so in the mind room.",
        "Samantha: Well, i've got duties to attend to, take care! And don't let the Vampire scare you."
    };

    

    string[] VarrenEncounter = new string[]
    {
        "$PlaySong|StienTheme",
        "$Move(Varren,-15.75,-14.29)",
        "Varren: Oh? And who might you be?|No one|AHHHHH$Prompt",
        "Blaze: Who are you? Where did you come from? $OptionA",
        "Varren: Me? I'm a... resident of this lovely establishment. $OptionA",
        "Blaze: A GHOST!!! $OptionB",
        "Varren: Calm down kid, I'm not a ghost. $OptionB",
        "Varren: I'm just another person trapped here like you. $OptionB",
        "$Emote|Protag|Angry",
        //Line above is a test line
        "Blaze: Where are we?",
        "Varren: Hell of course! You died! Where else did you think you would end up?",
        "Blaze: What do you mean? Isn't this a barn?",
        "$Emote|Protag|Surprised",
        //Line above is a test line
        "Varren: It's a hotel, and it's where you'll be spending the rest of your days, trapped here for eternity because of all of the terrible things you've done.|Are you pulling my leg?|What terrible things?$Prompt",
        "Blaze: I think I'd remember dying.$OptionA",
        "Varren: Murder, theft, being an all around terrible person.$OptionB",
        "Varren: Whatever horrible deeds you've commited, you're now stuck here as punishment.$OptionB",
        "Blaze: But why would I be sent to a hotel?$OptionB",
        "Varren: Haven't you noticed how this place seems off? The rooms shifting, the lack of windows, the monsters?",
        "Blaze: Yes but-",
        "Varren: Welcome to hell. You died. You're stuck here for eternity to be forever haunted by those... things. There is no exit, no escape. The atrocities you've committed have condemned you here for eternity.",
        "Blaze: But there has to be some kind of a mistake. I don't remember committing any atrocities or dying, maybe if I look around for long enough then I can find an exit, or the person who trapped us here?",
        "Varren: You're going to fail. I've searched every nook and cranny of this place. I'm sorry, but there's no way out kid.",
        "Blaze: Maybe you missed a spot?",
        "Varren: No.",
        "Blaze: Maybe there is a hidden room?",
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
        "Blaze: I really don't remember. There has to be some sort of mix-up here. I'm going to find whoever put us here and prove that there is a way out.",
        "Varren: There's no convincing you is there kid?|Well just giving up and just sitting here isn't going to accomplish anything.|There's no way I can stay in this stinky place$Prompt",
        "Varren: Your stubbornness is quite amusing, take this",
        "$Pause",
        "$GiveWeapon(Dagger)",
        " : You got a Dagger!",
        "$Resume",
        "Varren: You can't die here, but you still can feel pain. Let's see how far you get before you give up.",
         //"Varren: (disappears into smoke)",
        "Varren: Cya later kiddo",
        "$Move(Varren,-11.57,-14.3)",
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
        "Dr. Stein: I'll save you this time Victor"
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

    string[] DrHarrisQuestIntro = new string[]
    {
        //The player enters the outpatient room, Doctor Dr. Harris is standing inside with a clipboard
        "Dr. Harris: There you are! You must be my new assistant!",
        "Blaze: Assistant?",
        "Dr. Harris: Yes. Come along now, we have a lot to do. We need to get everything ready for the surgery.",
        "Blaze: Surgery?",
        "Dr. Harris: Did the nurse not tell you anything? I'm afraid the good doctor's son won't make it if we aren't on schedule.",
        "Blaze: His son? What happened to him?",
        "Dr. Harris: A horrendous accident! The poor lad took a hard hid during the semifinals.",
        "Blaze: The finals?",
        "Dr. Harris: Indeed, but don't fret my boy. I am fully confident in the abilities of Dr. Stein, and that's where we come in.",
        "Dr. Harris: Now let's see here, first off we need bandages and antiseptic, I'll handle that, you just collect some rootweed and numblumbitantrilia.",
        "Blaze: Rootfeet and numblubi... what?",
        "Dr. Harris: Rootweed, and.. forget it. You should be able to find some from defeating monsters.",
        "Dr. Harris: Come find me when you've got at least 20 pieces of each, don't keep me waiting.",
        "Dr. Harris: Todaloo!",
        //Dr. Harris walks off into a different room
        //Collect quest where the player needs to get emotional energy
    };

    //Once the player has enough emotional energy
    string[] DrHarrisQuest1 = new string[]
    {
        "$TimeScale(0)",
        "Blaze: I've defeated all of these monsters but found nothing but goop.",
        "Blaze: Hopefully this is what he's talking about.",
        "$TimeScale(1)",
    };

    string[] DrHarrisQuest2 = new string[]
    {
        //After collecting enough emotional energy (the goop), the player walks into a room where the doctor is
        "Dr. Harris: Ah! Just in time. I was wondering what was taking you so long.",
        "Blaze: I couldn't find anything but this goop.",
        "Dr. Harris: Such a hard worker too! This should be more than enough. Keep this up and I'll be sure to put in a good word with the doctor.",
        " : *As you hand Dr. Harris the goop, it imidiately changes into a bundle of dried plant stems and a collection of purple flower heads*",
        "Blaze: How did you do that?",
        "Dr. Harris: Do what?",
        "Blaze: The goop! It turned into what we needed!",
        "Dr. Harris: Goop? I appreciate your hard work, but please do be sure you are getting enough rest.",
        "Dr. Harris: If you would like I can talk to the doctor about prescribing you some sleep medications.",
        "Dr. Harris: We can't have you making mistakes on the job now, can we?|No Thanks|That would be nice|$Promtpt",
        "Dr. Harris: Suit yourself.$OptionA",
        "Dr. Harris: I will be sure to get you some.$OptionB",
        "Dr. Harris: The next thing on the list is rimblenut, I'll do that one, and O- Blood.",
        "Dr. Harris: For this one, you'll need something to collect the blood in, so let's see here.",
        " : Dr. Harris digs through his bag, which holds a collection of perfectly normal medical instruments...",
        " : and the most bizarre of ingredients...",
        " : before pulling out a scalpel.",
        "Dr. Harris: Ah, this should make it easier. When you encounter the monsters, use this scalpel.",
        "Dr. Harris: Severing the arteries should make collecting the blood we need a breeze.",
        "Dr. Harris: As usual, come and find me when you're done.",
        "Blaze: Why do we need all this bood",
        "Dr. Harris: Victor, the good Doctors son, lost a lot of blood after the accident",
        "Dr. Harris: We will need to replinish it if he hopes to play in the finals.",
        "Dr. Harris: That's enough questions for now. We must hurry if we want to save our star athlete in time!",
        "Dr. Harris: Todaloo!",
        //(Dr. Harris leaves the room)
        //Player must kill enemies using the scapel
    };

    string[] DrHarrisQuest3 = new string[]
    {
        "$TimeScale(0)",
        "Blaze: The blood bag is full now",
        "Blaze: I should head back",
        "$TimeScale(1)",
    };

    string[] DrHarrisQuest4 = new string[]
    {
        //(Blaze enters a room with Dr. Dr. Harris)
        "Dr. Harris: Ah good, you're here.",
        "Blaze: Is everything okay?",
        "Dr. Harris: I'm afraid the good doctor's son isn't doing as well as we hoped.",
        "Blaze: Did the rootfeet and numblistuff not work?",
        "Dr. Harris: The treatment wasn't as effective as we had hoped.",
        "Dr. Harris: In fact, I was only barely able to deliver the ingredients before the patient died.",
        "Blaze: He died?",
        "Dr. Harris: ...",
        "Dr. Harris: Nonesense. Not if I have anything to say about it.",
        "Dr. Harris: I'm sure the good doctor has everything under control.",
        "Dr. Harris: You've been a great help. I'll be sure to inform the doctor of your exceptional performance.",
        "Dr. Harris: However, there is one thing that remains.",
        "Dr. Harris: Please fetch our young patents jersy from the good doctors office.",
        "Dr. Harris: He will need it for his big day!",
        "Dr. Harris: I'll be waiting for you in the operatin room.",
        "Blaze: Wait, your scapel",
        "Dr. Harris: Hold onto it my dear boy. Our job is not yet complete",
        "Dr. Harris: Todaloo!",
        //The player needs to go to Dr. Stein's office and grab the jersey
    };

    //    (Blaze finds his way to the doctor's office (outpatient tileset room) inside they can grab the jersey
    //    and possibly have a file with a list of some of the doctor's other patients including the experiments he did on them)

    string[] DrHarrisQuest5 = new string[]
    {
        //(Blaze finds the jersy
        "Blaze: This looks like the jersy.",
        "Phone: Bzzz",
        "Blaze: Wait, a phone, and it has service!",
        "Blaze: That creepy vampire was wrong! I can call someone and get out of here",
        " : You try to call home, but instead of the phone rining, a pre-recorded message plays",
        "Victor: Dad, I can't wait for you to see me at the semi finals!",
        "Dr. Stein: Yeah...",
        "Victor: It sucks you missed the our game against Panthers, we absolutely crushed them.",
        "Dr. Stein: I'm sorry I missed it Victor.",
        "Victor: It's okay. I know you're really busy with work, plus you promised you'd make it to this one!",
        "Victor: Especially with James out, I'll be the lead running back, and coach told me several scouts would be coming!",
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
        " : Looking around, you can see several copies of Victors medicine chart.",
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
        "Dr. Frank N. Stein: ...",
        "Dr. Frank N. Stein: Ah! You're here. And not a moment too soon.",
        "Dr. Frank N. Stein: You have my thanks. Dr. Harris let me know how usefull you've been.",
        "Dr. Frank N. Stein: Thanks to your efforts, the surgery was a sucess!",
        "Blaze: Wait, it was?",
        "Dr. Frank N. Stein: Of course it was. I, the great Dr. Frank N. Stein, have sucessfully cured death!",
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
        "$Tele(FrankNStein,17.535,-45.04)",
        "$Kill(FrankNStein_Intro)",
        "$FadeOut(255,255,255)",
        "$ToggleUI(True)",
        "Blaze: Eww. Gross.",
        "Dr. Frank N. Stein: How dare you insult my son!",
        "Dr. Frank N. Stein: <b>DIE!</b>",
    };

    string[] SteinScene1 = new string[]
    {
        "$Animate(FrankNSteinMonster,Intermediate,True)",
        "Blaze: You have to accept your son is dead.",
        "Dr. Frank N. Stein: No! He's right in front of me!",
        "Dr. Frank N. Stein: Can't you see.|He wouldn't have wanted this|That's not him|$Prompt",
        "Dr. Frank N. Stein: No... You're wrong... $OptionA",
        "Dr. Frank N. Stein: Nonsense! $OptionB",
        "$Animate(FrankNSteinMonster,Intermediate,False)",
        //"Need to have him enrage if option b is chosen
    };

    string[] SteinEnd = new string[]
    {
        "$ToggleUI(False)",
        "$FadeIn(255,255,255)",
        "$Kill(FrankNSteinMonster)",
        "$Tele(FNSMonster_EndCutscene,20.328,-50.425)",
        "$Tele(Player,18.74,-50.49)",
        "$Tele(FrankNStein_Final,19.6,-47.46)",
        "$FadeOut(255,255,255)",
        "$MoveViaLerp(Main Camera,19.592,-50.49)",
        "$MoveViaLerp(FrankNStein_Final,20.18,-50.47)",
        "$Kill(FrankNStein_Final)",
        "$Animate(FNSMonster_EndCutscene,Stage2)",
        " : Talking Goes Here <b>AVIS PLEASE HELP</b>",
        "$Animate(FNSMonster_EndCutscene,Stage3)",
        "$BossPoof(FNSMonster_EndCutscene)",
        "$UnlockFromBoss",
        "$ToggleUI(False)",
        "$Timer(6)",
        "$Kill(FNSMonster_EndCutscene)",
        "$Tele(Varren,18.39,-52.89)",
        "$Move(Varren,20.28,-50.69)",
        "Varren: Dude Mementos are sick!!!",
        "Blaze: IKR!!!",
        "$Move(Varren,18.39,-52.89)",
        "$Kill(Varren)",
        "$MoveViaLerp(Main Camera,18.74,-50.49)",
    };
    #endregion
    #endregion
}
