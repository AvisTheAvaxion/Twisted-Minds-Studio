using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyMonster : EnemyStateMachine
{
    struct DashWeight
    {
        public Vector2 dir;
        public float weight;
    }
    DashWeight[] weights;

    [Header("Candy Monster Settings")]
    [SerializeField] int numOfDashes;
    [SerializeField] int possibleDashDirs = 8;
    [SerializeField] LayerMask dashObstacles;
    [SerializeField] float timeBtwDashes = 0.5f;
    [SerializeField] float dashCooldown = 5f;
    [SerializeField] float dashSpeed = 2;
    [SerializeField] float dashLength = 0.5f;
    [SerializeField] float dashRange = 2f;

    bool canDash;
    float dashTimer;
    float dashIntervalTimer = 0;
    int dashNumber = 0;

    Vector2 dashDir;

    protected override void Initialize()
    {
        weights = new DashWeight[possibleDashDirs];
        float angle = 0;
        float dAngle = 360f / possibleDashDirs;
        for (int i = 0; i < possibleDashDirs; i++)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            weights[i].weight = 0;
            weights[i].dir = new Vector2(x, y).normalized;

            angle += dAngle;
        }

        StartCoroutine(DashCooldown());
    }

    protected override void Fight()
    {
        if (canMove)
        {
            float distToTarget = GetDistanceToPlayer();
            Vector2 dirToPlayer = GetDirToPlayer();

            lastTargetPos = target.transform.position;

            if(canDash)
            {
                if(ChooseDashDir(out dashDir))
                {
                    dashNumber = 0;
                    dashIntervalTimer = 0;
                    dashTimer = 0;
                    currentState = States.Dash;
                }
            }

            if (distToTarget > maxDistanceToNavigate || RaycastPlayer(dirToPlayer, distToTarget).collider != null)
            {
                currentState = States.Searching;
            }
            else
            {
                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

                CheckToRotate(dirToPlayer);

                navigation.OrbitAroundTarget();

                if (distToTarget < range && canAttack)
                {
                    RangedAttackStart();

                    if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                }
            }
        }
    }

    protected override void Dash()
    {
        if(canMove && canDash)
        {
            if(dashNumber < numOfDashes)
            {
                if (dashTimer < dashLength)
                {
                    rb.velocity = dashDir * dashSpeed;
                    dashIntervalTimer = 0;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    if (dashIntervalTimer > timeBtwDashes)
                    {
                        dashNumber++;
                        dashIntervalTimer = 0;

                        if (ChooseDashDir(out dashDir))
                        {
                            dashTimer = 0;
                        }
                        else
                        {
                            dashNumber++;
                        }

                    }
                }
            } else
            {
                currentState = States.Fighting;
                StartCoroutine(DashCooldown());
            }
        }
        dashIntervalTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;
    }

    bool ChooseDashDir(out Vector2 dir)
    {
        Vector2 dirToTarget = GetDirToPlayer();
        float distToTarget = GetDistanceToPlayer();

        CheckToRotate(dirToTarget);

        float largestWeight = -10;
        //Set baseline weight based off dot product to desire direction
        for (int i = 0; i < possibleDashDirs; i++)
        {
            float w = Vector2.Dot(weights[i].dir, dirToTarget);

            if (distToTarget < dashRange)
                w = 1 - Mathf.Abs(w);

            RaycastHit2D hitInfo = Physics2D.Raycast((Vector2)rayCastOrigin.position, weights[i].dir, dashSpeed * dashLength, dashObstacles);
            float negativeWeight = 1 - Mathf.Clamp(hitInfo.distance / (dashSpeed * dashLength), 0f, 1f);

            w -= negativeWeight;

            weights[i].weight = w;

            if(weights[i].weight > largestWeight)
            {
                largestWeight = weights[i].weight;
            }
        }

        List<Vector2> dashDirs = new List<Vector2>();
        for (int i = 0; i < possibleDashDirs; i++)
        {
            if(weights[i].weight > largestWeight - 0.3f)
            {
                dashDirs.Add(weights[i].dir);
            }
        }

        if(dashDirs.Count > 0)
        {
            dir = dashDirs[Random.Range(0, dashDirs.Count)];
            CheckToRotate(dir);
            return true;
        } else
        {
            dir = new Vector2();
            return false;
        }
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
