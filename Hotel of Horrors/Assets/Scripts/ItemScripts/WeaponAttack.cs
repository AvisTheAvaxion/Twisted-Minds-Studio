using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Damage Attempt");
        if (collision.CompareTag("Enemy"))
        {
            //Debug.Log("Damageing");
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
