using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    bool paused;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject settings;

    [SerializeField] DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);

        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(dialogueManager.getCutsceneState().ToString());

        if (Input.GetKeyUp(KeyCode.Escape) && dialogueManager.getCutsceneState() == DialogueManager.CutsceneState.None)
        {
            paused = !paused;

            if (paused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0;

        pauseMenu.SetActive(true);
        pauseScreen.SetActive(true);
        settings.SetActive(false);
    }
    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        pauseScreen.SetActive(false);
    }
    public void CloseSettings()
    {
        settings.SetActive(false);
        pauseScreen.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
