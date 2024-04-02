using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullets : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("PlayerBullet") || collision.tag.Equals("EnemyBullet"))
        {
            Destroy(collision.gameObject);
        }
    }
}
