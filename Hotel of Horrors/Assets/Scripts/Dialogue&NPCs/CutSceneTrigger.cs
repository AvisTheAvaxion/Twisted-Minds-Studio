using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] Dialogue.Dialog cutscene;
    [SerializeField] DialogueManager DialogueManager;

    bool cutSceneStarted = false;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (cutSceneStarted) {return; }

        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Starting cutscene");
            DialogueManager.SetCutscene(cutscene);
            cutSceneStarted = true;

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cutSceneStarted) { return; }

        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Starting cutscene");
            DialogueManager.SetCutscene(cutscene);
            cutSceneStarted = true;

            gameObject.SetActive(false);
        }
    }
}
