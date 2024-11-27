using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarrenBoss : BossStateMachine
{
    AI ai;
    Animator animator;

    [System.Serializable]
    public struct AttackSettings
    {
        [Header("Phase 1")]
        public float p1LaunchForce;// = 5f;
        public Vector2 p1AttackCooldown;// = 7;
        public float teleportCooldown;// = 15;

        [Header("Phase 2")]
        public float p2NormalSpeed; //4
        public float p2ChaseSpeed; //6
        public float p2GracePeriod;// = 0.5f;
        public float p2FollowPeriod;// = 2f;
        public Vector2 p2AttackCooldown;// = 7f;
        public float p2StunLength;// = 5f;
        public float p2SummonCooldown;// = 10f;
        public float p2SummonRadius; // 2f;
        public float p2AttackRadius;
        public float p2Damage;
        public float p2Knockback;
    }

    [Header("Varren Settings")]
    [SerializeField] AttackSettings normalSettings;
    [SerializeField] AttackSettings enrageSettings;

    [Header("Phase 1")]
    [SerializeField] RuntimeAnimatorController animatorController;
    [SerializeField] Transform p1ProjectileSpawnPoint;
    [SerializeField] MultiProjectile[] p1Projectiles;
    [SerializeField] Transform[] teleportLocations;
    int currentTeleportLoc;
    float teleportTimer;

    [Header("Phase 2")]
    [SerializeField] LayerMask p2AttackMask;
    [SerializeField] Transform p2AttackPoint;
    [SerializeField] GameObject[] p2SummonEnemies;
    [SerializeField] LayerMask p2SummonMask;
    float summonTimer;

    BasicShooter shooter;

    AttackSettings currentSettings;

    bool isStunned;

    public override void Stun(float stunLength, bool overrideCurrent)
    {
        
    }

    protected override void Death()
    {
        
    }

    protected override IEnumerator DeathSequence()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator DialogueEnd()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator DialogueStart(Dialogue.Dialog cutscene)
    {
        throw new System.NotImplementedException();
    }

    protected override void Disenrage()
    {
        
    }

    protected override void Enrage()
    {
        
    }

    protected override void Fight()
    {
        switch(currentStageIndex)
        {
            case 0:
                Phase1Fight();
                break;
            case 1:
                Phase2Fight();
                break;
        }
    }

    #region Phase 1
    protected void Phase1Fight()
    {
        if(canAttack && teleportTimer > currentSettings.teleportCooldown)
        {
            teleportTimer = 0;
            StartCoroutine(TeleportSequence());
        }
        else if(canAttack)
        {
            StartCoroutine(ShootP1Projectile());
        }

        teleportTimer += Time.deltaTime;
    }

    IEnumerator ShootP1Projectile()
    {
        animator.SetTrigger("Shoot");

        canAttack = false;
        if (stages[currentStageIndex].attackSequence[currentAttack] <= p1Projectiles.Length - 1)
        {
            for (int i = 0; i < p1Projectiles.Length; i++)
            {
                MultiProjectile projectile = Instantiate(p1Projectiles[stages[currentStageIndex].attackSequence[currentAttack]].gameObject, p1ProjectileSpawnPoint.position, p1ProjectileSpawnPoint.rotation).GetComponent<MultiProjectile>();
                projectile.Initialize();
                yield return new WaitForSeconds(0.5f);
                projectile.Launch(currentSettings.p1LaunchForce, Vector2.up);
            }
        }
        else
        {
            for (int i = 0; i < p1Projectiles.Length; i++)
            {
                MultiProjectile projectile = Instantiate(p1Projectiles[i].gameObject, p1ProjectileSpawnPoint.position, p1ProjectileSpawnPoint.rotation).GetComponent<MultiProjectile>();
                yield return new WaitForSeconds(0.5f);
                projectile.Launch(currentSettings.p1LaunchForce, Vector2.up);
            }
        }

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(currentSettings.p1AttackCooldown)));
    }
    bool teleported;
    IEnumerator TeleportSequence()
    {
        teleported = false;
        canAttack = false;

        animator.SetTrigger("Teleport");

        yield return new WaitUntil(() => teleported);

        print("Teleported");

        int nextTeleportLoc;
        do
        {
            nextTeleportLoc = Random.Range(0, teleportLocations.Length);
        }
        while (nextTeleportLoc == currentTeleportLoc);

        currentTeleportLoc = nextTeleportLoc;
        transform.position = teleportLocations[currentTeleportLoc].position;

        yield return new WaitForSeconds(0.5f);

        if(attackCoroutine == null)
            canAttack = true;

        teleportTimer = 0;
    }
    public void Teleport() //Animation Event
    {
        teleported = true;
    }
    #endregion

    #region Phase 2
    bool p2HitAttack;
    protected void Phase2Fight()
    {
        if (!isStunned && phase2Attack == null)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ai.SetSpeed(currentSettings.p2NormalSpeed);
            animator.SetBool("isFlying", true);
            animator.SetFloat("x", Mathf.Clamp(rb.velocity.x, -1, 1));
            collider.enabled = false;
            //ai.SetSpeed(10);
            ai.OrbitAroundTarget(player.transform.position);
        }

        if(canAttack)
        {
            phase2Attack = StartCoroutine(Phase2Attack());
        }

        if(summonTimer >= currentSettings.p2SummonCooldown)
        {
            summonTimer = 0;
            SummonP2();
        }

        summonTimer += Time.deltaTime;
    }
    Coroutine phase2Attack;
    IEnumerator Phase2Attack()
    {
        ai.SetSpeed(currentSettings.p2ChaseSpeed);

        p2HitAttack = false;
        canAttack = false;
        bool arrived = false;
        while(!arrived)
        {
            arrived = !ai.MoveTowardsTarget(player.transform.position);
            animator.SetFloat("x", Mathf.Clamp(rb.velocity.x, -1, 1));
            yield return null;
        }
        float timer = 0;
        while(timer < currentSettings.p2FollowPeriod)
        {
            ai.MoveTowardsTarget(player.transform.position);
            animator.SetFloat("x", Mathf.Clamp(rb.velocity.x, -1, 1));
            yield return null;
            timer += Time.deltaTime;
        }
        rb.velocity = Vector3.zero;
        animator.SetFloat("x", 0);

        yield return new WaitForSeconds(currentSettings.p2GracePeriod);

        animator.SetBool("isFlying", false);

        yield return new WaitForSeconds(2f);

        if (p2HitAttack)
        {
            collider.enabled = true;
            animator.SetBool("isFlying", true);

            yield return new WaitForSeconds(1.5f);

            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(currentSettings.p2AttackCooldown)));

            phase2Attack = null;
        }
        else
        {
            StartCoroutine(StunP2(currentSettings.p2StunLength, currentSettings.p2AttackCooldown));
        }

    }
    //Animation Event
    public void SlamAttack()
    {
        collider.enabled = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(p2AttackPoint.position, currentSettings.p2AttackRadius, p2AttackMask);
        foreach (Collider2D collider in colliders)
        {
            IHealth health = collider.GetComponent<IHealth>();
            if (health != null)
            {
                Vector2 dir = ((collider.transform.position - new Vector3(0, 0.12f)) - p2AttackPoint.position).normalized;
                if (health.TakeDamage(currentSettings.p2Damage))
                {
                    health.Knockback(dir, currentSettings.p2Knockback);

                    p2HitAttack = true;
                }
            }
        }

        if(enraged)
        {
            shooter.Attack();
        }
    }
    Coroutine stunP2Coroutine;
    IEnumerator StunP2(float stunTime, Vector2 attackCooldown)
    {
        isStunned = true;

        canAttack = false;
        yield return new WaitForSeconds(stunTime);

        animator.SetBool("isFlying", true);
        collider.enabled = false;

        yield return new WaitForSeconds(1.5f);

        isStunned = false;
        phase2Attack = null;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(attackCooldown)));
    }
    public void SummonP2()
    {
        for (int i = 0; i < (enraged ? 4 : 2); i++)
        {
            Vector2 spawnPoint;
            do
            {
                spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * currentSettings.p2SummonRadius;
            }
            while (Physics2D.OverlapCircle(spawnPoint, 0.24f, p2SummonMask) != null);

            GameObject enemyToSpawn = p2SummonEnemies[Random.Range(0, p2SummonEnemies.Length)];
            Instantiate(enemyToSpawn, spawnPoint, enemyToSpawn.transform.rotation);
        }
    }
    #endregion

    protected override void Idle()
    {
        currentState = States.Fighting;
        currentStageIndex = 1;
    }

    protected override void Init()
    {
        ai = GetComponent<AI>();
        animator = GetComponent<Animator>();
        shooter = GetComponent<BasicShooter>();

        currentSettings = normalSettings;
    }

    protected override void OnDialogueEnd()
    {
        
    }

    Coroutine attackCoroutine;
    IEnumerator AttackCooldown(float cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;

        currentAttack++;
        currentAttack = currentAttack % stages[currentStageIndex].attackSequence.Length;

        attackCoroutine = null;
    }

    public float GetRandomBetween(Vector2 v)
    {
        return Random.Range(v.x, v.y);
    }
}