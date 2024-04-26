using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CustomRigidbody2D : MonoBehaviour
{
    const float EPSILON = 0.01f;
    public static float gravity = -1f;
    public static float scalingFactor = 0.4f;

    Rigidbody2D rb;
    public float height { get; private set; }
    public float upVelocity { get; private set; }
    [SerializeField] float startHeight = 0.5f;
    [SerializeField] float drag;
    [SerializeField] float bounciness = 1;
    [SerializeField] int maxNumberOfBounces = 3;
    [SerializeField] bool onStart;

    int numberOfBounces = 0;

    bool isGrounded;

    public bool freeze;

    Vector3 defaultScale;

    public void Initialize(float height, float upVelocity,  bool freeze = false)
    {
        this.height = height;
        this.upVelocity = upVelocity;
        this.freeze = freeze;

        numberOfBounces = 0;

        rb = GetComponent<Rigidbody2D>();

        Vector2 newVel = rb.velocity;
        newVel.y += upVelocity;
        rb.velocity = newVel;

        defaultScale = transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (onStart)
        {
            rb = GetComponent<Rigidbody2D>();

            AddForce(new Vector3(-2f, -0.3f, 0.5f), ForceMode2D.Impulse);

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
            rb.AddForce(new Vector2(force.x, force.y), forceMode);
            upVelocity += force.z / rb.mass * Time.fixedDeltaTime;
        } else
        {
            rb.AddForce(new Vector2(force.x, force.y), forceMode);
            upVelocity += force.z / rb.mass;
        }
    }

    private void FixedUpdate()
    {
        if (!freeze)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            float dragForce = drag / rb.mass * Time.fixedDeltaTime * Time.fixedDeltaTime;
            if (height > 0 || upVelocity > 0)
            {
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
                    isGrounded = true;
                    upVelocity = 0;

                    transform.localScale = defaultScale;
                }
            }
        } else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
    }
}
