using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    [Header("State Fields")]
    [SerializeField] public bool isInteractable;

    [Header("Dialoge")]
    [TextArea] public string dialoge;
}
