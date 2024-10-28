using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlastAbility : PlayerAbility
{
    [SerializeField] float maxRange = 3f;
    [SerializeField] float blastDelay = 0.12f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] BoxCollider2D collider2D;
    [SerializeField] LayerMask blockBlastMask;
    [SerializeField] Transform blast;
    [SerializeField] Transform blastEnd;

    Vector3 localPos;

    private void Start()
    {
        localPos = blast.localPosition;
    }

    public override void Use(ActionController controller, Ability ability)
    {
        this.ability = ability;
        this.controller = controller;

        cooldown = ability.cooldown;
        duration = ability.duration;
        maxRange = ability.range;

        Projectile projectile = blast.GetComponent<Projectile>();
        if (projectile) projectile.Initialize(ability);

        attackCoroutine = StartCoroutine(Attack());
    }

    Coroutine attackCoroutine;
    protected IEnumerator Attack()
    {
        blast.gameObject.SetActive(true);

        Vector2 defaultTexScale = lineRenderer.textureScale;
        isAttacking = true;
        //Entrance animation
        float timer = 0;

        while(timer < blastDelay)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)transform.position).normalized) * transform.rotation;
            blast.localPosition = localPos;
            yield return null;
            timer += Time.deltaTime;
        }

        lineRenderer.textureScale = defaultTexScale;

        blastEnd.gameObject.SetActive(true);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Vector3.zero);
        collider2D.enabled = true;
        blast.localPosition = localPos;
        controller.ShakeCamera(cameraShakeFrequency, duration, false);
        timer = 0;
        while(timer < duration)
        {
            Vector2 dir = (controller.CrosshairPosition - (Vector2)transform.position).normalized;
            RaycastHit2D hitInfo = Physics2D.Raycast(blast.position, dir, maxRange, blockBlastMask);
            float distance = maxRange;
            if(hitInfo.collider != null)
            {
                distance = hitInfo.distance;
            }
            lineRenderer.SetPosition(1, Vector3.up * distance);
            blastEnd.localPosition = Vector3.up * distance;
            collider2D.offset = new Vector2(0, distance / 2);
            collider2D.size = new Vector2(collider2D.size.x, distance);
            transform.rotation = Quaternion.FromToRotation(transform.up, (controller.CrosshairPosition - (Vector2)transform.position).normalized) * transform.rotation;
            blast.localPosition = localPos;
            yield return null;
            timer += Time.deltaTime;
        }

        //Exit animation
        lineRenderer.enabled = false;
        collider2D.enabled = false;
        blastEnd.gameObject.SetActive(false);
        blast.gameObject.SetActive(false);

        isAttacking = false;
        attackCoroutine = null;
    }

    public override void CancelAbility()
    {
        isAttacking = false;

        if (attackCoroutine != null)
        {
            lineRenderer.enabled = false;
            collider2D.enabled = false;
            blastEnd.gameObject.SetActive(false);
            blast.gameObject.SetActive(false);
            StopCoroutine(attackCoroutine);
        }
    }
}
