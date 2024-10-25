using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    bool paused;

    [SerializeField] Canvas PauseScreen;
    [SerializeField] DialogueManager dialogueManager;

    [SerializeField] CanvasScaler scaler;

    // Start is called before the first frame update
    void Start()
    {
        PauseScreen.gameObject.SetActive(false);
        paused = false;


        if (GlobalSettings.isFullScreen)
        {
            scaler.scaleFactor = 1f;
        }
        else
        {
            scaler.scaleFactor = 0.3f;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
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

        PauseScreen.gameObject.SetActive(true);
    }
    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1;
        PauseScreen.gameObject.SetActive(false);
    }
}
