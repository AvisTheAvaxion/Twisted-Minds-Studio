using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuObj;
    [SerializeField] GameObject settingsObj;

    private void Awake()
    {
        UnityEngine.Cursor.visible = true;

        Screen.SetResolution(1920, 1080, true);
        Screen.fullScreen = true;
        GlobalSettings.isFullScreen = true;
        gameObject.GetComponent<CanvasScaler>().scaleFactor = 1f;
    }

    public void PlayGame()
    {
        //Update when we have save/load features
        
        //loads the test scene
        SceneManager.LoadScene(1);
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
