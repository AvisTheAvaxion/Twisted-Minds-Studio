using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbility : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    public float Cooldown { get => cooldown; }
    public bool isAttacking { get; protected set; }

    public abstract void Use();
}
