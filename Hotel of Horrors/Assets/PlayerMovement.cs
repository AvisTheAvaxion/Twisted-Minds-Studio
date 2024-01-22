using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Object References")]
    [SerializeField] GameObject player;
    private Transform playerTrans;

    [Header("Movement Modifiers")]
    public float movementSpeed;

    float vertical;
    float horizontal;

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        playerTrans.position += new Vector3(horizontal, vertical, 0f).normalized * movementSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
        print("hi");
    }
}
