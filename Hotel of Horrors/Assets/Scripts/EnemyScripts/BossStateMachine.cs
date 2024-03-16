using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : EnemyStates
{
    [SerializeField] float dashSpeed;
    [SerializeField] float minAttackCooldown = 1f;
    [SerializeField] float maxAttackCooldown = 4f;
    Vector3 dashTarget;
    bool canAttack = true;
    bool dashing = false;

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
        //if (dist > targetDistFromPlayer)
        //{
        //    if(dashTarget == null || GetDistanceToTarget() < 0.25f)
        //    {
        //        canMove = false;
        //        dashTarget = playerObject.transform.position;
        //    }

        //    if (canMove)
        //    {
        //        GetPosBetweenTarget();
        //        transform.position = targetPos;
        //        //print("moving in attack " + targetPos);
        //    }
        //    else
        //    {
        //        StartCoroutine(WaitBeforeMoving());
        //    }

        //}
        //else 
        //{ 
        //    shooterClass.Attack();
        //} 

        //make sure the cooldown is finished before attacking
        if (canAttack)
        {
            print("Attacking");
            //slam attack
            if(dist < 1f)
            {
                shooterClass.Attack();
                StartCoroutine(WaitBeforeAttack());
            }
            //dash toward the players current location
            else
            {
                dashTarget = playerObject.transform.position;
                dashing = true;
            }
            canAttack = false;
        }

        //logic for dashing
        if (dashing)
        {
            GetPosBetweenTarget();
            transform.position = targetPos;

            if (GetDistanceToTarget() < .25f)
            {
                print("ended dash");
                dashing = false;
                StartCoroutine(WaitBeforeAttack());
            }
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

    protected IEnumerator WaitBeforeAttack()
    {
        print("Started cooldown");
        canAttack = false;
        yield return new WaitForSeconds(Random.Range(minAttackCooldown, maxAttackCooldown));
        print("Served Cooldown");
        canAttack = true;
    }
}
