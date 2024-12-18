using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CustomRigidbody2D : MonoBehaviour
{
    const float EPSILON = 0.01f;
    public static float gravity = -2f;
    public static float scalingFactor = 0.4f;

    Rigidbody2D rb;
    public float height { get; private set; }
    public float upVelocity { get; private set; }
    [SerializeField] float startHeight = 0.5f;
    [SerializeField] float drag;
    [SerializeField] float bounciness = 1;
    [SerializeField] int maxNumberOfBounces = 3;
    [SerializeField] ContactFilter2D contactFilter;
    [SerializeField] bool onStart;
    [SerializeField] bool endAsTrigger = false;

    int numberOfBounces = 0;

    bool isGrounded;

    public bool freeze;

    Vector3 defaultScale;

    Collider2D[] colliders;

    bool skipFrame;

    public void Initialize(float height, float upVelocity,  bool freeze = false)
    {
        this.height = height;
        this.upVelocity = upVelocity;
        this.freeze = freeze;

        numberOfBounces = 0;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        colliders = transform.GetComponentsInChildren<Collider2D>();

        AddForce(new Vector3(0, 0, upVelocity), ForceMode2D.Impulse);

        defaultScale = transform.localScale;
    }

    public void Initialize(float upVelocity, bool freeze = false)
    {
        height = startHeight;
        this.upVelocity = upVelocity;
        this.freeze = freeze;

        numberOfBounces = 0;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        colliders = transform.GetComponentsInChildren<Collider2D>();

        AddForce(new Vector3(0, 0, upVelocity), ForceMode2D.Impulse);

        defaultScale = transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (onStart)
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            colliders = transform.GetComponentsInChildren<Collider2D>();

            //AddForce(new Vector3(-2f, -0.3f, 0.5f), ForceMode2D.Impulse);

            height = startHeight;

            Vector2 newVel = rb.velocity;
            newVel.y += upVelocity;
            rb.velocity = newVel;

            numberOfBounces = 0;

            defaultScale = transform.localScale;
        }
    }

    public void AddForce(Vector3 force, ForceMode2D forceMode)
    {
        if(forceMode == ForceMode2D.Force)
        {
            rb.AddForce(new Vector2(force.x * 2, force.y), forceMode);
            upVelocity += force.z / rb.mass * Time.fixedDeltaTime;
        } else
        {
            rb.AddForce(new Vector2(force.x * 2, force.y), forceMode);
            upVelocity += force.z / rb.mass;
        }

        skipFrame = true;
    }

    private void FixedUpdate()
    {
        if (!freeze)
        {
            if(skipFrame) { skipFrame = false; return; }

            //rb.constraints = RigidbodyConstraints2D.None;
            float dragForce = drag / rb.mass * Time.fixedDeltaTime * Time.fixedDeltaTime;
            if (height > 0 || upVelocity > 0)
            {
                if (colliders.Length > 0 && colliders[0].isTrigger == false)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].isTrigger = true;
                    }
                }

                isGrounded = false;

                Vector2 newVel = rb.velocity;
                newVel.y -= upVelocity;

                upVelocity += gravity * Time.fixedDeltaTime;

                newVel.y += upVelocity;

                upVelocity += -Mathf.Sign(upVelocity) * dragForce;
                newVel += new Vector2(-Mathf.Sign(newVel.x), -Mathf.Sign(newVel.y)) * dragForce;
                rb.velocity = newVel;

                height += upVelocity * Time.fixedDeltaTime;

                transform.localScale = defaultScale * (1 + height * scalingFactor);
            }
            else
            {
                Vector2 newVel = rb.velocity;

                height = 0;

                if (!isGrounded && Mathf.Abs(upVelocity) > EPSILON)
                {
                    newVel.y -= upVelocity;
                    rb.velocity = newVel;
                }

                if (bounciness > 0 && numberOfBounces < maxNumberOfBounces)
                {
                    numberOfBounces++;

                    upVelocity *= -bounciness;
                    newVel.y += upVelocity;
                  
                    upVelocity += -Mathf.Sign(upVelocity) * dragForce;

                    if (Mathf.Abs(upVelocity) > EPSILON)
                        height += upVelocity * Time.fixedDeltaTime;

                    transform.localScale = defaultScale * (1 + height * scalingFactor);
                }

                if (Mathf.Abs(newVel.x) > EPSILON || Mathf.Abs(newVel.y) > EPSILON)
                {
                    newVel += new Vector2(-Mathf.Sign(newVel.x), -Mathf.Sign(newVel.y)) * dragForce;
                    rb.velocity = newVel;
                }

                if (height <= 0)
                {
                    if (colliders.Length > 0 && colliders[0].isTrigger == true)
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            colliders[i].isTrigger = endAsTrigger;
                        }
                    }

                    isGrounded = true;
                    upVelocity = 0;

                    transform.localScale = defaultScale;
                }
            }
        } else
        {
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal;
            Vector2 newVel = rb.velocity;


            newVel.y -= upVelocity;
            newVel.x *= normal.x * contact.normalImpulse * bounciness;
            newVel.y *= normal.y * contact.normalImpulse * bounciness;
            newVel.y += upVelocity;

            rb.velocity = newVel;
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*List<Collider2D> overlappingColliders = new List<Collider2D>();
        int num = collision.OverlapCollider(contactFilter, overlappingColliders);
        Collider2D activeCollider = null;
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < colliders.Length; j++)
            {
                if (overlappingColliders[i] == colliders[j])
                {
                    activeCollider = colliders[j];
                    break;
                }
            }
        }
        if (activeCollider == null) return;*/
        Vector2 updatedPosition = (Vector2)transform.position - new Vector2(0, height);
        Vector2 closestPointOnOtherCollider = collision.ClosestPoint(updatedPosition);
        //Vector2 closestPointOnActiveCollider = activeCollider.ClosestPoint(closestPointOnOtherCollider);

        //print(closestPointOnActiveCollider);
        if(true)
        {
            Vector2 normal = (updatedPosition - closestPointOnOtherCollider).normalized;
            Vector2 newVel = rb.velocity;

            //print($"{name}: {normal}");

            newVel.y -= upVelocity;
            if (Mathf.Sign(normal.x) != Mathf.Sign(newVel.x))
                newVel.x *= Mathf.Abs(normal.x) * bounciness * -1;
            if (Mathf.Sign(normal.y) != Mathf.Sign(newVel.y))
                newVel.y *= Mathf.Abs(normal.y) * bounciness * -1;
            newVel.y += upVelocity;

            rb.velocity = newVel;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Wall")) return;

        /*List<Collider2D> overlappingColliders = new List<Collider2D>();
        int num = collision.OverlapCollider(contactFilter, overlappingColliders);
        Collider2D activeCollider = null;
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < colliders.Length; j++)
            {
                if (overlappingColliders[i] == colliders[j])
                {
                    activeCollider = colliders[j];
                    break;
                }
            }
        }
        if (activeCollider == null) return;*/
        Vector2 updatedPosition = (Vector2)transform.position - new Vector2(0, height);
        //collision.
        Vector2 closestPointOnOtherCollider = collision.ClosestPoint(updatedPosition);
        //Vector2 closestPointOnActiveCollider = activeCollider.ClosestPoint(closestPointOnOtherCollider);

        //print(closestPointOnActiveCollider);
        if (true)
        {
            Vector2 normal = (updatedPosition - closestPointOnOtherCollider).normalized;
            Vector2 newVel = rb.velocity;

            newVel.y -= upVelocity;
            if (Mathf.Sign(normal.x) != Mathf.Sign(newVel.x))
                newVel.x *= Mathf.Abs(normal.x) * bounciness * -1;
            if (Mathf.Sign(normal.y) != Mathf.Sign(newVel.y))
                newVel.y *= Mathf.Abs(normal.y) * bounciness * -1;
            newVel.y += upVelocity;

            rb.velocity = newVel;
        }
    }
}
