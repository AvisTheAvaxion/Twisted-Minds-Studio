using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveFileGUI : MonoBehaviour
{
    [SerializeField] TMP_Text saveName;
    [SerializeField] TMP_Text floorNum;
    [SerializeField] TMP_Text playTime;
    [SerializeField] TMP_Text lastPlayed;
    [SerializeField] Animator saveAnimator;

    int saveNum;
    public int SaveNum { get => saveNum; }

    MenuController menuController;
    private void Start()
    {
        menuController = FindObjectOfType<MenuController>();
    }

    public void SetSaveFileGUI(SerializedClass saveData, int saveNum)
    {
        this.saveNum = saveNum;
        saveName.text = $"Save {saveNum}";
        floorNum.text = $"Floor {saveData.level}";
        playTime.text = $"Playtime - {string.Format("{0:00.#}", Mathf.RoundToInt(saveData.playTime / 3600))}:" +
            $"{string.Format("{0:00.#}", Mathf.RoundToInt(saveData.playTime / 60))}";
        //playTime.text = $"Playtime - {string.Format("{0:0.0}", saveData.playTime / 60f)}";
        lastPlayed.text = $"Last Played - {saveData.month}/{saveData.day}/{saveData.year}";
    }

    public void SetSelectedFile()
    {
        if(menuController == null) menuController = FindObjectOfType<MenuController>();

        //menuController.LoadGame(saveNum);
        menuController.SetSelectedFile(saveNum);
        //saveAnimator.SetBool("Selected", true);
        //border.sprite = selectedBorderSprite;
    }
    public void UpdateVisuals(bool selected)
    {
        saveAnimator.SetBool("Selected", selected);
        //border.sprite = borderSprite;
    }

    /*public void SetSaveToDelete()
    {
        menuController.SetSelectedFile(saveNum);
    }*/
}
