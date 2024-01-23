using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Object References")]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    private Transform playerTrans;

    [Header("Movement Modifiers")]
    public float movementSpeed;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    Vector2 movementVector = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(movementVector != Vector2.zero)
        {
            print("got 1");
            int count = rb.Cast(
                movementVector,
                movementFilter,
                castCollisions,
                movementSpeed * Time.deltaTime + collisionOffset);

            if(count == 0)
            {
                print("Got 2");
                rb.MovePosition(rb.position + movementVector.normalized * movementSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void OnWASD(InputValue inputValue)
    {
        movementVector = inputValue.Get<Vector2>();
        print("hi");
    }
}
