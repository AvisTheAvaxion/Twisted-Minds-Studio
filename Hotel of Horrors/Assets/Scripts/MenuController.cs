using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuObj;
    [SerializeField] GameObject settingsObj;

    public void PlayGame()
    {
        //Update when we have save/load features
        
        //loads the test scene
        SceneManager.LoadScene(3);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ToggleSettings()
    {
        mainMenuObj.SetActive(!mainMenuObj.activeSelf);
        settingsObj.SetActive(!settingsObj.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }
}
