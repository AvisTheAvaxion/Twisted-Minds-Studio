using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeStrike : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float frameRate;
    [SerializeField] string defaultTag = "Enemy";

    string targetTag;

    int damage;
    float knockback;
    float deflectionStrength;

    Effect[] effects;

    public void Init(int damage, float deflectionStrength, float knockback, Effect[] effects = null)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = defaultTag;

        this.effects = effects;

        StartCoroutine(Animate());
    }

    public void Init(int damage, float knockback, float deflectionStrength, string targetTag, Effect[] effects = null)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = targetTag;

        this.effects = effects;

        StartCoroutine(Animate());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null) { 
                health.TakeDamage(damage);

                if (effects != null)
                {
                    foreach (Effect effect in effects)
                    {
                        health.InflictEffect(effect);
                    }
                }
            }

            if (knockback > 0)
            {
                //Rigidbody2D otherRB = collision.attachedRigidbody;
                //otherRB.AddForce((collision.transform.position - transform.position).normalized * knockback, ForceMode2D.Impulse);
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
