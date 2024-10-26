using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyStompAbility : PlayerAbility
{
    [SerializeField] float knockbackForce = 3f;
    [SerializeField] GameObject stompObject;

    Collider2D collider;

    public override void CancelAbility()
    {
        /*isAttacking = false;
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            collider.enabled = false;
        }*/
    }

    public override void Use(ActionController controller, Ability ability)
    {
        this.ability = ability;
        this.controller = controller;

        cooldown = ability.cooldown;
        duration = ability.duration;

        collider = GetComponent<Collider2D>();
        collider.enabled = false;

        ((CircleCollider2D)collider).radius = ((CircleCollider2D)collider).radius * ability.size;

        attackCoroutine = StartCoroutine(Attack());
    }

    Coroutine attackCoroutine;
    protected IEnumerator Attack()
    {
        isAttacking = true;

        collider.enabled = true;
        stompObject.SetActive(true);

        controller.ShakeCamera(cameraShakeFrequency, duration, false);

        yield return new WaitForSeconds(duration);

        collider.enabled = false;
        isAttacking = false;

        yield return new WaitForSeconds(1f);

        stompObject.SetActive(false);
        attackCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(ability.damage);
                health.Knockback((collision.transform.position - stompObject.transform.position).normalized, knockbackForce);
            }
        }
    }
}
