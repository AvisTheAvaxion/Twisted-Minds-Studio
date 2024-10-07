using System;
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
    QuestSystem questSys;
    [SerializeField] List<GameObject> spawnedMonsters;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float obstacleCheckRadius = 0.2f;
    public bool isActiviated;

    public Floor floor { get; private set; }

    private void Awake()
    {
        questSys = FindObjectOfType<QuestSystem>();
    }

    void Start()
    {
        spawnedMonsters = new List<GameObject>();
        queue = new Queue<GameObject>(enemyQueue);
        spawnArea = GetComponent<PolygonCollider2D>();
        floor = FindObjectOfType<Floor>();
    }

    void Update()
    {
        ActivateSpawning(isActiviated);
    }

    #region Enemy Spawning
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
                GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
                if (objects.Length <= 0 || (objects.Length == 1 && objects[0].GetComponent<BossStateMachine>()))
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
                floor.AddEnemy();
                GameObject monster = Instantiate(queue.Dequeue(), GetRandomPosition(spawnBound), Quaternion.identity);
                monster.GetComponent<EnemyStateMachine>().OnEnemyDeath += EnemyDeathDetected;

                EnemyStateMachine esm = monster.GetComponent<EnemyStateMachine>();
                if(esm)
                {
                    esm.SetSpawner(this);
                }
            }
            else
            {
                break;
            }
        }
    }

    //Gets random valid position of room
    public Vector2 GetRandomPosition(Bounds bound)
    {
        //Loop that gets random positions until a position in the Spawnbounds is found
        while (true)
        {
            Vector2 randomSpawn = new Vector2(UnityEngine.Random.Range(bound.min.x, bound.max.x), UnityEngine.Random.Range(bound.min.y, bound.max.y));
            //End loop if valid pos
            if (spawnArea.OverlapPoint(randomSpawn) && Physics2D.OverlapCircle(randomSpawn, obstacleCheckRadius, obstacleMask) == null)
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

    public Vector2 GetRandomPosition(Vector2 pos, float radius)
    {
        //Loop that gets random positions until a position in the Spawnbounds is found
        while (true)
        {
            Vector2 randomSpawn = pos + UnityEngine.Random.insideUnitCircle * radius;
            //End loop if valid pos
            if (spawnArea.OverlapPoint(randomSpawn) && Physics2D.OverlapCircle(randomSpawn, obstacleCheckRadius, obstacleMask) == null)
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
    #endregion

    void EnemyDeathDetected(object sender, EventArgs e)
    {
        string enemyName = (string) sender;
        questSys.QuestEvent(QuestSystem.QuestEventType.EnemyDeath, enemyName);
    }

    public void ResetSpawner()
    {
        isActiviated = false;
        queue = new Queue<GameObject>(enemyQueue);
    }
}
