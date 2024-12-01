using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JawbreakerBehavior : EnemyStateMachine
{
    [Header("References")]
    [SerializeField] GameObject slamEffect;

    public void SlamEffect()
    {
        slamEffect.SetActive(true);
    }
}
