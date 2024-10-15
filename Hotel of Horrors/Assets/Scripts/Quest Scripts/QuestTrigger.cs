using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] string triggerName;
    QuestSystem quest;
    private void Awake()
    {
        quest = FindObjectOfType<QuestSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        quest.QuestEvent(QuestSystem.QuestEventType.TripTrigger, triggerName);
        Destroy(this.gameObject);
    }
}
