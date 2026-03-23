using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifetime = 5f;      // auto-destroy after 5 seconds
    public int damage = 1;           // amount of damage

    [Header("Target Layer")]
    public LayerMask hitLayers;      // layers this bullet can hit

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 direction, float bulletSpeed)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only interact with layers in hitLayers
        if ((hitLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // 🔴 GET ENEMY (IMPORTANT: use parent!)
        ChaseAI enemy = collision.GetComponentInParent<ChaseAI>();

        if (enemy != null)
        {
            Vector2 hitPoint = collision.ClosestPoint(transform.position);
            enemy.TakeHit(hitPoint);
        }

        // Deal damage
        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChaseAI enemy = collision.collider.GetComponent<ChaseAI>();

        if (enemy != null)
        {
            Vector2 hitPoint = collision.contacts[0].point;
            enemy.TakeHit(hitPoint);
        }

        Destroy(gameObject);
    }
}
