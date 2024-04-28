using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballHelmetAbility : SpecialAbility
{
    [SerializeField] float launchForce = 1f;
    [SerializeField] float launchHeight = 0.5f;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject footballHelmetProjectile;
    [SerializeField] GameObject littleSteinPrefab;

    LittleStein littleStein;

    public override void Use(AttackController controller)
    {
        this.controller = controller;

        Vector2 dir = (controller.CrosshairPosition - (Vector2)transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, dir) * transform.rotation;
        GameObject go = Instantiate(footballHelmetProjectile, spawnPoint.position, Quaternion.identity);
        FootballHelmetProjectile proj = go.GetComponent<FootballHelmetProjectile>();
        if (proj) proj.Init(this);
        CustomRigidbody2D rb = go.GetComponent<CustomRigidbody2D>();
        if (rb) 
        {
            rb.Initialize(launchHeight);
            rb.AddForce(new Vector3(dir.x, dir.y, 0.5f) * launchForce, ForceMode2D.Impulse); 
        }
        isAttacking = true;
    }

    public void SpawnStein(Vector2 spawnPosition)
    {
        GameObject newStein = Instantiate(littleSteinPrefab, spawnPosition, Quaternion.identity);
        littleStein = newStein.GetComponent<LittleStein>();

        littleStein.SetPlayer(controller.gameObject.transform);

        durationCoroutine = StartCoroutine(AttackDuration());
    }

    Coroutine durationCoroutine;
    IEnumerator AttackDuration()
    {
        isAttacking = true;
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        if(littleStein != null)
            littleStein.SetCurrentState(LittleStein.States.Death);

        isAttacking = false;
    }

    public override void CancelUse()
    {
        if (durationCoroutine != null)
        {
            StopCoroutine(durationCoroutine);
            if (littleStein != null)
                littleStein.SetCurrentState(LittleStein.States.Death);

            isAttacking = false;
        }
    }

}
