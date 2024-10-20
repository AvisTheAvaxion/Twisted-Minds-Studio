using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSlash : MonoBehaviour
{
    [System.Serializable]
    public struct FrameSorting
    {
        public int[] frameLayers;

        public FrameSorting(int[] frameLayersIn)
        {
            this.frameLayers = frameLayersIn;
        }
    }

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider;
    [SerializeField] Sprite[] sprites;
    [SerializeField] int frameRate;
    //[SerializeField] string defaultTag = "Enemy";
    //[SerializeField] bool faceUp = false;
    [SerializeField] FrameSorting[] frameSorting = new FrameSorting[] { 
        new FrameSorting(new int[] { 2, 2, 2 }), //0
        new FrameSorting(new int[] { 2, 2, 2 }), //1
        new FrameSorting(new int[] { 2, 2, 2 }), //2
        new FrameSorting(new int[] { 0, 0, 0 }), //3
        new FrameSorting(new int[] { 0, 0, 0 }), //4
        new FrameSorting(new int[] { 0, 0, 0 }), //5
        new FrameSorting(new int[] { 2, 2, 2 }), //6
        new FrameSorting(new int[] { 2, 2, 2 })  //7
    };

    Weapon weapon;
    int attackNum;

    string targetTag;

    int damage;
    float knockback;
    float deflectionStrength;
    float hitStopLength;

    CameraShake cameraShake;

    ActionController actionController;

    Effect[] effects;

    [SerializeField] EnemyAudioManager enemyAudioManager;

    int sortingID;

    public void Init(ActionController actionController, float hitStopLengthIn, int damage, float knockback, float deflectionStrength, string targetTag, CameraShake cameraShake, int sortingIDIn, Effect[] effects = null)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.actionController = actionController;

        this.damage = damage;
        this.knockback = knockback;
        this.deflectionStrength = deflectionStrength;
        this.targetTag = targetTag;
        this.cameraShake = cameraShake;
        hitStopLength = hitStopLengthIn;

        sortingID = sortingIDIn;

        this.effects = effects;

        if (collider == null) collider = GetComponent<Collider2D>();
        collider.enabled = false;

        StartCoroutine(Animate());
    }
    public void Init(ActionController actionController, Weapon weapon, int attackNumIn, string targetTag, CameraShake cameraShake, int sortingIDIn)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        this.actionController = actionController;

        this.weapon = weapon;
        attackNum = attackNumIn;

        EffectInfo[] effectInfos = weapon.GetInfo().GetEffectsToInflict();
        Effect[] effects = new Effect[effectInfos.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new Effect(effectInfos[i], weapon.GetInfo().GetChanceToInflictEffect());
        }

        this.targetTag = targetTag;
        this.cameraShake = cameraShake;

        sortingID = sortingIDIn;

        if (collider == null) collider = GetComponent<Collider2D>();
        collider.enabled = false;

        StartCoroutine(Animate());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null) {
                if (health.TakeDamage(weapon != null ? weapon.GetDamage(actionController.attackNumber) : damage))
                {
                    enemyAudioManager.Damage();

                    if ((weapon != null ? weapon.GetKnockback(actionController.attackNumber) : knockback) > 0)
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

                    GameTime.AddHitStop(weapon != null ? weapon.GetInfo().GetAttack(attackNum).hitStopLength : hitStopLength);
                }
            }
        } else
        {
            Lootable lootable = collision.gameObject.GetComponent<Lootable>();
            if (lootable != null)
            {
                lootable.TakeDamage(weapon != null ? weapon.GetDamage(actionController.attackNumber) : damage);
            }
        }
    }

    public float GetDeflectionStrength()
    {
        return weapon != null ? weapon.deflectionStrength : deflectionStrength;
    }

    IEnumerator Animate()
    {
        /*if (faceUp)
        {
            print(transform.eulerAngles);
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }*/

        WaitForSeconds wait = new WaitForSeconds(1f / frameRate);
        if (sprites.Length > 0)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sortingOrder = frameSorting[sortingID].frameLayers[i];
                if (i > 0) collider.enabled = true;
                if (i >= sprites.Length - 1) collider.enabled = false;

                spriteRenderer.sprite = sprites[i];
                yield return wait;
            }
        }
        Destroy(gameObject);

        actionController.AttackEnd();
    }
}
