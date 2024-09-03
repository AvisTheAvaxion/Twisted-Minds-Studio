using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElevatorMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject loadMenu;
    [SerializeField] GameObject storeMenu;
    [SerializeField] GameObject savedText;
    [SerializeField] GameObject loadText;
    [SerializeField] GameObject elevatorBackground;
    [SerializeField] GameObject mindRoomMenu;
    [SerializeField] Image fadeImage;
    [SerializeField] float doorTransitionLength = 0.5f;
    [SerializeField] PlayerMovement player;
    [SerializeField] NewAudioManager audioManager;
    [SerializeField] SerializationManager serializationManager;

    private void Start()
    {
        elevatorBackground.SetActive(false);
    }

    public void SwitchToMain()
    {
        mainMenu.SetActive(true);
        loadMenu.SetActive(false);
        storeMenu.SetActive(false);
    }

    public void SwitchToStore()
    {
        mainMenu.SetActive(false);
        loadMenu.SetActive(false);
        storeMenu.SetActive(true);
    }

    public void SwitchToLoad()
    {
        loadText.SetActive(true);
        serializationManager.LoadData();
        //mainMenu.SetActive(false);
        //loadMenu.SetActive(true);
        //storeMenu.SetActive(false);
    }

    public void SaveGame()
    {
        savedText.SetActive(true);

        //save game here
        serializationManager.SaveData();
    }

    public void ReturnToPrevRoom()
    {
        StartCoroutine(GetFreshRoom());
    }

    public void LeaveMindMenu()
    {
        Cursor.visible = false;
        mindRoomMenu.SetActive(false);
    }

    IEnumerator GetFreshRoom()
    {
        print("fading black");
        audioManager.PlayEffect("DoorOpen");
       
        // loop over 1 second - fade to black
        for (float i = 0; i <= doorTransitionLength / 2f; i += Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }


        print("fading clear");
        // loop over 1 second backwards - fade to clear
        for (float i = doorTransitionLength / 2f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(0, 0, 0, i / (doorTransitionLength / 2f));
            yield return null;
        }
        player.canMove = true;
        elevatorBackground.SetActive(false);
    }
}
