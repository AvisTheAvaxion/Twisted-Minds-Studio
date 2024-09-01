using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float frameRate;
    [SerializeField] string defaultTag = "Enemy";

    Weapon weapon;

    string targetTag;

    int damage;
    float knockback;
    float deflectionStrength;

    CameraShake cameraShake;

    Effect[] effects;

    [SerializeField] EnemyAudioManager enemyAudioManager;

    public void Init(int damage, float knockback, float deflectionStrength, string targetTag, CameraShake cameraShake, Effect[] effects = null)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = targetTag;
        this.cameraShake = cameraShake;

        this.effects = effects;

        StartCoroutine(Animate());
    }
    public void Init(Weapon weapon, string targetTag, CameraShake cameraShake)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.weapon = weapon;

        EffectInfo[] effectInfos = weapon.GetInfo().GetEffectsToInflict();
        Effect[] effects = new Effect[effectInfos.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new Effect(effectInfos[i], weapon.GetInfo().GetChanceToInflictEffect());
        }

        this.targetTag = targetTag;
        this.cameraShake = cameraShake;

        StartCoroutine(Animate());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null) {
                if (health.TakeDamage(weapon != null ? weapon.damage : damage))
                {
                    enemyAudioManager.Damage();

                    if ((weapon != null ? weapon.knockback : knockback) > 0)
                    {
                        Vector2 dir = (collision.transform.position - transform.position).normalized;
                        health.Knockback(dir, weapon != null ? weapon.knockback : knockback);
                    }

                    if (cameraShake != null) cameraShake.ShakeCamera(0.5f);

                    if (effects != null)
                    {
                        foreach (Effect effect in effects)
                        {
                            health.InflictEffect(effect);
                        }
                    }
                }
            }
        } else
        {
            Lootable lootable = collision.gameObject.GetComponent<Lootable>();
            if (lootable != null)
            {
                lootable.TakeDamage(weapon != null ? weapon.damage : damage);
            }
        }
    }

    public float GetDeflectionStrength()
    {
        return weapon != null ? weapon.deflectionStrength : deflectionStrength;
    }

    IEnumerator Animate()
    {
        WaitForSeconds wait = new WaitForSeconds(frameRate);
        if (sprites.Length > 0)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return wait;
            }
        }
        Destroy(gameObject);
    }
}
