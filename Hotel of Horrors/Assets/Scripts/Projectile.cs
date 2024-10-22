using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float hitStopLength = 0.05f;
    [SerializeField] float deflectionResistance;
    [SerializeField] [Range(0,1)] float chanceToInflictEffect;
    [SerializeField] EffectInfo[] effectsToInflict;
    [SerializeField] int maxTargets = 1;
    [SerializeField] int maxWallBounces = 1;
    [SerializeField] bool goThroughWalls = false;
    [SerializeField] GameObject impactEffect;

    protected Animator animator;

    protected Rigidbody2D rb;
    protected Collider2D collider;

    int targetsHit = 0;

    int wallBounces = 0;

    float forceDelay;

    protected virtual void Start()
    {
        targetsHit = 0;
        wallBounces = 0;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        collider = GetComponent<Collider2D>();
    }

    public virtual void Initialize(Ability ability)
    {
        damage = (int)ability.damage;
        deflectionResistance = ability.deflectionResistance;
        maxTargets = ability.maxTargets;
        goThroughWalls = ability.goThroughWalls;
        chanceToInflictEffect = ability.GetInfo().GetChanceToInflicEffects();
        effectsToInflict = ability.GetInfo().GetEffectsToInflict();

        forceDelay = 0;
    }
    public virtual void Initialize(Ability ability, float forceDelayIn)
    {
        damage = (int)ability.damage;
        deflectionResistance = ability.deflectionResistance;
        maxTargets = ability.maxTargets;
        goThroughWalls = ability.goThroughWalls;
        chanceToInflictEffect = ability.GetInfo().GetChanceToInflicEffects();
        effectsToInflict = ability.GetInfo().GetEffectsToInflict();

        forceDelay = forceDelayIn;
        collider = GetComponent<Collider2D>();
        collider.enabled = false;
    }
    public virtual void Initialize(Weapon weapon, int deflectionResistance, int maxTargets, bool goThroughWalls, float forceDelayIn)
    {
        this.deflectionResistance = deflectionResistance;
        this.maxTargets = maxTargets;
        this.goThroughWalls = goThroughWalls;

        damage = weapon.damage;
        chanceToInflictEffect = weapon.GetInfo().GetChanceToInflictEffect();
        effectsToInflict = weapon.GetInfo().GetEffectsToInflict();

        forceDelay = forceDelayIn;
        collider = GetComponent<Collider2D>();
        collider.enabled = false;
    }
    public virtual void Initialize(Weapon weapon, float forceDelayIn)
    {
        damage = weapon.damage;
        chanceToInflictEffect = weapon.GetInfo().GetChanceToInflictEffect();
        effectsToInflict = weapon.GetInfo().GetEffectsToInflict();

        forceDelay = forceDelayIn;
        collider = GetComponent<Collider2D>();
        collider.enabled = false;
    }

    public virtual void StartApplyForce(Vector2 force, ForceMode2D forceMode)
    {
        StartCoroutine(ApplyForce(force, forceMode));
    }

    IEnumerator ApplyForce(Vector2 force, ForceMode2D forceMode)
    {
        yield return new WaitForSeconds(forceDelay);
        collider.enabled = true;
        rb.AddForce(force, forceMode);
    }

    protected virtual void DestroySelf(bool hitWall = false)
    {
        if(hitWall)
        {
            Destroy(gameObject);
        } 
        else
        {
            rb.velocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetTrigger("Destroy");
                collider.enabled = false;
                Destroy(gameObject, 1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected bool CheckWallCollision(Collider2D collision)
    {
        return collision.tag.Equals("Wall") || collision.tag.Equals("WestDoor") || collision.tag.Equals("EastDoor") || collision.tag.Equals("NorthDoor") || collision.tag.Equals("SouthDoor");
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(CheckWallCollision(collision) && !goThroughWalls)
        {
            if (wallBounces < maxWallBounces)
            {
                Vector2 closestPoint = collision.ClosestPoint(transform.position);
                Vector2 normal = ((Vector2)transform.position - closestPoint).normalized;
                rb.velocity = Vector2.Reflect(rb.velocity, normal);

                transform.rotation = Quaternion.FromToRotation(transform.up, rb.velocity.normalized) * transform.rotation;

                wallBounces++;
            }
            else
            {
                //Spawn impact effect
                if (impactEffect != null)
                {
                    Vector2 impactSpawnPoint = collision.ClosestPoint(transform.position);
                    Vector2 rotateDir = ((Vector2)transform.position - impactSpawnPoint).normalized;
                    Quaternion rot = Quaternion.FromToRotation(impactEffect.transform.up, rotateDir) * impactEffect.transform.rotation;
                    Destroy(Instantiate(impactEffect, impactSpawnPoint, rot), 1.5f);
                }

                DestroySelf(true);
            }
        }
        else if(collision.tag.Equals("MeleeStrike"))
        {
            MeleeSlash meleeStrike = collision.GetComponent<MeleeSlash>();
            if(meleeStrike && meleeStrike.GetDeflectionStrength() > deflectionResistance)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                Vector2 dir = -rb.velocity.normalized;
                rb.velocity = Vector2.zero;
                rb.AddForce(dir * (meleeStrike.GetDeflectionStrength()), ForceMode2D.Impulse);

                transform.rotation = Quaternion.FromToRotation(transform.up, dir) * transform.rotation;

                if (tag.Equals("PlayerBullet")) tag = "EnemyBullet";
                if (tag.Equals("EnemyBullet")) tag = "PlayerBullet";
            } else
            {
                DestroySelf();
            }
        }
        else if ((this.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("Player")) || (this.tag.Equals("EnemyBullet") && !collision.gameObject.tag.Equals("Enemy")))
        {
            if (!collision.gameObject.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("EnemyBullet") && !collision.name.Equals("Enemy Spawner"))
            {
                IHealth health = collision.gameObject.GetComponent<IHealth>();
                if (health != null)
                {
                    if (health.TakeDamage(damage))
                    {
                        if (effectsToInflict != null)
                        {
                            foreach (EffectInfo effectInfo in effectsToInflict)
                            {
                                health.InflictEffect(new Effect(effectInfo, chanceToInflictEffect));
                            }
                        }

                        targetsHit++;
                        if (targetsHit >= maxTargets)
                            DestroySelf();

                        GameTime.AddHitStop(hitStopLength);
                    }
                }
                else
                {
                    Lootable lootable = collision.gameObject.GetComponent<Lootable>();
                    if (lootable != null)
                    {
                        lootable.TakeDamage(damage);
                        targetsHit++;
                        if(targetsHit >= maxTargets)
                            DestroySelf();
                    }
                }
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if ((this.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("Player")) || (this.tag.Equals("EnemyBullet") && !collision.gameObject.tag.Equals("Enemy")))
        {
            if (!collision.gameObject.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("EnemyBullet") && !collision.name.Equals("Enemy Spawner"))
            {
                IHealth health = collision.gameObject.GetComponent<IHealth>();
                if (health != null)
                {
                    if (health.TakeDamage(damage))
                    {
                        if (effectsToInflict != null)
                        {
                            foreach (EffectInfo effectInfo in effectsToInflict)
                            {
                                health.InflictEffect(new Effect(effectInfo, chanceToInflictEffect));
                            }
                        }
                        targetsHit++;
                        if (targetsHit >= maxTargets)
                            DestroySelf();
                    }
                }
            }
        }
    }
}
