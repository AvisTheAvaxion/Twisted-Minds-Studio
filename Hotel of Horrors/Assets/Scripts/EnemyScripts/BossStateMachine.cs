using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : EnemyStates
{
    [SerializeField] float dashSpeed;
    Vector3 dashTarget;

    private void Update()
    {
        ChooseState();
    }
    private void Start()
    {
        playerObject = GameObject.Find("Player");
    }

    protected override void Fight()
    {
        float dist = GetDistanceToPlayer();
        if (dist > targetDistFromPlayer)
        {
            if(dashTarget == null || GetDistanceToTarget() < 0.25f)
            {
                canMove = false;
                dashTarget = playerObject.transform.position;
            }

            if (canMove)
            {
                GetPosBetweenTarget();
                transform.position = targetPos;
                //print("moving in attack " + targetPos);
            }
            else
            {
                StartCoroutine(WaitBeforeMoving());
            }

        }
        else 
        { 
            shooterClass.Attack();
        } 


    }



    protected float GetDistanceToTarget()
    {
        return Vector3.Distance(this.transform.position, dashTarget);
    }

    protected void GetPosBetweenTarget()
    {
        targetPos = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime); // calculate distance to move).normalized;
    }
}
