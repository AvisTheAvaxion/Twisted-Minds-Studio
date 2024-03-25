using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyStates : MonoBehaviour
{
    [SerializeField] protected BasicShooter shooterClass;
    [SerializeField] protected States currentState = States.Idle;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Transform currentRoom;
    [SerializeField] protected float roomRadius = 5;
    [SerializeField] protected float aggroRadius = 9;
    [SerializeField] protected float targetDistFromPlayer = 7;

    protected bool canMove;
    protected GameObject playerObject;
    protected Vector2 randomDir;
    protected Vector3 targetPos;

    AILerp ai;

    public enum States
    {
        Idle, Dialogue , Patrolling, Fighting, Death
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
            case States.Idle:
                Idle();
                break;
            case States.Dialogue:
                Idle();
                break;
            case States.Patrolling:
                Patrol();
                break;
            case States.Fighting:
                Fight();
                break;
            case States.Death:
                Death();
                break;
        }
    }

    protected void Idle()
    {
        //starts patrolling when the player is in the room
        if(GetDistanceToPlayer() < 5)
        {
            currentState = States.Fighting;
        }
    }

    protected void Death() 
    { 
    
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
            currentState = States.Fighting;
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
        randomDir = Random.insideUnitCircle; //Get a random position around a unit circle.
    }
}
