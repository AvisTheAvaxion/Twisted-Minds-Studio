using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void TakeDamage(float amount, Effect effect = null);
    public void TakeDamage(float amount, float stunLength, Effect effect = null);
    public void Heal(float amount);
    public StatsController.Effector InflictEffect(Effect effect);
}
