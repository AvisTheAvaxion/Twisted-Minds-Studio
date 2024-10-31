using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] NewAudioManager NewAudioManager;
    [SerializeField] Toggle fullScreenToggle;
    [SerializeField] CustomToggle fullScreenCustom;
    [SerializeField] Toggle vSyncToggle;
    [SerializeField] CustomToggle vSyncCustom;
    [SerializeField] Slider camShakeDampenSlider;
    [SerializeField] TMP_Text resolutionText;
    [SerializeField]
    Vector2Int[] resolutions =
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1366, 768),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160),
        new Vector2Int(768, 432),
        new Vector2Int(1600, 900),
        new Vector2Int(1600, 1000)
    };

    int resolution;
    bool fullscreen;
    bool vsync;
    //[SerializeField] GameObject mainMenu;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (NewAudioManager == null) NewAudioManager = FindObjectOfType<NewAudioManager>();

        LoadSettings(false);
    }

    public void LoadSettings(bool apply = true)
    {
        fullScreenToggle.isOn = fullscreen = (PlayerPrefs.HasKey("Fullscreen") ? PlayerPrefs.GetInt("Fullscreen") == 1 : true);
        fullScreenCustom.UpdateToggle();
        vSyncToggle.isOn = vsync = PlayerPrefs.HasKey("Vsync") ? PlayerPrefs.GetInt("Vsync") == 1 : false;
        vSyncCustom.UpdateToggle();

        Vector2Int currentRes = new Vector2Int(PlayerPrefs.HasKey("ResolutionX") ? PlayerPrefs.GetInt("ResolutionX") : 1920,
            PlayerPrefs.HasKey("ResolutionY") ? PlayerPrefs.GetInt("ResolutionY") : 1080);
        for (int i = 0; i < resolutions.Length; i++)
        {
            if(currentRes.x == resolutions[i].x && currentRes.y == resolutions[i].y)
            {
                resolution = i;
            }
        }
        resolutionText.text = $"{resolutions[resolution].x} x {resolutions[resolution].y}";

        camShakeDampenSlider.value = PlayerPrefs.HasKey("Dampener") ? PlayerPrefs.GetFloat("Dampener") * 10 : 8;

        if(apply)
            ApplySettings();
    }

    public void OnFullScreenUpdate()
    {
        fullscreen = fullScreenToggle.isOn;
        if (NewAudioManager) NewAudioManager.PlayEffect("NextLine");
        /*if (fullScreenToggle.isOn)
        {
            //Screen.SetResolution(1920, 1080, true);
            Screen.fullScreen = true;
            GlobalSettings.isFullScreen = true;
        }
        else
        {
            //Screen.SetResolution(768, 432, false);
            Screen.fullScreen = false;
            GlobalSettings.isFullScreen = false;
        }*/
    }
    public void OnVsyncUpdate()
    {
        vsync = vSyncToggle.isOn;
        if(NewAudioManager) NewAudioManager.PlayEffect("NextLine");
    }

    public void IncrementResolution(bool increment)
    {
        resolution = ((increment ? resolution + 1 : resolution - 1) + resolutions.Length) % resolutions.Length;
        resolutionText.text = $"{resolutions[resolution].x} x {resolutions[resolution].y}";

        if (NewAudioManager) NewAudioManager.PlayEffect("NextLine");
    }

    public void ApplySettings()
    {
        if (NewAudioManager) NewAudioManager.PlayEffect("NextLine");

        GlobalSettings.isFullScreen = fullscreen;
        Screen.SetResolution(resolutions[resolution].x, resolutions[resolution].y, fullscreen);
        QualitySettings.vSyncCount = vsync ? 1 : 0;

        UpdateSettings();
    }

    public void UpdateSettings()
    {
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("Vsync", vsync ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionX", resolutions[resolution].x);
        PlayerPrefs.SetInt("ResolutionY", resolutions[resolution].y);
        PlayerPrefs.SetFloat("Dampener", camShakeDampenSlider.value / 10f);
    }
}
