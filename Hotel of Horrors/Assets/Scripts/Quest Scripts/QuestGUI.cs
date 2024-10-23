using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestGUI : MonoBehaviour
{
    [SerializeField] GameObject questGUI;
    [SerializeField] TextMeshProUGUI questTitleGUI;
    [SerializeField] TextMeshProUGUI questDescGUI;

    void ToggleQuestGUI(bool isActive)
    {
        questGUI.SetActive(isActive);
    }

    public void SetQuestTitle(string title)
    {
        questTitleGUI.text = title;
    }

    public void SetQuestDesc(string desc)
    {
        questDescGUI.text = desc;
    }
}
