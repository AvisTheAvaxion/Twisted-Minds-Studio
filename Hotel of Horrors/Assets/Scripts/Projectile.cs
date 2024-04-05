using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Wall"))
        {
            Destroy(gameObject);
        }
        else if(collision.tag.Equals("MeleeStrike"))
        {
            MeleeStrike meleeStrike = collision.GetComponent<MeleeStrike>();
            if(meleeStrike && meleeStrike.GetDeflectionStrength() > 0)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = Vector2.zero;
                Vector2 dir = Vector2.Reflect(-transform.up, collision.transform.up);
                rb.AddForce(dir * meleeStrike.GetDeflectionStrength() / 2f, ForceMode2D.Impulse);

                transform.rotation = Quaternion.FromToRotation(-transform.up, dir) * transform.rotation;

                if (tag.Equals("PlayerBullet")) tag = "EnemyBullet";
                if (tag.Equals("EnemyBullet")) tag = "PlayerBullet";
            }
        }
        else if ((this.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("Player")) || (this.tag.Equals("EnemyBullet") && !collision.gameObject.tag.Equals("Enemy")))
        {
            if (!collision.gameObject.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("EnemyBullet") && !collision.name.Equals("Enemy Spawner"))
            {
                IHealth health = collision.gameObject.GetComponent<IHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    Debug.Log(collision.name + " Destroyed " + gameObject.name);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((this.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("Player")) || (this.tag.Equals("EnemyBullet") && !collision.gameObject.tag.Equals("Enemy")))
        {
            if (!collision.gameObject.tag.Equals("PlayerBullet") && !collision.gameObject.tag.Equals("EnemyBullet") && !collision.name.Equals("Enemy Spawner"))
            {
                IHealth health = collision.gameObject.GetComponent<IHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    Debug.Log(collision.name + " Destroyed " + gameObject.name);
                    Destroy(gameObject);
                }
            }
        }
    }
}
