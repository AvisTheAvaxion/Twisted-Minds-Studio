using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] Dialogue.Dialog cutscene;
    [SerializeField] DialogueManager DialogueManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Starting cutscene");
            DialogueManager.SetCutscene(cutscene);
        }

    }
}
