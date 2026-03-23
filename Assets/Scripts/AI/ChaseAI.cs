using System;
using UnityEngine;

public class ChaseAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float stopDistance = 5f;

    [Header("Target")]
    public Transform target;

    [Header("Vision")]
    public LayerMask obstacleLayers;
    public LayerMask playerLayer;

    private AIShooting aiShooting;
    public Raycaster bloodManager;

    public void TakeHit(Vector2 hitPoint)
    {
        bloodManager.SpawnBlood(hitPoint, false);
    }
    private void Start()
    {
        aiShooting = GetComponent<AIShooting>();
    }

    void Update()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(target.position, transform.position);

        bool hasLOS = HasLineOfSight();

        // Movement
        if (distance > stopDistance || !hasLOS)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }

        // Tell shooting system what to do
        bool canShoot = distance <= stopDistance && hasLOS;

        if (aiShooting != null)
        {
            aiShooting.SetShootingEnabled(canShoot);
            aiShooting.target = target; // ✅ moved here
        }

        // Flip sprite
        transform.localScale = new Vector3(
            target.position.x < transform.position.x ? -1 : 1,
            1,
            1
        );
    }

    bool HasLineOfSight()
    {
        if (aiShooting == null || aiShooting.gunPivot == null)
            return false;

        Vector2 origin = aiShooting.gunPivot.position;
        Vector2 direction = (target.position - aiShooting.gunPivot.position).normalized;
        float distance = Vector2.Distance(aiShooting.gunPivot.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayers);

        return hit.collider == null;
    }
}
