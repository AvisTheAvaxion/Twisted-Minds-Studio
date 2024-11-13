using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VarrenBoss : BossStateMachine
{
    AI ai;

    [Header("Phase 1")]
    [SerializeField] Transform p1ProjectileSpawnPoint;
    [SerializeField] MultiProjectile[] p1Projectiles;
    [SerializeField] float p1LaunchForce = 5f;
    [SerializeField] float p1AttackCooldown = 7;
    [SerializeField] float teleportCooldown = 15;
    [SerializeField] Transform[] teleportLocations;
    int currentTeleportLoc;
    float teleportTimer;

    [Header("Phase 2")]
    [SerializeField] float travelHeight;
    [SerializeField] float p2AttackCooldown;
    [SerializeField] float p2StunLength;
    [SerializeField] float p2SummonCooldown;

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
        if(teleportTimer > teleportCooldown)
        {
            teleportTimer = 0;
            StartCoroutine(Teleport());
        }
        else if(canAttack)
        {
            StartCoroutine(ShootP1Projectile());
        }

        teleportTimer += Time.deltaTime;
    }

    IEnumerator ShootP1Projectile()
    {
        canAttack = false;
        MultiProjectile projectile = Instantiate(p1Projectiles[stages[currentStageIndex].attackSequence[currentAttack]].gameObject, p1ProjectileSpawnPoint.position, p1ProjectileSpawnPoint.rotation).GetComponent<MultiProjectile>();
        yield return new WaitForSeconds(0.5f);
        projectile.Launch(p1LaunchForce, Vector2.up);

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCooldown(p1AttackCooldown));
    }
    IEnumerator Teleport()
    {
        canAttack = false;

        yield return new WaitForSeconds(0.5f);

        int nextTeleportLoc;
        do
        {
            nextTeleportLoc = Random.Range(0, teleportLocations.Length);
        }
        while (nextTeleportLoc == currentTeleportLoc);

        currentTeleportLoc = nextTeleportLoc;
        transform.position = teleportLocations[currentTeleportLoc].position;

        yield return new WaitForSeconds(0.5f);

        canAttack = true;
        teleportTimer = 0;
    }
    #endregion

    #region Phase 2
    protected void Phase2Fight()
    {

    }

    IEnumerator Phase2Attack()
    {
        while(ai.MoveTowardsTarget(player.transform.position + Vector3.up * travelHeight))
        {

        }
        yield return null;
    }
    #endregion

    protected override void Idle()
    {
        
    }

    protected override void Init()
    {
        ai = GetComponent<AI>();
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
    }
}
