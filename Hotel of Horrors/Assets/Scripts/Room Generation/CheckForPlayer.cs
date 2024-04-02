using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForPlayer : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;

    private void Start()
    {
        spawner = GetComponentInChildren<EnemySpawner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entering " + gameObject.name);
            spawner.isActiviated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exiting " + gameObject.name);
            spawner.isActiviated = false;
        }
    }
}
