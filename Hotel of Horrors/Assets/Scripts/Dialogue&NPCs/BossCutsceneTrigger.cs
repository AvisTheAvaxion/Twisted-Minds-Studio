using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCutsceneTrigger : MonoBehaviour
{
    [SerializeField] Dialogue.Dialog cutscene;
    [SerializeField] DialogueManager DialogueManager;
    [SerializeField] BossStateMachine boss;

    bool cutSceneStarted = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cutSceneStarted) { return; }

        if (collision.gameObject.name == "Player")
        {
            boss.OnDialogueStart(cutscene);
            cutSceneStarted = true;
            gameObject.SetActive(false);
        }
    }
}
