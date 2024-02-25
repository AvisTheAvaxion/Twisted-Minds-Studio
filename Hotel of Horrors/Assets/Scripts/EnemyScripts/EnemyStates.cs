using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    [SerializeField] BasicShooter shooterClass;
    [SerializeField] States currentState = States.idle;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform currentRoom;
    [SerializeField] float roomRadius = 5;
    GameObject playerObject;

    public enum States
    {
        idle, patrolling, fighting
    }

    private void Start()
    {
        playerObject = GameObject.Find("Player");
    }

    private void Update()
    {
        switch (currentState)
        {
            case States.idle:
                Idle();
                break;
            case States.patrolling:
                Patrol();
                break;
            case States.fighting:
                Fight();
                break;
        }
    }

    float GetDistanceToPlayer()
    {
        return Vector3.Distance(this.transform.position, playerObject.transform.position);
    }

    void Idle()
    {
        if(GetDistanceToPlayer() < 20)
        {
            currentState = States.patrolling;
        }
    }

    void Patrol()
    {
        //wander aimlessly around the room
        Vector2 rng = Random.insideUnitCircle; //Get a random position around a unit circle.
        var dir = new Vector3(rng.x, 1, rng.y);
        dir.Normalize(); //Ensure magnitude is 1 so we just have a direction
        dir *= roomRadius; //Multiply by the radius to determine how far.
        dir += currentRoom.position; //This makes the new position relative to the player.

    }

    void Fight()
    {
        if(GetDistanceToPlayer() > 7)
        {
            var step = moveSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, playerObject.transform.position, step);
        }
        shooterClass.Attack();
    }

}
