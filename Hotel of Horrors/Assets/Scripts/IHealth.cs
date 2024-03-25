using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void TakeDamage(float amount);
    public void TakeDamage(float amount, float stunLength);
    public void Heal(float amount);
}
