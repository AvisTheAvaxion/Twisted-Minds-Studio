using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    /// <summary>
    /// Applies the given damage and applies any effects if given
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="effect"></param>
    public void TakeDamage(float amount, Effect effect = null);
    /// <summary>
    /// Applies the given damage and optional effects and stuns the character for given stun length
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="stunLength"></param>
    /// <param name="effect"></param>
    public void TakeDamage(float amount, float stunLength, Effect effect = null);
    /// <summary>
    /// Heals the character for given amount
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount);
    /// <summary>
    /// Inflicts the given effect on the character
    /// </summary>
    /// <param name="effect"></param>
    /// <returns>Returns the created effector to allow for any subscriptions to the provided events</returns>
    public StatsController.Effector InflictEffect(Effect effect);
    /// <summary>
    /// Method to force the update of any health visuals
    /// </summary>
    public void UpdateHealth();
    /// <summary>
    /// Applys knockback to character in dir at given strength
    /// </summary>
    /// <param name="from"></param>
    /// <param name="strength"></param>
    public void Knockback(Vector3 dir, float strength);
}
