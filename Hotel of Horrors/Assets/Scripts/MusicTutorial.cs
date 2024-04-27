using UnityEngine;

public class MusicTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //The message that brought you here... probably
        Debug.Log("View the script sending this message to learn how to play music.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //How to use the AudioManager
    void howToUseAudioManager()
    {
        //The audio manager is set up as public and static, so that it can be used from any script you like.
        //You don't have a seperate script for music, in addition, you don't need to make an object.

        //To play a track, you need to call the AudioManager.Play function and pass in either a track name, or id, as shown below

        //Using the track name
        AudioManager.Play("test track");

        //Using the track id
        AudioManager.Play(0);


        //To view the track names an Id's you can call the class's ToString method
        //This will return a string containing the class's tracks and Id's
        Debug.Log(AudioManager.ToString());

        //In addition, the ToString method can take a string as a Parameter to search the list of availible tracks
        //This will return a string listing the names and id's of any tracks that contain the string query
        //It is reccomended to print this to console to view the string

        //For example
        Debug.Log(AudioManager.ToString("Stien"));
        //Would return any tracks that have the word "Stien" in there name. The string is NOT case sensitive
        Debug.Log(AudioManager.ToString("sTiEn"));
        //Would return the same thing.


        //May also add methods to check if things are currently being played

        //Hope this tutorial is helpful
        // -Avis


        //P.S. Don't press H
    }
}
