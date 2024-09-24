using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Werewolf : EnemyStateMachine
{
    [Header("Werewolf Specific Settings")]
    [SerializeField] float maxJumpRange = 2f;
    [SerializeField] float jumpLength = 1f;
    [SerializeField] float minAttackDistance = 0.2f;
    [SerializeField] float jumpSpeed = 1f;
    [SerializeField] float jumpCancelStun = 2f;

    Vector2 jumpDir;
    bool isAttacking;
    bool jumping = false;

    float jumpTimer = 0;

    protected override void Fight()
    {
        if(isAttacking)
        {
            float distToTarget = GetDistanceToPlayer();

            if (jumping)
                rb.velocity = jumpDir * jumpSpeed;
            else
                rb.velocity = Vector2.zero;

            jumpTimer += Time.deltaTime;

            if(jumping && (distToTarget <= minAttackDistance || jumpTimer > jumpLength))
            {
                animator.SetTrigger("Attack");
                animator.SetBool("isJumping", false);
                //MeleeAttack();
            }
        }
        else if (canMove)
        {
            float distToTarget = GetDistanceToPlayer();
            Vector2 dirToPlayer = GetDirToPlayer();

            lastTargetPos = target.transform.position;

            if (distToTarget > maxDistanceToNavigate || RaycastPlayer(dirToPlayer, distToTarget).collider != null)
            {
                currentState = States.Searching;
            }
            else
            {
                if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);

                CheckToRotate(dirToPlayer);

                if (canAttack)
                {
                    navigation.MoveTowardsTarget();
                    if (distToTarget < maxJumpRange)
                    {
                        MeleeAttackStart();
                        if (hasWalkCycle && animator != null) animator.SetBool("isWalking", false);
                    }
                }
                else
                {
                    navigation.OrbitAroundTarget();
                }
            }
        }
    }

    public override void MeleeAttackStart()
    {
        canMove = false;
        canAttack = false;
        isAttacking = true;

        jumpDir = (lastTargetPos - transform.position).normalized;
        jumpTimer = 0;

        if (animator != null)
        {
            animator.ResetTrigger("Cancel");
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Jump");
            animator.SetBool("isJumping", true);
        }
    }
    public void BeginJump()
    {
        jumping = true;
    }
    public override void MeleeAttack()
    {
        rb.velocity = Vector2.zero;
        jumping = false;

        //base.MeleeAttack();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeRadius);
        print(colliders.Length);
        enemyAudioManager.Attack(gameObject);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag.Equals("Player"))
            {
                IHealth health = colliders[i].gameObject.GetComponent<IHealth>();
                Vector2 dir = (colliders[i].transform.position - transform.position).normalized;

                if(health.TakeDamage(meleeDamage))
                    health.Knockback(dir, knockback);

                if (effectsToInflict != null)
                {
                    foreach (EffectInfo effectInfo in effectsToInflict)
                    {
                        health.InflictEffect(new Effect(effectInfo, chanceToInflictEffect));
                    }
                }
                break;
            }
        }
    }
    public override void MeleeAttackEnd()
    {
        isAttacking = false;
        canMove = true;

        StartCoroutine(WaitBeforeAttacking(attackCooldownMin, attackCooldownMax));
    }

    public override void Knockback(Vector2 dir, float strength)
    {
        base.Knockback(dir, strength);

        if(isAttacking)
        {
            animator.SetTrigger("Cancel");
            animator.SetBool("isJumping", false);
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Attack");
            currentStunLength = jumpCancelStun;

            rb.velocity = Vector2.zero;
            jumping = false;

            isAttacking = false;
            canMove = true;
            canAttack = true;
        }
    }
}
