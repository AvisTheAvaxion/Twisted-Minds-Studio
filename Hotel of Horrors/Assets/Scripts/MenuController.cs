using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPage;
    [SerializeField] SettingsScript settingsPage;
    [SerializeField] GameObject loadGamePage;

    [SerializeField] Transform saveFilesContainer;
    [SerializeField] GameObject saveFilePrefab;
    [SerializeField] GameObject noSavesMessage;

    List<SaveFileGUI> saveFilesGUI;

    SerializationManager serialization;

    int nextSaveFileNum;

    int selectedFileNum;

    private void OnEnable()
    {
        UnityEngine.Cursor.visible = true;

        GameTime.UnpauseTime();

        if(settingsPage)
            settingsPage.LoadSettings();
        /*Screen.SetResolution(1920, 1080, true);
        Screen.fullScreen = true;
        GlobalSettings.isFullScreen = true;
        gameObject.GetComponent<CanvasScaler>().scaleFactor = 1f;*/

        saveFilesGUI = new List<SaveFileGUI>();

        selectedFileNum = -1;
        if (PlayerPrefs.HasKey("SaveFile"))
            selectedFileNum = PlayerPrefs.GetInt("SaveFile");

        serialization = FindObjectOfType<SerializationManager>();

        if(loadGamePage != null)
            LoadSaveFiles();
    }

    public void UpdateGUI()
    {
        for (int i = 0; i < saveFilesGUI.Count; i++)
        {
            if (saveFilesGUI[i].SaveNum != selectedFileNum)
            {
                saveFilesGUI[i].UpdateVisuals(false);
            }
            else
            {
                saveFilesGUI[i].UpdateVisuals(true);
            }
        }
    }

    public void PlayGame()
    {
        //Update when we have save/load features
        if (!PlayerPrefs.HasKey("SaveFile"))
        {
            NewGame();
        }
        else
        {
            int saveFile = PlayerPrefs.GetInt("SaveFile");
            //loads the test scene
            LoadGame(saveFile);
        }
    }

    public void NewGame()
    {
        //Update when we have save/load features
        PlayerPrefs.SetInt("SaveFile", nextSaveFileNum);
        
        //loads the test scene
        SceneManager.LoadScene(1);
    }


    public void LoadGame(int saveFile)
    {
        PlayerPrefs.SetInt("SaveFile", saveFile);
        SerializedClass saveData = serialization.SoftLoadData(saveFile);
        if (saveData != null)
        {
            //SceneManager.LoadScene(1);
            SceneManager.LoadScene($"Floor {saveData.level}");
        }
        else
        {
            NewGame();
        }
    }

    public IEnumerator DestroySave(int fileNum)
    {
        serialization.DeleteSave(fileNum);
        yield return null;
        LoadSaveFiles();
    }

    public void SetSelectedFile(int fileNum)
    {
        this.selectedFileNum = fileNum;

        UpdateGUI();
    }
    public void ConfirmDeleteSave()
    {
        StartCoroutine(DestroySave(selectedFileNum));
        selectedFileNum = -1;
        UpdateGUI();
    }
    public void ConfirmLoadSaveFile()
    {
        if (selectedFileNum >= 0)
            LoadGame(selectedFileNum);
        else
            NewGame();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ToggleSettings()
    {
        mainMenuPage.SetActive(!mainMenuPage.activeSelf);
        settingsPage.gameObject.SetActive(!settingsPage.gameObject.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }

    public void LoadSaveFiles()
    {
        saveFilesGUI.Clear();
        for (int i = 0; i < saveFilesContainer.childCount; i++)
        {
            Destroy(saveFilesContainer.GetChild(i).gameObject);
        }
        /*while(saveFilesContainer.childCount > 0)
        {
            Destroy(saveFilesContainer.GetChild(saveFilesContainer.childCount - 1).gameObject);
        }*/

        int saveNum = 1;
        SerializedClass[] saves = serialization.GetAllSaveData();
        for (int i = 0; i < saves.Length; i++)
        {
            GameObject go = Instantiate(saveFilePrefab, saveFilesContainer);
            SaveFileGUI saveFileGUI = go.GetComponent<SaveFileGUI>();
            if (saveFileGUI)
            {
                saveFileGUI.SetSaveFileGUI(saves[i], saves[i].saveNum);
                saveFilesGUI.Add(saveFileGUI);

                if(saveFileGUI.SaveNum == selectedFileNum)
                {
                    saveFileGUI.SetSelectedFile();
                }
            }

            if (saves[i].saveNum == saveNum) saveNum++;
        }

        nextSaveFileNum = saveNum;

        if(saves.Length > 0)
        {
            noSavesMessage.SetActive(false);
        } 
        else
        {
            noSavesMessage.SetActive(true);
        }
    }
}
