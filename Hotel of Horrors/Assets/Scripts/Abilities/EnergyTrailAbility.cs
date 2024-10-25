using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyTrailAbility : PlayerAbility
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] float timeBtwSpawns = 1f;
    [SerializeField] GameObject trailInstance;

    public override void Use(ActionController controller, Ability ability)
    {
        this.ability = ability;
        this.controller = controller;

        cooldown = ability.cooldown;
        duration = ability.duration;

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        float spawnTime = timeBtwSpawns;
        float t = 0;
        while(t < duration)
        {
            if(spawnTime > timeBtwSpawns)
            {
                controller.ShakeCamera(cameraShakeFrequency, 0.2f, false);

                GameObject go = Instantiate(trailInstance, spawnPoint.position, spawnPoint.rotation);
                EnergyTrailInstance instance = go.GetComponent<EnergyTrailInstance>();
                if(instance != null)
                {
                    instance.Initialize(ability.damage, ability.size, duration / 2);
                }

                spawnTime = 0;
            }
            yield return null;
            spawnTime += Time.deltaTime;
            t += Time.deltaTime;
        }

        isAttacking = false;
    }
}
