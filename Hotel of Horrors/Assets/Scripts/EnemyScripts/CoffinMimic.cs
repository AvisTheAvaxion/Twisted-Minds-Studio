using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffinMimic : EnemyStateMachine
{
    [Header("Coffin Mimic Settings")]
    [SerializeField] Collider2D attackCollider;
    [SerializeField] float lungeForce = 5f;

    public override void MeleeAttackStart()
    {
        if (animator != null) animator.SetBool("Attacking", true);
        canMove = false;
        canAttack = false;
    }

    public override void MeleeAttack()
    {
        attackCollider.enabled = true;

        Vector2 dirToPlayer = GetDirToPlayer();
        rb.AddForce(dirToPlayer * lungeForce, ForceMode2D.Impulse);
    }

    public override void MeleeAttackEnd()
    {
        attackCollider.enabled = false;

        canMove = true;

        if (animator != null) animator.SetBool("Attacking", false);

        if (contWalkRightAfterAttack)
        {
            if (hasWalkCycle && animator != null) animator.SetBool("isWalking", true);
        }

        StartCoroutine(WaitBeforeAttacking(attackCooldownMin - myHealth.stats.GetCurrentValue(Stat.StatType.AttackSpeed),
            attackCooldownMax - myHealth.stats.GetCurrentValue(Stat.StatType.AttackSpeed)));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            enemyAudioManager.Attack(gameObject);

            IHealth health = collision.gameObject.GetComponent<IHealth>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;

            health.TakeDamage(meleeDamage + myHealth.stats.GetCurrentValue(Stat.StatType.MeleeDamage));
            health.Knockback(dir, knockback);
            if (effectsToInflict != null)
            {
                foreach (EffectInfo effectInfo in effectsToInflict)
                {
                    health.InflictEffect(new Effect(effectInfo, chanceToInflictEffect));
                }
            }
            attackCollider.enabled = false;
        }
    }
}
