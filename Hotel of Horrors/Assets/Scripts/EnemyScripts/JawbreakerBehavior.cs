using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class JawbreakerBehavior : Lootable
{
    [Header("References")]
    [SerializeField] GameObject slamEffect;
    [SerializeField] GameObject shadow;
    [SerializeField] new BoxCollider2D collider;
    [SerializeField] Animator animator;
    [SerializeField] new SpriteRenderer renderer;
    [SerializeField] Sprite[] breakingSprites;
    
    private new void Start()
    {
        collider.enabled = false;
        base.Start();
    }

    public void SlamEffect()
    {
        slamEffect.SetActive(true);
        collider.enabled = true;
    }

    public override void TakeDamage(float amount)
    {
        switch (base.currentHealth--)
        {
            case 5:
                renderer.sprite = breakingSprites[0];
                break;
            case 4:
                renderer.sprite = breakingSprites[1];
                break;
            case 3:
                renderer.sprite = breakingSprites[2];
                break;
            case 2:
                renderer.sprite = breakingSprites[3];
                break;
            case 1:
                collider.enabled = false;
                shadow.SetActive(false);
                renderer.sprite = breakingSprites[4];
                animator.SetTrigger("Fade");
                Destroy(gameObject, 2.5f);
                break;
            default:
                break;
        }
    }
}
