using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] bool playerInRange;
    bool isNPCInteractable;

    [SerializeField] GameObject DialogeBG;

    private void Awake()
    {
        DialogeBG = GameObject.Find("CanvasDialoge").transform.GetChild(0).gameObject;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            isNPCInteractable = collision.GetComponent<NPCBehavior>().isInteractable;
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            playerInRange = false;
        }
    }

    void OnInteract()
    {
        if (playerInRange && isNPCInteractable)
        {
            DialogeBG.SetActive(true);
        }
    }
}
