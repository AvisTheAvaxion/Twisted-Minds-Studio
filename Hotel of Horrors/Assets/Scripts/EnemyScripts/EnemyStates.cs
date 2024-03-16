using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    [SerializeField] protected BasicShooter shooterClass;
    [SerializeField] protected States currentState = States.idle;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Transform currentRoom;
    [SerializeField] protected float roomRadius = 5;
    [SerializeField] protected float aggroRadius = 9;
    [SerializeField] protected float targetDistFromPlayer = 7;

    protected bool canMove;
    protected GameObject playerObject;
    protected Vector2 randomDir;
    protected Vector3 targetPos;

    public enum States
    {
        idle, dialogue , patrolling, fighting
    }

    private void Start()
    {
        playerObject = GameObject.Find("Player");
    }

    private void Update()
    {
        ChooseState();
    }

    protected void ChooseState()
    {
        switch (currentState)
        {
            case States.idle:
                Idle();
                break;
            case States.dialogue:
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

    protected float GetDistanceToPlayer()
    {
        return Vector3.Distance(this.transform.position, playerObject.transform.position);
    }

    protected void GetPosBetweenPlayer()
    {
        targetPos = Vector3.MoveTowards(transform.position, playerObject.transform.position, moveSpeed * Time.deltaTime); // calculate distance to move).normalized;
    }

    protected void SetRandomDir()
    {
        randomDir =  Random.insideUnitCircle; //Get a random position around a unit circle.
    }

    protected void Idle()
    {
        //starts patrolling when the player is in the room
        if(GetDistanceToPlayer() < 5)
        {
            currentState = States.fighting;
        }
    }

    protected void Patrol()
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

    protected virtual void Fight()
    {
        if (GetDistanceToPlayer() > targetDistFromPlayer)
        {


            if (canMove)
            {
                GetPosBetweenPlayer();
                transform.position = targetPos;
                //print("moving in attack " + targetPos);
            }
            else
            {
                StartCoroutine(WaitBeforeMoving());
            }

        }
        else canMove = false;
        shooterClass.Attack();
    }


    protected IEnumerator WaitBeforeMoving()
    {
        canMove = false;
        yield return new WaitForSeconds(Random.Range(.5f, 2f));
        canMove = true;
    }
}
