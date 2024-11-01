using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] Objectives.ObjectiveTriggers objective;
    QuestSystem quest;
    private void Awake()
    {
        quest = FindObjectOfType<QuestSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            quest.QuestEvent(QuestSystem.QuestEventType.TripTrigger, objective.ToString());
            Destroy(this.gameObject);
        }
    }
}
