using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] Toggle fullScreenToggle;
    [SerializeField] CanvasScaler scaler;
    [SerializeField] GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnFullScreenUpdate()
    {
        Screen.fullScreen = fullScreenToggle.isOn;

        if (fullScreenToggle.isOn)
        {
            Screen.SetResolution(1920, 1080, true);
            Screen.fullScreen = true;
            GlobalSettings.isFullScreen = true;
            scaler.scaleFactor = 1f;
        }
        else
        {
            Screen.SetResolution(768, 432, false);
            Screen.fullScreen = false;
            GlobalSettings.isFullScreen = false;
            scaler.scaleFactor = 0.40f;
        }
    }
}
