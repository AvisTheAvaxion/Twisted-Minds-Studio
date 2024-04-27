using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AI : MonoBehaviour
{
    public struct Weight
    {
        public float weight;
        public Vector2 dir;
    }

    Rigidbody2D rb;

    Vector2 unobstructedDir;
    int unobstructedIndex = 0;

    bool pathObstructed = false;

    [SerializeField] Transform target;
    [Header("Settings")]
    [SerializeField] float currentDirSmoothing = 0.5f;
    [SerializeField] float objectRadius = 0.2f;
    [SerializeField] float speed = 2f;
    [SerializeField] float orbitRadius = 1f;
    [SerializeField] float orbitRadiusThickness = 0.2f;
    float orbitOffset;
    [Header("Weight Settings")]
    [SerializeField][Range(4, 24)] int numOfWeights = 16;
    [SerializeField] bool visualizeWeights = true;
    [Header("Obstacle Settings")]
    [SerializeField][Range(0.01f, 1000f)] float obstacleDistanceSensitivity = 1;
    [SerializeField] float desireKeepSameDir = 0.1f;
    [SerializeField] float noObstacleDesire = 0.3f;
    [SerializeField] string comradeTag = "Enemy";
    [SerializeField] LayerMask obstacleMask;
    [Header("Path Obstruction Settings")]
    [Tooltip("Weight value to identity a path obstruction")]
    [SerializeField] float obstructionDetectionThreshold = 0.1f;
    [Tooltip("Maximum weight value needed to change current direction when a path obstruction is detected")]
    [SerializeField] float maxObstructionChangeDir = 0f;
    [Tooltip("Minimum weight value of current direction needed to change current direction when a path obstruction is detected")]
    [SerializeField] float minObstructionChangeDir = -0.8f;
    Weight[] weights;

    Vector2 newDir;
    Vector2 currentDir;

    bool canMove = true;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        orbitOffset = Random.Range(0, orbitRadius / 2f);

        //Initialize the weights and their directions
        float angle = 0;
        float dAngle = 360f / numOfWeights;
        weights = new Weight[numOfWeights];
        for (int i = 0; i < numOfWeights; i++)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            weights[i].weight = 0;
            weights[i].dir = new Vector2(x, y).normalized;

            angle += dAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
       /* if (canMove)
        {
            OrbitAroundTarget();
        }*/
    }

    private void Move()
    {
        currentDir = Vector2.Lerp(currentDir, newDir, currentDirSmoothing * Time.deltaTime * 10f);
        rb.velocity = currentDir * speed;
    }

    public void MoveTowardsTarget()
    {
        if (!canMove || target == null) return;

        Vector2 dirToTarget = (target.position - transform.position).normalized;

        float largestWeight = 0;
        //Set baseline weight based off dot product to desire direction
        for (int i = 0; i < numOfWeights; i++)
        {
            float w = Vector2.Dot(weights[i].dir, dirToTarget);
            weights[i].weight = w;

            float w2 = Vector2.Dot(weights[i].dir, currentDir);
            weights[i].weight += w2 * desireKeepSameDir;
        }

        ObstacleDetection(false);

        //Find largest weight and head in that direction
        largestWeight = weights[0].weight;
        int largestWeightIndex = 0;
        newDir = weights[0].dir;
        for (int i = 1; i < numOfWeights; i++)
        {
            if (weights[i].weight > largestWeight)
            {
                newDir = weights[i].dir;
                largestWeightIndex = i;
                largestWeight = weights[i].weight;
            }
        }

        Move();
    }
    public void MoveTowardsTarget(Vector3 targetPosition)
    {
        if (!canMove) return;

        Vector2 dirToTarget = (targetPosition - transform.position).normalized;

        float largestWeight = 0;
        //Set baseline weight based off dot product to desire direction
        for (int i = 0; i < numOfWeights; i++)
        {
            float w = Vector2.Dot(weights[i].dir, dirToTarget);
            weights[i].weight = w;

            float w2 = Vector2.Dot(weights[i].dir, currentDir);
            weights[i].weight += w2 * desireKeepSameDir;
        }

        ObstacleDetection(false);

        //Find largest weight and head in that direction
        largestWeight = weights[0].weight;
        int largestWeightIndex = 0;
        newDir = weights[0].dir;
        for (int i = 1; i < numOfWeights; i++)
        {
            if (weights[i].weight > largestWeight)
            {
                newDir = weights[i].dir;
                largestWeightIndex = i;
                largestWeight = weights[i].weight;
            }
        }

        Move();
    }

    bool isOrbiting;
    public void OrbitAroundTarget()
    {
        if (!canMove || target == null) return;

        Vector2 vectorToTarget = target.position - transform.position;
        Vector2 dirToTarget = vectorToTarget.normalized;

        float distToTarget = vectorToTarget.magnitude;

        if (distToTarget > orbitRadius + orbitOffset + orbitRadiusThickness)
            isOrbiting = false;
        else if (distToTarget <= orbitRadius + orbitOffset)
            isOrbiting = true;

        float largestWeight = 0;
        //Set baseline weight based off dot product to desire direction
        for (int i = 0; i < numOfWeights; i++)
        {
            float w = Vector2.Dot(weights[i].dir, dirToTarget);

            if (isOrbiting)
            {
                w = 1 - Mathf.Abs(w);
            }

            weights[i].weight = w;

            float w2 = Vector2.Dot(weights[i].dir, currentDir);
            weights[i].weight += w2 * desireKeepSameDir;
        }

        ObstacleDetection(isOrbiting);

        //Find largest weight and head in that direction
        largestWeight = weights[0].weight;
        int largestWeightIndex = 0;
        newDir = weights[0].dir;
        for (int i = 1; i < numOfWeights; i++)
        {
            if (weights[i].weight > largestWeight)
            {
                newDir = weights[i].dir;
                largestWeightIndex = i;
                largestWeight = weights[i].weight;
            }
        }

        PathObstruction(largestWeight, largestWeightIndex);

        Move();
    }

    private void PathObstruction(float largestWeight, int largestWeightIndex)
    {
        //If path obstructed priortize traveling in one direction until unobstructed
        if (pathObstructed)
        {
            float newLargestWeight = -1;
            int newLargestWeightIndex = 0;
            for (int i = 0; i < numOfWeights; i++)
            {
                float w = Vector2.Dot(weights[i].dir, unobstructedDir);
                weights[i].weight += w * 0.4f;
                if (weights[i].weight > newLargestWeight)
                {
                    newLargestWeight = weights[i].weight;

                    newLargestWeightIndex = i;
                    newDir = weights[i].dir;
                }
            }

            if (newLargestWeight > maxObstructionChangeDir || weights[unobstructedIndex].weight < minObstructionChangeDir)
            {
                unobstructedDir = newDir;
                unobstructedIndex = newLargestWeightIndex;
            }

            newDir = unobstructedDir;

            if (weights[unobstructedIndex].weight >= obstructionDetectionThreshold)
            {
                newDir = weights[largestWeightIndex].dir;
                pathObstructed = false;
            }
        }

        if (!pathObstructed && largestWeight < obstructionDetectionThreshold)
        {
            pathObstructed = true;
            unobstructedDir = newDir;
            unobstructedIndex = largestWeightIndex;
        }
    }

    private void ObstacleDetection(bool isOrbiting)
    {
        //Check for obstacles in the path of weights
        for (int i = 0; i < numOfWeights; i++)
        {
            RaycastHit2D[] hitInfos = Physics2D.RaycastAll((Vector2)transform.position + weights[i].dir * objectRadius, weights[i].dir, obstacleDistanceSensitivity, obstacleMask);
            if (hitInfos.Length > 0)
            {
                RaycastHit2D hitInfo = hitInfos[hitInfos.Length - 1];

                //Find closest obstacle that isnt self
                for (int j = 0; j < hitInfos.Length; j++)
                {
                    if (hitInfos[j].collider != null && hitInfos[j].collider.gameObject.name != name && hitInfos[j].distance < hitInfo.distance)
                        hitInfo = hitInfos[j];
                }

                if (hitInfo.collider != null && hitInfo.collider.gameObject.name != name)
                {
                    float negativeWeight = 1 - Mathf.Clamp(hitInfo.distance / obstacleDistanceSensitivity, 0f, 1f);

                    if (isOrbiting && hitInfo.collider.gameObject.CompareTag(comradeTag))
                        negativeWeight = 1 - Mathf.Abs(negativeWeight - 0.65f);

                    weights[i].weight -= negativeWeight;

                    //Propogate the negative weight across nearby weights at reduced amount
                    weights[(i + 1) % numOfWeights].weight -= negativeWeight / 2;
                    weights[(i + 2) % numOfWeights].weight -= negativeWeight / 3;
                    weights[(i - 1) >= 0 ? i - 1 : i - 1 + numOfWeights].weight -= negativeWeight / 2;
                    weights[(i - 2) >= 0 ? i - 2 : i - 2 + numOfWeights].weight -= negativeWeight / 3;
                }
                else
                {
                    //Reward no obstacles in path
                    weights[i].weight += weights[i].weight * noObstacleDesire;
                }
            }

            weights[i].weight = Mathf.Clamp(weights[i].weight, -1f, 1f);
        }
    }

    Coroutine pauseMovement;
    public void PauseMovement(float length)
    {
        if (pauseMovement != null)
            StopCoroutine(pauseMovement);

        pauseMovement = StartCoroutine(PauseMovementSequence(length));
    }
    IEnumerator PauseMovementSequence(float length)
    {
        canMove = false;
        yield return new WaitForSeconds(length);
        canMove = true;
    }

    private void OnDrawGizmos()
    {
        if (!visualizeWeights) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, objectRadius);

        if (weights == null) return;

        for (int i = 0; i < numOfWeights; i++)
        {
            if(weights[i].weight >= 0)
            {
                Gizmos.color = Color.green;
            } else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine((Vector2)transform.position + weights[i].dir * objectRadius, (Vector2)transform.position + weights[i].dir * objectRadius + weights[i].dir * Mathf.Abs(weights[i].weight));
        }
    }
}