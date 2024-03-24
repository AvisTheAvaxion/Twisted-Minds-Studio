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
            collision.GetComponent<HealthController>().TakeDamage(damage);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
