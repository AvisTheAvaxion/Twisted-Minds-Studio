using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [Header("Drop Settings")]
    [SerializeField] protected float dropRadius = 0.5f;
    [SerializeField] protected int minDropAmount;
    [SerializeField] protected int maxDropAmount;
    [SerializeField] protected GameObject itemHolder;
    [SerializeField] protected ItemDrop[] itemDrops;

    [Header("Other Settings")]
    [SerializeField] float health = 10;
    [SerializeField] GameObject destroyedVersion;
    [SerializeField] float destroyEffectLifetime = 2f;

    CustomRigidbody2D[] rigidBodies;

    float currentHealth;

    private void Start()
    {
        currentHealth = health;

        rigidBodies = GetComponentsInChildren<CustomRigidbody2D>();
    }

    protected virtual void SpawnItemDrops()
    {
        if (itemDrops == null || itemDrops.Length == 0) return;

        int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);
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
                Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
                if (rb) rb.AddForce(GetRandomDir() * dropRadius, ForceMode2D.Impulse);
            }
        }
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
            if (destroyedVersion)
            {
                GameObject go = Instantiate(destroyedVersion, transform.position, transform.rotation);
                rigidBodies = go.GetComponentsInChildren<CustomRigidbody2D>();
                for (int i = 0; i < rigidBodies.Length; i++)
                {
                    rigidBodies[i].gameObject.SetActive(true);
                    rigidBodies[i].transform.parent = null;
                    rigidBodies[i].Initialize(0);
                    Vector2 dir = GetRandomDir() * 2;
                    rigidBodies[i].AddForce(new Vector3(dir.x, dir.y, 0.5f), ForceMode2D.Impulse);
                }
            }
            Destroy(gameObject);
        }
    }
}
