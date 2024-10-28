using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    [SerializeField] protected float cooldown = 10f;
    [SerializeField] protected float duration = 1f;
    [SerializeField] protected float cameraShakeFrequency = 0.5f;
    public float Cooldown { get => cooldown; }
    public bool isAttacking { get; protected set; }

    protected ActionController controller;
    protected Ability ability;

    public abstract void Use(ActionController controller, Ability ability);

    public abstract void CancelAbility();
    //protected abstract IEnumerator Attack();
}
