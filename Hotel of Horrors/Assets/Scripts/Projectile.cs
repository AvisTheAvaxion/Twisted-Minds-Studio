using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.tag.Equals("Player") || !collision.tag.Equals("Enemy"))
        {
            return;
        }

        if ((this.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("Player")) || (this.tag.Equals("EnemyBullet") && !collision.gameObject.tag.Equals("Enemy")))
        {
            if (!collision.gameObject.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("EnemyBullet") && !collision.name.Equals("Enemy Spawner"))
            {
                IHealth health = collision.gameObject.GetComponent<IHealth>();
                if (health != null) health.TakeDamage(damage);
                print(collision.name + " Destroyed " + gameObject.name);
                Destroy(gameObject);
            }
        }
    }
}
