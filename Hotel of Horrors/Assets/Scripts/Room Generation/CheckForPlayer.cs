using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForPlayer : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;
    QuestSystem questSystem;
    [SerializeField] bool activateOnEnter = true;
    private void Awake()
    {
        questSystem = FindObjectOfType<QuestSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && activateOnEnter)
        {
            Debug.Log("Player entering " + gameObject.transform.parent.name);
            questSystem.QuestEvent(QuestSystem.QuestEventType.RoomEnter, gameObject.transform.parent.name);
            spawner.isActiviated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("Player exiting " + gameObject.name);
            spawner.isActiviated = false;
        }
    }

    
}
