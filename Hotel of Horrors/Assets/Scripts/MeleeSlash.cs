using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float frameRate;
    [SerializeField] string defaultTag = "Enemy";

    string targetTag;

    int damage;
    float knockback;
    float deflectionStrength;

    CameraShake cameraShake;

    Effect[] effects;

    [SerializeField] EnemyAudioManager enemyAudioManager;

    public void Init(int damage, float deflectionStrength, float knockback, CameraShake cameraShake, Effect[] effects = null)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = defaultTag;
        this.cameraShake = cameraShake;

        this.effects = effects;

        StartCoroutine(Animate());
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null) {
                if (health.TakeDamage(damage))
                {
                    enemyAudioManager.Damage();

                    if (knockback > 0)
                    {
                        Vector2 dir = (collision.transform.position - transform.position).normalized;
                        health.Knockback(dir, knockback);
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
                lootable.TakeDamage(damage);
            }
        }
    }

    public float GetDeflectionStrength()
    {
        return deflectionStrength;
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
