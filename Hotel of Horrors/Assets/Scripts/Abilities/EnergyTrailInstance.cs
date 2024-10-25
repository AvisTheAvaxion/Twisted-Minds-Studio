using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyTrailInstance : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;

    Collider2D collider;
    Animator animator;

    bool canDamage = true;
    float damageTimer = 0.5f;

    float damage;
    //float size;
    //float duration;

    public void Initialize(float damage, float size, float duration)
    {
        this.damage = damage;
        //this.size = size;
        //this.duration = duration;

        collider = GetComponent<Collider2D>();
        collider.enabled = true;

        transform.localScale *= size;

        animator = GetComponent<Animator>();

        canDamage = true;
        damageTimer = 0.5f;

        Invoke("DestroyTrail", duration);
    }

    public void DestroyTrail()
    {
        collider.enabled = false;
        animator.SetTrigger("Destroy");
        Destroy(gameObject, 2f);
    }

    public void StartPS()
    {
        particles.Play();
    }
    public void StopPS()
    {
        particles.Stop();
    }

    private void Update()
    {
        if(damageTimer > 0.5f)
        {
            canDamage = true;
        }
        damageTimer += Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canDamage && collision.CompareTag("Enemy"))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            canDamage = false;
            damageTimer = 0;
        }
    }
}
