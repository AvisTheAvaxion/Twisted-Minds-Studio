using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
