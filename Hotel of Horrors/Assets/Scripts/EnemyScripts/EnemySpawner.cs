using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Description("Monsters will spawn until the queue is depleted")] List<GameObject> enemyQueue;
    [SerializeField, Description("The amount of monsters that will spawn every wave.")] int spawnPerWave;
    PolygonCollider2D spawnArea;
    Queue<GameObject> queue;

    public bool isActiviated;
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<GameObject>(enemyQueue);
        spawnArea = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        ActivateSpawning(isActiviated);
    }

    void ActivateSpawning(bool isActive)
    {
        if(isActive)
        {
            //If there are no enemies to spawn
            if (queue.Count <= 0)
            {

            }
            //If there are still enemies to spawn
            else
            {
                if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
                {
                    SpawnWave();
                }
            }
        }
    }

    //Spawns enemies based off Enemy Queue and Spawn Per Wave
    void SpawnWave()
    {
        Bounds spawnBound = spawnArea.bounds;
        //Find a random position for every enemy spawn in the wave
        for (int i = 0; i < spawnPerWave; i++)
        {
            if (queue.Count > 0)
            {
                Instantiate(queue.Dequeue(), GetRandomPosition(spawnBound), Quaternion.identity);
            }
            else
            {
                break;
            }
        }
    }

    //Gets random valid position of room
    Vector2 GetRandomPosition(Bounds bound)
    {
        //Loop that gets random positions until a position in the Spawnbounds is found
        while (true)
        {
            Vector2 randomSpawn = new Vector2(Random.Range(bound.min.x, bound.max.x), Random.Range(bound.min.y, bound.max.y));
            //End loop if valid pos
            if (spawnArea.OverlapPoint(randomSpawn))
            {
                return randomSpawn;
            }
            //Continue loop if non-valid pos
            else
            {
                continue;
            }
        }
    }
}
