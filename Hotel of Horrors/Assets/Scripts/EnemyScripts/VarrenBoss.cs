using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VarrenBoss : BossStateMachine
{
    AI ai;
    Animator animator;
    EnemyAudioManager audio;

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

        [Header("Phase 3")]
        public float p3LaunchForce; //2
        public Vector2 p3LaunchCooldown; //8
        public Vector2 p3SlamCooldown; //10
        public float p3SlamDamage; //30
        public float p3StunLength; //7
        public float p3Knockback; //3f

        [Header("Phase 4")]
        public float p4LaunchForce;
        public Vector2 p4LaunchCooldown;
        public Vector2 p4DaggerCooldown;
        public float daggerGracePeriod;
        public float daggerLaunchForce;
        public float p4SummonCooldown;
        public float p4StunLength;
    }

    [Header("Varren Settings")]
    [SerializeField] AttackSettings normalSettings;
    [SerializeField] AttackSettings enrageSettings;
    [SerializeField] int startPhase;

    [Header("Phase 1")]
    [SerializeField] RuntimeAnimatorController p1AnimatorController;
    [SerializeField] Transform p1ProjectileSpawnPoint;
    [SerializeField] MultiProjectile[] p1Projectiles;
    [SerializeField] Transform[] teleportLocations;
    int currentTeleportLoc;
    float teleportTimer;

    [Header("Phase 2")]
    [SerializeField] RuntimeAnimatorController p2AnimatorController;
    [SerializeField] LayerMask p2AttackMask;
    [SerializeField] Transform p2AttackPoint;
    [SerializeField] GameObject[] p2SummonEnemies;
    [SerializeField] LayerMask p2SummonMask;
    float summonTimer;
    BasicShooter shooterP2;

    [Header("Phase 3")]
    [SerializeField] bool debugPhase3;
    [SerializeField] RuntimeAnimatorController p3AnimatorController;
    [SerializeField] GameObject[] p3SummonEnemies;
    [SerializeField] Transform leftArmAttackPoint;
    [SerializeField] Transform leftArmSlamStart;
    [SerializeField] Transform rightArmAttackPoint;
    [SerializeField] Transform rightArmSlamStart;
    [SerializeField] GameObject slamEffectPrefab;
    [SerializeField] Vector2 armSizeBounds;
    [SerializeField] BasicShooter shooter1P3;
    [SerializeField] Transform shooter1P3AttackPoint;
    [SerializeField] BasicShooter shooter2P3;
    [SerializeField] Collider2D rightArmCollider;
    [SerializeField] Collider2D leftArmCollider;
    [SerializeField] ParticleSystem p3BloodPS;

    [Header("Phase 4")]
    [SerializeField] RuntimeAnimatorController p4AnimatorController;
    [SerializeField] GameObject[] p4SummonEnemies;
    [SerializeField] GameObject[] p4AttackPatterns;
    [SerializeField] Transform p4launchPoint;
    [SerializeField] GameObject daggerPrefab;
    [SerializeField] Transform[] daggerLaunchPoints;
    [SerializeField] Collider2D headCollider;
    [SerializeField] Collider2D summonContainer;
    [SerializeField] LayerMask p4SummonMask;
    [SerializeField] ParticleSystem p4BloodPS;

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
        currentState = States.Death;

        yield return null;
        rb.velocity = Vector2.zero;
        //animator.SetTrigger("Death");
        Disenrage();

        if (currentStageIndex == 3)
        {
            SceneManager.LoadScene("EndDemo");
        }
        else
        {
            if (!dialogueSegmentStarted)
            {
                OnDialogueStart(stages[currentStageIndex].cutscene);
            }
            bossHealth.HideHealthBar();
        }
    }

    protected override IEnumerator DialogueEnd()
    {
        yield return new WaitForSeconds(0.1f);
        OnDialogueEnd();
    }

    protected override IEnumerator DialogueStart(Dialogue.Dialog cutscene)
    {
        dialogueSegmentStarted = true;

        //Cancel anything happening already
        yield return null;

        if (stunP2Coroutine != null) StopCoroutine(stunP2Coroutine);
        if (stunP3Coroutine != null) StopCoroutine(stunP3Coroutine);
        if (stunP4Coroutine != null) StopCoroutine(stunP4Coroutine);
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);

        isStunned = false;

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        DestroySummonEnemies();

        yield return new WaitForSeconds(0.1f);
        DialogueManager.SetCutscene(cutscene);
        currentState = States.Dialogue;
    }

    protected override void Disenrage()
    {
        enraged = false;
    }

    protected override void Enrage()
    {
        enraged = true;
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
            case 2:
                Phase3Fight();
                break;
            case 3:
                Phase4Fight();
                break;
        }
    }

    #region Phase 1
    protected void Phase1Fight()
    {
        if(canAttack && teleportTimer >= currentSettings.teleportCooldown)
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
        audio.PlaySound("SmallVarrenLaunchAttack");

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

        if(!isStunned && canAttack)
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
        audio.PlaySound("SmallVarrenSlam");

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
            shooterP2.Attack();
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

            GameObject enemyToSpawn = p2SummonEnemies[enraged ? 0 : 1];
            Instantiate(enemyToSpawn, spawnPoint, enemyToSpawn.transform.rotation);
        }
    }
    public void PlayP2WingSwoosh()
    {
        audio.PlaySound("SmallVarrenWingWoosh");
    }
    #endregion

    #region Phase 3
    Coroutine phase3Attack;
    protected void Phase3Fight()
    {
        if (!isStunned && canAttack)
        {
            switch (stages[currentStageIndex].attackSequence[currentAttack])
            {
                case 1:
                    phase3Attack = StartCoroutine(Phase3Attack());
                    break;
                case 2:
                    phase3Attack = StartCoroutine(Phase3Attack());
                    break;
                case 3:
                    phase3Attack = StartCoroutine(Phase3Slam()); 
                    break;
                case 4:
                    phase3Attack = StartCoroutine(Phase3Slam());
                    break;
            }
        }
    }
    public void Phase3AttackEvent()
    {
        doAttackP3 = true;
    }
    public void PlaySmallBloodAttackSound()
    {
        print("Play Small Blood Attack");
        switch (stages[currentStageIndex].attackSequence[currentAttack])
        {
            case 1:
                audio.PlaySound("BVBloodAttack1");
                break;
            case 2:
                audio.PlaySound("BVBloodAttack2");
                break;
        }
    }
    bool doAttackP3;
    IEnumerator Phase3Attack()
    {
        doAttackP3 = false;
        canAttack = false;
        switch (stages[currentStageIndex].attackSequence[currentAttack])
        {
            case 1:
                animator.SetBool("BloodAttack1", true);
                break;
            case 2:
                animator.SetBool("BloodAttack2", true);
                break;
        }

        yield return new WaitUntil(() => doAttackP3);
        p3BloodPS.Play();

        switch (stages[currentStageIndex].attackSequence[currentAttack])
        {
            case 1:
                shooter1P3.SetTarget(shooter1P3AttackPoint);
                shooter1P3.SetLaunchForce(currentSettings.p3LaunchForce);
                shooter1P3.Attack();
                yield return new WaitForSeconds(4.5f);
                break;
            case 2:
                shooter2P3.SetLaunchForce(currentSettings.p3LaunchForce);
                shooter2P3.Attack();
                yield return new WaitForSeconds(0.9f);
                break;
        }

        animator.SetBool("BloodAttack1", false);
        animator.SetBool("BloodAttack2", false);
        p3BloodPS.Stop();

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(currentSettings.p3LaunchCooldown)));

        phase3Attack = null;
    }
    public void Phase3SlamEvent()
    {
        doP3SlamAttack = true;
    }
    public void PlayBigSlamSound()
    {
        audio.PlaySound("BigVarrenSlam");
    }
    bool doP3SlamAttack;
    IEnumerator Phase3Slam()
    {
        bool hitPlayer = false;

        canAttack = false;
        doP3SlamAttack = false;

        Collider2D[] colliders = null;
        Vector2 slamPoint = Vector2.zero;

        animator.SetBool("Slam", true);
        //yield return new WaitUntil(() => doP3SlamAttack);

        if (stages[currentStageIndex].attackSequence[currentAttack] == 3)
        {
            rightArmCollider.enabled = true;
            leftArmCollider.enabled = false;
        }
        else
        {
            leftArmCollider.enabled = true;
            rightArmCollider.enabled = false;
        }

        if (stages[currentStageIndex].attackSequence[currentAttack] == 3)
        {
            slamPoint = rightArmAttackPoint.position;
            colliders = Physics2D.OverlapBoxAll(slamPoint, armSizeBounds, 0f, p2AttackMask);
            foreach (Collider2D collider in colliders)
            {
                IHealth health = collider.GetComponent<IHealth>();
                if (health != null)
                {
                    Vector2 dir = ((collider.transform.position - new Vector3(0, 0.12f)) - (Vector3)slamPoint).normalized;
                    if (health.TakeDamage(currentSettings.p3SlamDamage))
                    {
                        health.Knockback(new Vector2(dir.x, 0).normalized, currentSettings.p3Knockback);
                        hitPlayer = true;
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Destroy(Instantiate(slamEffectPrefab, (Vector2)rightArmSlamStart.position + Vector2.down * i * 0.24f, slamEffectPrefab.transform.rotation), 1.5f);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else if (stages[currentStageIndex].attackSequence[currentAttack] == 4)
        {
            slamPoint = leftArmAttackPoint.position;
            colliders = Physics2D.OverlapBoxAll(slamPoint, armSizeBounds, 0f, p2AttackMask);
            foreach (Collider2D collider in colliders)
            {
                IHealth health = collider.GetComponent<IHealth>();
                if (health != null)
                {
                    Vector2 dir = ((collider.transform.position - new Vector3(0, 0.12f)) - (Vector3)slamPoint).normalized;
                    if (health.TakeDamage(currentSettings.p3SlamDamage))
                    {
                        health.Knockback(new Vector2(dir.x, 0).normalized, currentSettings.p3Knockback);
                        hitPlayer = true;
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Destroy(Instantiate(slamEffectPrefab, (Vector2)leftArmSlamStart.position + Vector2.down * i * 0.24f, slamEffectPrefab.transform.rotation), 1.5f);
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(2f);
        animator.SetBool("Slam", false);

        if (hitPlayer)
        {
            animator.SetBool("SlamStuck", false);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(currentSettings.p3SlamCooldown)));
            phase3Attack = null;
        }
        else
        {
            animator.SetBool("SlamStuck", true);
            stunP3Coroutine = StartCoroutine(StunP3(currentSettings.p3StunLength, 
                stages[currentStageIndex].attackSequence[currentAttack] == 3, currentSettings.p3SlamCooldown));
        }

    }
    Coroutine stunP3Coroutine;
    IEnumerator StunP3(float stunTime, bool rightArm, Vector2 attackCooldown)
    {
        isStunned = true;

        canAttack = false;
        yield return new WaitForSeconds(stunTime);

        rightArmCollider.enabled = false;
        leftArmCollider.enabled = false;

        animator.SetBool("SlamStuck", false);

        yield return new WaitForSeconds(1.5f);

        isStunned = false;
        phase3Attack = null;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(attackCooldown)));
    }
    #endregion

    #region Phase 4
    public void Phase4Fight()
    {
        if(!isStunned && canAttack)
        {
            switch (stages[currentStageIndex].attackSequence[currentAttack])
            {
                case 0:
                    phase4Attack = StartCoroutine(Phase4Launch());
                    break;
                case 1:
                    phase4Attack = StartCoroutine(DaggerAttack());
                    break;
            }
        }

        if (summonTimer >= currentSettings.p4SummonCooldown)
        {
            summonTimer = 0;
            SummonP4();
        }

        summonTimer += Time.deltaTime;
    }
    public void Phase4AttackEvent()
    {
        doAttackP4 = true;
    }
    public void PlayBigBloodAttack()
    {
        audio.PlaySound("BigVarrenBloodAttack");
    }
    bool doAttackP4;
    Coroutine phase4Attack;
    IEnumerator Phase4Launch()
    {
        doAttackP4 = false;
        canAttack = false;
        animator.SetBool("BloodAttack", true);
        yield return new WaitUntil(() => doAttackP4);
        p4BloodPS.Play();

        //GameObject pattern = p4AttackPatterns[stages[currentStageIndex].attackSequence[currentAttack] % p4AttackPatterns.Length];
        GameObject pattern = p4AttackPatterns[Random.Range(0, p4AttackPatterns.Length)];
        for (int i = pattern.transform.childCount - 1; i >= 0; i--)
        {
            GameObject obj = Instantiate(pattern.transform.GetChild(i).gameObject, p4launchPoint.position, pattern.transform.rotation);
            MultiProjectile multi = obj.GetComponent<MultiProjectile>();
            if(multi != null)
            {
                multi.Initialize();
                multi.Launch(currentSettings.p4LaunchForce, Vector2.down);
                Destroy(obj, 10f);
            }
            yield return new WaitForSeconds(0.25f);
        }
        p4BloodPS.Stop();
        animator.SetBool("BloodAttack", false);

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(currentSettings.p4LaunchCooldown)));

        phase4Attack = null;
    }
    bool hitWithDagger = false;
    IEnumerator DaggerAttack()
    {
        hitWithDagger = false;
        canAttack = false;
        for (int i = 0; i < 5; i++)
        {
            Transform launchPoint = daggerLaunchPoints[Random.Range(0, daggerLaunchPoints.Length)];
            GameObject obj = Instantiate(daggerPrefab, launchPoint.position, daggerPrefab.transform.rotation);
            Destroy(obj, 10f);
            Vector2 dirToPlayer = (player.transform.position - launchPoint.position).normalized;
            obj.transform.rotation = Quaternion.FromToRotation(obj.transform.up, dirToPlayer) * obj.transform.rotation;
            TrajectoryProjectile proj = obj.GetComponent<TrajectoryProjectile>();
            if (proj != null)
            {
                proj.SetGracePeriod(currentSettings.daggerGracePeriod);
                proj.Launch(currentSettings.daggerLaunchForce);
            }
            yield return new WaitForSeconds(1f);
            if (hitWithDagger) break;
        }


        yield return new WaitForSeconds(3f);

        if (!hitWithDagger)
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(currentSettings.p4DaggerCooldown)));

            phase4Attack = null;
        }
    }
    public void SummonP4()
    {
        for (int i = 0; i < (enraged ? 4 : 2); i++)
        {
            Vector2 spawnPoint;
            do
            {
                spawnPoint = (Vector2)summonContainer.transform.position
                    + Vector2.right * Random.Range(-summonContainer.bounds.size.x/2, summonContainer.bounds.size.x/2)
                    + Vector2.up * Random.Range(-summonContainer.bounds.size.y/2, summonContainer.bounds.size.y/2);
            }
            while (Physics2D.OverlapCircle(spawnPoint, 0.24f, p4SummonMask) != null);

            GameObject enemyToSpawn = p4SummonEnemies[Random.Range(0, p4SummonEnemies.Length)];
            Instantiate(enemyToSpawn, spawnPoint, enemyToSpawn.transform.rotation);
        }
    }
    Coroutine stunP4Coroutine;
    IEnumerator StunP4(float stunTime, Vector2 attackCooldown)
    {
        animator.SetBool("Stun", true);
        isStunned = true;

        headCollider.enabled = true;

        canAttack = false;
        yield return new WaitForSeconds(stunTime);

        headCollider.enabled = false;

        animator.SetBool("Stun", false);
        yield return new WaitForSeconds(1.5f);

        isStunned = false;
        phase4Attack = null;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(GetRandomBetween(attackCooldown)));
    }
    #endregion
    public void PlayBigWingSwoosh()
    {
        audio.PlaySound("BigVarrenWingWoosh");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(currentStageIndex == 3 && !isStunned && collision.tag == "SpecialBullet")
        {
            hitWithDagger = true;
            stunP4Coroutine = StartCoroutine(StunP4(currentSettings.p4StunLength, currentSettings.p4DaggerCooldown));
        }
        if(currentStageIndex == 2)
        {

        }
    }

    protected override void Idle()
    {
        currentStageIndex = startPhase;
        if (currentStageIndex == 0) teleportTimer = currentSettings.teleportCooldown;

        if (currentStageIndex == 0)
        {
            if(GetDistanceToPlayer() < 2f)
                OnDialogueStart(openCutscene);
        } 
        else if (currentStageIndex == 2)
        {
            if (GetDistanceToPlayer() < 20f)
                OnDialogueStart(openCutscene);
        }
        else
        {
            currentState = States.Fighting;
        }
    }

    protected override void Init()
    {
        ai = GetComponent<AI>();
        animator = GetComponent<Animator>();
        shooterP2 = GetComponent<BasicShooter>();
        audio = GetComponentInChildren<EnemyAudioManager>();
        audio.MuteAudio();

        currentSettings = normalSettings;
    }

    void DestroySummonEnemies()
    {
        EnemyStateMachine[] enemies = FindObjectsOfType<EnemyStateMachine>();
        foreach (EnemyStateMachine enemy in enemies)
        {
            IHealth enemyHealth = enemy.GetComponent<IHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(100000);
            }
        }
    }

    protected override void OnDialogueEnd()
    {
        if (bossFightStarted)
        {
            int choiceMade = (int)FindObjectOfType<DialogueManager>().GetLastPlayerChoice();
            //print("Choice Made: " + choiceMade);
            if (!isDying)
            {
                if (stages[currentStageIndex].correctChoice != choiceMade)
                {
                    Enrage();
                }
                currentAttack = 0;
                currentStageIndex++;

                if(currentStageIndex == 1)
                {
                    animator.runtimeAnimatorController = p2AnimatorController;
                }
                else if(currentStageIndex == 2)
                {
                    animator.runtimeAnimatorController = p3AnimatorController;
                }
                else if (currentStageIndex == 3)
                {
                    animator.runtimeAnimatorController = p4AnimatorController;
                }
                
                if (currentStageIndex == 2 || currentStageIndex == 3)
                {
                    rightArmCollider.enabled = false;
                    leftArmCollider.enabled = false;
                }

                if (stunP2Coroutine != null) StopCoroutine(stunP2Coroutine);
                if (stunP3Coroutine != null) StopCoroutine(stunP3Coroutine);
                if (stunP4Coroutine != null) StopCoroutine(stunP4Coroutine);
                if (attackCoroutine != null) StopCoroutine(attackCoroutine);

                isAttacking = false;
                isStunned = false;
                canAttack = true;

                if (currentStageIndex != 2 && currentStageIndex != 3)
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                else
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;

                currentState = States.Fighting;
            }
            else
            {
                currentState = States.Death;
            }

            dialogueSegmentStarted = false;
        }
        else
        {
            currentState = States.Fighting;
            bossFightStarted = true;

            isAttacking = false;
            canAttack = true;

            currentAttack = 0;

            audio.UnMuteAudio();

            bossHealth.UpdateHealth();
            bossHealth.ShowHealthBar();

            dialogueSegmentStarted = false;

            if (currentStageIndex != 2 && currentStageIndex != 3)
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            else
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

            if (currentStageIndex == 2 || currentStageIndex == 3)
            {
                rightArmCollider.enabled = false;
                leftArmCollider.enabled = false;
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        if(debugPhase3)
        {
            if (leftArmAttackPoint != null)
                Gizmos.DrawWireCube(leftArmAttackPoint.position, new Vector3(armSizeBounds.x, armSizeBounds.y, 1));
            if (rightArmAttackPoint != null)
                Gizmos.DrawWireCube(rightArmAttackPoint.position, new Vector3(armSizeBounds.x, armSizeBounds.y, 1));
        }
    }
}
