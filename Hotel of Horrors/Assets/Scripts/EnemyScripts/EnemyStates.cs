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
    [SerializeField] float aggroRadius = 9;
    [SerializeField] float targetDistFromPlayer = 7;

    bool canMove;
    GameObject playerObject;
    Vector2 randomDir;
    Vector3 targetPos;

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
        print(currentState.ToString());
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

    void GetPosBetweenPlayer()
    {
        targetPos = Vector3.MoveTowards(transform.position, playerObject.transform.position, moveSpeed * Time.deltaTime); // calculate distance to move).normalized;
    }

    void SetRandomDir()
    {
        randomDir =  Random.insideUnitCircle; //Get a random position around a unit circle.
    }

    void Idle()
    {
        //starts patrolling when the player is in the room
        if(GetDistanceToPlayer() < 5)
        {
            currentState = States.fighting;
        }
    }

    void Patrol()
    {
        //wander aimlessly around the room
        var dir = new Vector3(randomDir.x, 1, randomDir.y);
        dir.Normalize(); //Ensure magnitude is 1 so we just have a direction
        dir *= roomRadius; //Multiply by the radius to determine how far.
        dir += transform.position; //This makes the new position relative to the enemy that is moving.

        if (Vector3.Distance(this.transform.position, dir) > .5)
        {
            print("moving");
            var step = moveSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, dir, moveSpeed * Time.deltaTime);
        }
        else
        {
            print("chaning destination");
            SetRandomDir();
        }

        if(GetDistanceToPlayer() < aggroRadius){
            currentState = States.fighting;
        }
    }

    void Fight()
    {
        if (GetDistanceToPlayer() > targetDistFromPlayer)
        {


            if (canMove)
            {
                GetPosBetweenPlayer();
                transform.position = targetPos;
                print("moving in attack " + targetPos);
            }
            else
            {
                StartCoroutine(WaitBeforeMoving());
            }

        }
        else canMove = false;
        shooterClass.Attack();
    }


    IEnumerator WaitBeforeMoving()
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(.5f, 2f));
        canMove = true;
    }
}
