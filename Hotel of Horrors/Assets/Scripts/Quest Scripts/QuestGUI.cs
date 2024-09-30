using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestGUI : MonoBehaviour
{
    [SerializeField] GameObject questGUI;
    [SerializeField] TextMeshProUGUI questTextGUI;
    QuestSystem questSystem;

    private void Awake()
    {
        questSystem = FindObjectOfType<QuestSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        questSystem.OnQuestUpdate += SetQuestText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleQuestGUI(bool isActive)
    {
        questGUI.SetActive(isActive);
    }

    void SetQuestText(object sender, EventArgs e)
    {
        QuestType quest = (QuestType)sender;
        questTextGUI.text = quest.ToSaveString();
    }
}
