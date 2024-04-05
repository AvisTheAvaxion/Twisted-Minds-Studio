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

    public void Init(int damage, float deflectionStrength, float knockback)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = defaultTag;

        StartCoroutine(Animate());
    }

    public void Init(int damage, float knockback, float deflectionStrength, string targetTag)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = targetTag;

        StartCoroutine(Animate());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null) health.TakeDamage(damage);

            if (knockback > 0)
            {
                //Rigidbody2D otherRB = collision.attachedRigidbody;
                //otherRB.AddForce((collision.transform.position - transform.position).normalized * knockback, ForceMode2D.Impulse);
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
