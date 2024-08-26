using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Dialogue
{
    public string[] getDialogue(string name)
    {
        switch (name)
        {
            case "WakeUpTutorial":
                return WakeUpTutorial;

            case "VarrenEncounter":
                return VarrenEncounter;
                
        }
        return null;
    }



    string[] WakeUpTutorial = new string[]
    {
        "Protag: Ughhh",
        "Protag: What? Where am I?",
        "Protag: *looks around* Is this a... barn?",
        "Protag: Well, just standing here isn't going to accomplish anything",
        "Use WSAD to move",
        "Protag: I wonder if there is anything useful around here?",
        "System: Press F to interact with drawers and doors",
    };

    /*


//Protag leaves room A and ends up in room A again)
0|Protag: Huh? I could've sworn I just left this room.


//Protag sees a monster in a room
0|Protag: Nope.
//Protage leaves

//- Varren first floor encounter

1#|Varren: Oh? And who might you be?[1No one1][2AHHHHH!]
1a|Protag: Who are you? Where did you come from?
1a|Varren: Me? I'm a... resident of this lovely establishment
1b|Protag: A GHOST!!!
1b|Varren: Calm down kid, I'm not a ghost
1b|Varren: I'm just another person trapped here like you
1|Protag: Where are we?
1|Varren: Hell of course! You died! Where else did you think you would end up?
1|Protag: What do you mean? Isn't this a barn?
1#|Varren: It's a hotel, and it's where you'll be spending the rest of your days, trapped here for eternity because of all of the terrible things you've done.[1Are you pulling my leg?1][2What terrible things?2]
1a|Protag: I think I'd remember dying.
1b|Varren: Murder, theft, being an all around terrible person
1b|Varren: Whatever horrible deeds you've commited, you're now stuck here as punishment
1b|Protag: But why would I be sent to a hotel?
1|Varren: Haven't you noticed how this place seems off? The rooms shifting, the lack of windows, the monsters?
1|Protag: Yes but-
1|Varren: Welcome to hell. You died. You're stuck here for eternity to be forever haunted by those... things. There is no exit, no escape. The atrocities you've committed have condemned you here for eternity.
1|Protag: But there has to be some kind of a mistake. I don't remember committing any atrocities or dying, maybe if I look around for long enough then I can find an exit, or the person who trapped us here?
1|Varren: You're going to fail. I've searched every nook and cranny of this place. I'm sorry, but there's no way out kid.
1|Protag: Maybe you missed a spot?
1|Varren: No.
1|Protag: Maybe there is a hidden room?
1|Varren: NO!!!
1|Varren: $i("Sigh")
1|Varren: $i("stubborn little ****")
1|Varren: There aren't any hidden rooms
1|Varren: There is no one watching us
1|Varren: And there certantly aren't any exits.
1|Varren: Once you arrive, there is no way to leave. You're stuck here.
1|Varren: Forever.
1|Varren: And Ever.
1|Varren: And Ever.
1|Varren: Varren:...
1|Varren: Since we're stuck here why don't you go ahead and tell me what it is you've done to end up here?
1|Protag: I really don't remember. There has to be some sort of mix-up here. I'm going to find whoever put us here and prove that there is a way out.
1#|Varren: There's no convincing you is there kid?[1Well just giving up and just sitting here isn't going to accomplish anything.1][2There's no way I can stay in this stinky place2]
1|Varren: Your stubbornness is quite amusing, take this
1|Varren:$give("Dagger")
1| Varren: You can't die here, but you still can feel pain. Let's see how far you get before you give up.
(Varren disappears into smoke)
Protag: Well, I guess I should get moving.



//- Combat tutorial

//See's Slime Lady tries to target, Slime Lady realizes he has no idea what he's doing and teaches the player basic combat controls
2|Protag: A monster!
2|Protag: This is a good time to try testing my dagger
//Protag charges at the cleaning lady, who dodges the attack
2|Monster: Oh dear, I don't think you're going to hit anything if you charge at them like that
2|Protag: You can talk?
2#|Monster: Why wouldn't I be able to? My name is Samantha, and yours?[1Protag1][2What's it matter to you2]
2a|Samantha: Well it's certainly nice to meet you Protag, but please do try to refrain from attacking me for no reason
2b|Samantha: One finds it helps to make friends





//- Side Quest
//The player enters the outpatient room, Doctor Dr. Harris is standing inside with a clipboard
3|Dr. Harris: There you are! You must be my new assistant!
3|Protag: Assistant?
3|Dr. Harris: Yes. Come along now, we have a lot to do. We need to get everything ready for the surgery.
3|Protag: Surgery?
3|Dr. Harris: Did the nurse not tell you anything? I'm afraid the good doctor's son won't make it if we aren't on schedule.
3|Dr. Harris: Let's see here, first off we need bandages and antiseptic, I'll handle that, you just collect some rootweed and numblumbitantrilia.
3|Protag: Rootfeet and numblubi... what?
3|Dr. Harris: Rootweed, and.. forget it. You should be able to find some from defeating monsters.
3|Dr. Harris: Come find me when you've got at least 20 pieces of each, don't keep me waiting.
3|Dr. Harris: Todaloo!
//Dr. Harris walks off into a different room


//Player after killing several enemies and collecting only emotional energy
4|Protag: None of these monsters seem to be leaving anything besides this goop. Is this what he wants?


//Player after killing several more enemies
5|Protag: Still nothing but goop.


//After collecting enough emotional energy (the goop), the player walks into a room where the doctor is
6|Dr. Harris: Ah! Just in time. I was wondering what was taking you so long.
6|Protag: I couldn't find anything but this goop.
6|Dr. Harris: Such a hard worker too! This should be more than enough. Keep this up and I'll be sure to let the doctor know to give you a bonus.
6|As you hand Dr. Harris the goop, it imidiately changes into a bundle of dried plant stems and a collection of purple flower heads
6|Protag: How did you do that?
6|Dr. Harris: Do what?
6|Protag: The goop! It turned into what we needed!
6|Dr. Harris: Goop? I appreciate your hard work, but please make sure you are getting enough rest.
6|Dr. Harris: If you would like I can talk to the doctor about prescribing you some sleep medications. We can't have you making mistakes on the job now, can we?
6|Dr. Harris: The next thing on the list is rimblenut, I'll do that one, and O- Blood.
6|Dr. Harris: For this one, you'll need something to collect the blood in, so let's see here.
//Dr. Harris digs through his bag, which holds a collection of perfectly normal medical instruments and the most bizarre of ingredients before pulling out a scalpel)

6|Dr. Harris: Ah, here we are. This should make it easier. When you encounter the monsters, use this scalpel. Severing the arteries should make collecting the blood we need a breeze.
6|Dr. Harris:$give("Scapel")
6|Dr. Harris:$give("BloodBag")
6|Dr. Harris: As usual, come and find me when you're done.
6|Protag: Wait!
6|Dr. Harris: Sorry, no time for questions, we're on a tight schedule here.
6|Dr. Harris: Todaloo!
//(Dr. Harris leaves the room)


(The player must defeat enemies using the scalpel, as enemies take bleeding DOT, the blood bag fills up with blood, and once the bag is 100% full, the player can meet with the doctor again)


//(Protag enters a room with Dr. Dr. Harris)
7|Dr. Harris: Ah just on time, I knew we could count on you!
(Protag hands the doctor the blood bag and the scalpel)
7|Dr. Harris: You can keep the scalpel, never know when you might need it
Protag: So, what happened to the doctor's son?
Dr. Harris: It was tragic, really. The poor boy only had 30 yards left to go, when he went down hard.
Protag: What do you mean?
Dr. Harris: A broken leg, a sprained wrist, and a concussion, just one week before the finals.
Dr. Harris: But no matter, once we've got this cure finished he'll be back up and at it in no time, even better than he was before.
Dr. Harris: Now for the final ingredient, well not really an ingredient, but we need his jersey. You should be able to find it in the good doctor's office.
Dr. Harris: In the meantime, I shall start preparing the ingredients so that the doctor can do his work.
Dr. Harris: Todaloo!


(Protag finds his way to the doctor's office (outpatient tileset room) inside they can grab the jersey and possibly have a file with a list of some of the doctor's other patients including the experiments he did on them)




(Protag finds Dr Dr. Harris after grabbing the jersey)
Dr. Harris: Ah, the jersey.
Dr. Harris: This should be all for now, I've prepared the ingredients for the doctor and he should be able to cure his son now. Thank you for all of your help, I shall let the doctor how helpful you've been.
Dr. Harris: Todaloo!
    */





    string[] VarrenEncounter = new string[]
    {

    };




}