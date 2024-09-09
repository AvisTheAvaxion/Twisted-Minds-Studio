using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionalEnergy : MonoBehaviour
{
    public const int minEmotionalEnergyPickup = 5;
    public const int maxEmotionalEnergyPickup = 20;

    [SerializeField] float activationTime = 0.7f;
    [SerializeField] float attractionRange = 2;
    [SerializeField] float attractionForce = 2;
    [SerializeField] int defaultLayer;
    [SerializeField] int collectableLayer;
    public int emotionalEnergy { get; private set; }

    CustomRigidbody2D rb;

    float attractionRangeSqr;
    bool attractToPlayer;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attractionRangeSqr = attractionRange * attractionRange;

        attractToPlayer = false;

        rb = GetComponent<CustomRigidbody2D>();

        gameObject.layer = defaultLayer;
        Invoke("ActivateCollider", activationTime);

        if(emotionalEnergy <= 0) 
        {
            emotionalEnergy = 5;
        }
    }

    private void FixedUpdate()
    {
        if(attractToPlayer)
        {
            Vector2 vectorToPlayer = player.transform.position - transform.position;

            if(vectorToPlayer.sqrMagnitude <= attractionRangeSqr)
            {
                rb.AddForce(vectorToPlayer.normalized * attractionForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
            }
        }
    }

    public void SetEmotionalEnergy(int emotionalEnergy)
    {
        this.emotionalEnergy = emotionalEnergy;
    }
    void ActivateCollider()
    {
        gameObject.layer = collectableLayer;
        attractToPlayer = true;
        //GetComponent<Collider2D>().enabled = true;
    }
}
