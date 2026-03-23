using Unity.Cinemachine;
using UnityEngine;

public class LatchPlayer : MonoBehaviour
{
     [Header("Possession Settings")]
    public float latchRange = 1.5f;
    public LayerMask enemyLayer;
    public Vector2 exitOffset = new Vector2(1f, 0f);

    [Header("Player Visuals")]
    public SpriteRenderer[] spritesToHide;
    

    [Header("Possession Cooldown")]
    public float possessCooldown = 0.2f;

    [Header("Invisible Layer")]
    public string invisibleLayerName = "InvisiblePlayer";
    private int originalLayer;

    private GameObject possessedEnemy = null;
    private float lastPossessTime = -10f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private void Start()
    {
        originalLayer = gameObject.layer;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time > lastPossessTime + possessCooldown)
        {
            if (possessedEnemy == null)
                TryPossess();
            else
                ReleasePossession();

            lastPossessTime = Time.time;
        }
    }

    private void TryPossess()
    {
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, latchRange, enemyLayer);
    if (enemyCollider == null) return;

    GameObject enemy = enemyCollider.gameObject;

    // Disable enemy AI
    ChaseAI ai = enemy.GetComponent<ChaseAI>();
    if (ai != null) ai.enabled = false;

    // Disable AI shooting and stop any firing
    AIShooting aiShoot = enemy.GetComponent<AIShooting>();
    if (aiShoot != null)
    {
        aiShoot.enabled = false;
        aiShoot.SetShootingEnabled(false);
    }

    // Enable player controls on enemy
    PlayerMovement pm = enemy.GetComponent<PlayerMovement>();
    if (pm != null) pm.enabled = true;

    PlayerShooting ps = enemy.GetComponent<PlayerShooting>();
    if (ps != null)
    {
        ps.enabled = true;          // enable player control
        ps.SetShootingEnabled(true); // allow shooting
        ps.ResetCooldown();         // prevent disappearing bullet bug
    }

    possessedEnemy = enemy;

    // Move player inside enemy and parent it
    transform.position = enemy.transform.position;
    transform.SetParent(enemy.transform);

    // Hide player visuals
    foreach (var sr in spritesToHide)
        sr.enabled = false;

    // Disable physics
    if (playerCollider != null)
        playerCollider.enabled = false;

    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    // Change layer
    int layer = LayerMask.NameToLayer(invisibleLayerName);
    if (layer != -1)
        gameObject.layer = layer;
    
    // Subscribe to enemy death
    Health enemyHealth = possessedEnemy.GetComponent<Health>();
    if (enemyHealth != null)
        enemyHealth.onDeath.AddListener(OnPossessedEnemyDeath);

    // Redirect AI to possessed enemy
    ChaseAI[] allEnemies = FindObjectsOfType<ChaseAI>();
    foreach (var otherAI in allEnemies)
    {
        if (otherAI.target == transform)
            otherAI.target = possessedEnemy.transform;
    }
    }

    private void ReleasePossession()
    {
        if (possessedEnemy == null) return;

        // Unsubscribe death event
        Health enemyHealth = possessedEnemy.GetComponent<Health>();
        if (enemyHealth != null)
            enemyHealth.onDeath.RemoveListener(OnPossessedEnemyDeath);

        // Restore enemy AI
        ChaseAI ai = possessedEnemy.GetComponent<ChaseAI>();
        if (ai != null) ai.enabled = true;

        // Restore AI shooting
        AIShooting aiShoot = possessedEnemy.GetComponent<AIShooting>();
        if (aiShoot != null)
        {
            aiShoot.enabled = true;
            aiShoot.SetShootingEnabled(false); // reset so AI will decide next shot cleanly
        }

        // Disable player control on enemy
        PlayerMovement pm = possessedEnemy.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        PlayerShooting ps = possessedEnemy.GetComponent<PlayerShooting>();
        if (ps != null) ps.enabled = false;

        // Unparent player
        transform.SetParent(null);

        // Move player next to enemy
        Vector3 exitPos = possessedEnemy.transform.position + (Vector3)exitOffset;
        exitPos.z = 0f;
        transform.position = exitPos;

        // Restore visuals
        foreach (var sr in spritesToHide)
            sr.enabled = true;

        // Restore physics
        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // Restore layer
        gameObject.layer = originalLayer;
        
        // Redirect AI back to player
        ChaseAI[] allEnemies = FindObjectsOfType<ChaseAI>();
        foreach (var otherAI in allEnemies)
        {
            if (otherAI.target == possessedEnemy.transform)
                otherAI.target = transform;
        }

        possessedEnemy = null;
    }

    private void OnPossessedEnemyDeath()
    {
        if (possessedEnemy == null) return;

        // Unparent player
        transform.SetParent(null);
        transform.position = possessedEnemy.transform.position;

        // Restore visuals
        foreach (var sr in spritesToHide)
            sr.enabled = true;

        // Restore physics
        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        gameObject.layer = originalLayer;

       
        // Redirect AI back to player
        ChaseAI[] allEnemies = FindObjectsOfType<ChaseAI>();
        foreach (var otherAI in allEnemies)
        {
            if (otherAI.target == possessedEnemy.transform)
                otherAI.target = transform;
        }

        possessedEnemy = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, latchRange);
    }
}


