using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float deflectionResistance;
    [SerializeField] [Range(0,1)] float chanceToInflictEffect;
    [SerializeField] EffectInfo[] effectsToInflict;
    [SerializeField] int maxTargets = 1;
    [SerializeField] int maxWallBounces = 1;
    [SerializeField] bool goThroughWalls = false;

    Rigidbody2D rb;

    int targetsHit = 0;

    int wallBounces = 0;

    private void Start()
    {
        targetsHit = 0;
        wallBounces = 0;

        rb = GetComponent<Rigidbody2D>();
    } 

    public void Initialize(Ability ability)
    {
        damage = (int)ability.damage;
        deflectionResistance = ability.deflectionResistance;
        maxTargets = ability.maxTargets;
        goThroughWalls = ability.goThroughWalls;
        chanceToInflictEffect = ability.GetInfo().GetChanceToInflicEffects();
        effectsToInflict = ability.GetInfo().GetEffectsToInflict();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Wall") && !goThroughWalls)
        {
            if (wallBounces < maxWallBounces)
            {
                Vector2 closestPoint = collision.ClosestPoint(transform.position);
                Vector2 normal = ((Vector2)transform.position - closestPoint).normalized;
                rb.velocity = Vector2.Reflect(rb.velocity, normal);

                wallBounces++;
            } else
                Destroy(gameObject);
        }
        else if(collision.tag.Equals("MeleeStrike"))
        {
            MeleeSlash meleeStrike = collision.GetComponent<MeleeSlash>();
            if(meleeStrike && meleeStrike.GetDeflectionStrength() > deflectionResistance)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = Vector2.zero;
                Vector2 dir = Vector2.Reflect(-transform.up, collision.transform.up);
                rb.AddForce(dir * (meleeStrike.GetDeflectionStrength() - deflectionResistance) / 2f, ForceMode2D.Impulse);

                transform.rotation = Quaternion.FromToRotation(-transform.up, dir) * transform.rotation;

                if (tag.Equals("PlayerBullet")) tag = "EnemyBullet";
                if (tag.Equals("EnemyBullet")) tag = "PlayerBullet";
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
                            Destroy(gameObject);
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
                            Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
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
                            Destroy(gameObject);
                    }
                }
            }
        }
    }
}
