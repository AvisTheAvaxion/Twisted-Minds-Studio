
public class Dialogue
{
    public string[] getDialogue(string name)
    {
        switch (name)
        {
            case "WakeUpTutorial":
                return WakeUpTutorial;
            case "SameRoomAgain":
                return SameRoomAgain;
            case "FirstMonster":
                return FirstMonster;
            case "VarrenEncounter":
                return VarrenEncounter;
            case "CombatEncounter":
                return CombatTutorial;
            case "DrHarrisQuest":
                return DrHarrisQuest;
            case "SteinIntro":
                return SteinIntro;

        }
        return null;
    }

    public enum Dialog
    {
        None,
        WakeUpTutorial,
        SameRoomAgain,
        FirstMonster,
        VarrenEncounter,
        CombatTutorial,
        DrHarrisQuest,
        DrHarrisQuest2,
        DrHarrisQuest3,
        DrHarrisQuest4,
        DrHarrisQuest5,
        SteinIntro,
    }

    #region Floor 1 Dialog
    #region Tutorial
    string[] WakeUpTutorial = new string[]
    {
        "Protag: Ughhh",
        "Protag: What? Where am I?",
        "Protag: *looks around* Is this a... barn?",
        "Protag: Well, just standing here isn't going to accomplish anything",
        "System: Use WASD to move",
        "Protag: I wonder if there is anything useful around here?",
        "System: Press F to interact with drawers and doors",
    };

    string[] SameRoomAgain = new string[]
    {
        //Protag ends up in same room after leaving
        "Protag: Huh? I could've sworn I just left this room."
    };

    string[] FirstMonster = new string[]
    {
        //Protag sees a monster in a room
        "Protag: Nope."
        //Protage leaves
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
    #endregion
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
        "As you hand Dr. Harris the goop, it imidiately changes into a bundle of dried plant stems and a collection of purple flower heads",
        "Protag: How did you do that?",
        "Dr. Harris: Do what?",
        "Protag: The goop! It turned into what we needed!",
        "Dr. Harris: Goop? I appreciate your hard work, but please make sure you are getting enough rest.",
        "Dr. Harris: If you would like I can talk to the doctor about prescribing you some sleep medications. We can't have you making mistakes on the job now, can we?",
        "Dr. Harris: The next thing on the list is rimblenut, I'll do that one, and O- Blood.",
        "Dr. Harris: For this one, you'll need something to collect the blood in, so let's see here.",
        //Dr. Harris digs through his bag, which holds a collection of perfectly normal medical instruments and the most bizarre of ingredients before pulling out a scalpel)

    };

    string[] DrHarrisQuest3 = new string[]
    {
        "Dr. Harris: Ah, here we are. This should make it easier. When you encounter the monsters, use this scalpel. Severing the arteries should make collecting the blood we need a breeze.",
        "$Give(Scapel)",
        "$Give(BloodBag)",
        "Dr. Harris: As usual, come and find me when you're done.",
        "Protag: Wait!",
        "Dr. Harris: Sorry, no time for questions, we're on a tight schedule here.",
        "Dr. Harris: Todaloo!",
        //(Dr. Harris leaves the room)
    };

    string[] DrHarrisQuest4 = new string[]
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

    string[] DrHarrisQuest5 = new string[]
    {
        //(Protag finds Dr Dr. Harris after grabbing the jersey)
        "Dr. Harris: Ah, the jersey.",
        "Dr. Harris: This should be all for now, I've prepared the ingredients for the doctor and he should be able to cure his son now. Thank you for all of your help, I shall let the doctor how helpful you've been.",
        "Dr. Harris: Todaloo!",
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
        "Protag: Eww. Gross.",
        "Dr. Frank N. Stein: DIE!",
    };
    #endregion

    #endregion
}
