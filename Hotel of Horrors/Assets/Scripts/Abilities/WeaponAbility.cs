using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbility : MonoBehaviour
{
    [SerializeField] protected float duration = 1f;
    [SerializeField] protected float cameraShakeFrequency = 0.5f;
    protected Weapon weapon;
    public bool isAttacking { get; protected set; }

    protected ActionController controller;

    public abstract void Use(ActionController controller, Weapon weapon);
}
