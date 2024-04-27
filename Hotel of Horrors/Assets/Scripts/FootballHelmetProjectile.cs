using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomRigidbody2D))]
public class FootballHelmetProjectile : MonoBehaviour
{
    CustomRigidbody2D rb;

    FootballHelmetAbility footballHelmetAbility;

    public void Init(FootballHelmetAbility ability)
    {
        footballHelmetAbility = ability;

        rb = GetComponent<CustomRigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.height <= 0)
        {
            footballHelmetAbility.SpawnStein((Vector2)transform.position);
            Destroy(gameObject);
        }
    }
}
