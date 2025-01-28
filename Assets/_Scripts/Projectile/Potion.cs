using _Scripts.CoreSystem;
using UnityEngine;

public class Potion : CoreComp
{
    public GameObject projectile;
    [SerializeField] private float spawnOffset; // Offset for spawning the projectile
    [SerializeField] private float explosionForce = 10f;

    [SerializeField] private CollisionSenses collisionSenses; // Reference to CollisionSenses

    private void Start()
    {
        if (collisionSenses == null)
        {
            collisionSenses = GetComponent<CollisionSenses>();
            if (collisionSenses == null)
            {
                Debug.LogError("CollisionSenses component is not attached or assigned to this GameObject.");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (projectile != null)
            {
                Instantiate(projectile, transform.position + new Vector3(spawnOffset, spawnOffset, 0f),
                    transform.rotation);
            }
            else
            {
                Debug.LogError("Projectile reference is missing. Please assign it in the Inspector.");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 explosionPosition = transform.position;

        // Check if touching Player using CollisionSenses property
        if (collisionSenses.Player)
        {
            Destroy(gameObject);
            Explosion(explosionPosition);
            Debug.Log("Explosion caused by a detected Player.");
        }

        // Check if touching Enemy using CollisionSenses property
        if (collisionSenses.Enemy)
        {
            Destroy(gameObject);
            Explosion(explosionPosition);
            Debug.Log("Explosion caused by a detected Enemy.");
        }
    }

    void ApplyExplosionForce(Vector2 explosionPosition)
    {
        // Find players within the explosion radius
        Collider2D[] playersInRadius = Physics2D.OverlapCircleAll(
            explosionPosition, 
            collisionSenses.PlayerCheckRadius, 
            collisionSenses.WhatIsPlayer
        );

        // Find enemies within the explosion radius
        Collider2D[] enemiesInRadius = Physics2D.OverlapCircleAll(
            explosionPosition, 
            collisionSenses.EnemyCheckRadius, 
            collisionSenses.WhatIsEnemy
        );

        // Apply explosion force to players
        foreach (Collider2D obj in playersInRadius)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (rb.position - explosionPosition).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                Debug.Log($"Applied explosion force to Player: {obj.name}");
            }
        }

        // Apply explosion force to enemies
        foreach (Collider2D obj in enemiesInRadius)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (rb.position - explosionPosition).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                Debug.Log($"Applied explosion force to Enemy: {obj.name}");
            }
        }
    }

    void Explosion(Vector2 explosionPosition)
    {
        // Apply explosion logic
        ApplyExplosionForce(explosionPosition);
    }
}