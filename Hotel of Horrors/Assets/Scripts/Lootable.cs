using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [Header("Drop Settings")]
    [SerializeField] protected float dropRadius = 0.5f;
    [SerializeField] protected float destroyedLaunchForce = 10f;
    [SerializeField] protected float startHeight = 0.2f;
    [SerializeField] protected float upwardsVelocity = 1f;
    [SerializeField] protected int minDropAmount;
    [SerializeField] protected int maxDropAmount;
    [SerializeField] protected AnimationCurve dropAmountChance;
    [SerializeField] protected GameObject itemHolder;
    [SerializeField] protected ItemDrop[] itemDrops;

    [Header("Other Settings")]
    [SerializeField] float health = 10;
    [SerializeField] GameObject[] destroyedPieces;
    QuestSystem questSystem;

    CustomRigidbody2D[] rigidBodies;

    float currentHealth;

    [SerializeField] GameObject DeathSound;

    private void Start()
    {
        currentHealth = health;

        questSystem = FindObjectOfType<QuestSystem>();
        rigidBodies = GetComponentsInChildren<CustomRigidbody2D>();
    }

    protected virtual void SpawnItemDrops()
    {
        if (itemDrops == null || itemDrops.Length == 0) return;

        int dropAmount = Mathf.RoundToInt(Mathf.Lerp(minDropAmount, maxDropAmount, dropAmountChance.Evaluate(Random.Range(0, 1f))));
        float weightTotal = 0;
        for (int i = 0; i < itemDrops.Length; i++)
        {
            weightTotal += itemDrops[i].weight;
        }


        for (int i = 0; i < dropAmount; i++)
        {
            float rand = Random.Range(0, weightTotal);
            int e = 0;
            for (; e < itemDrops.Length; e++)
            {
                if (rand < itemDrops[e].weight)
                    break;
                rand -= itemDrops[e].weight;
            }
            if (e < itemDrops.Length)
            {
                GameObject go = Instantiate(itemHolder, transform.position, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.forward));
                ItemData data = go.GetComponent<ItemData>();
                if (data) data.SetItemData(itemDrops[e].item, Random.Range(itemDrops[e].minCount, itemDrops[e].maxCount + 1));
                CustomRigidbody2D rb = go.GetComponent<CustomRigidbody2D>();
                if (rb) 
                {
                    rb.Initialize(startHeight, Random.Range(0f, upwardsVelocity));
                    rb.AddForce(GetRandomDir() * dropRadius, ForceMode2D.Impulse); 
                }
            }
        }

        Instantiate(DeathSound);
    }

    Vector2 GetRandomDir()
    {
        return Random.insideUnitCircle; //Get a random position around a unit circle.
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            SpawnItemDrops();
            for (int i = 0; i < destroyedPieces.Length; i++)
            {
                GameObject go = Instantiate(destroyedPieces[i], transform.position + destroyedPieces[i].transform.localPosition, destroyedPieces[i].transform.rotation);
                CustomRigidbody2D rigidBody = go.GetComponent<CustomRigidbody2D>();
                if (rigidBody)
                {
                    rigidBody.gameObject.SetActive(true);
                    rigidBody.transform.parent = null;
                    rigidBody.Initialize(0);
                    //Vector2 dir = (go.transform.position - transform.position).normalized * destroyedLaunchForce;
                    Vector2 dir = GetRandomDir().normalized * destroyedLaunchForce;
                    rigidBody.AddForce(new Vector3(dir.x, dir.y, 0.5f), ForceMode2D.Impulse);
                }
            }
            questSystem.QuestEvent(QuestSystem.QuestEventType.LootableBroken, null);
            Destroy(gameObject);
        }
    }
}
