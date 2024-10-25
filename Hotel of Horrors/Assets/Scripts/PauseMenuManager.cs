using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    bool paused;

    [SerializeField] Canvas canvas;
    [SerializeField] GameObject PauseScreen;
    [SerializeField] GameObject Settings;

    [SerializeField] DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        canvas.gameObject.SetActive(false);

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

        canvas.gameObject.SetActive(true);
        PauseScreen.SetActive(true);
        Settings.SetActive(false);
    }
    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1;
        canvas.gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        Settings.SetActive(true);
        PauseScreen.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
