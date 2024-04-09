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
        mainMenu.SetActive(false);
        loadMenu.SetActive(true);
        storeMenu.SetActive(false);
    }

    public void SaveGame()
    {
        savedText.SetActive(true);

        //save game here

    }

    public void ReturnToPrevRoom()
    {

    }
}
