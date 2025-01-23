using UnityEngine;

public class Potion : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Reference to the explosion prefab
    private Rigidbody2D potionRb;

    private void Awake()
    {
        // Get the Rigidbody2D component of the potion
        potionRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Ensure the potion collider is not a trigger initially (for normal collision)
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true; // Set the potion collider as trigger to pass through the player
        }

        // Ignore collision between the potion and the player
        Collider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log what the potion is triggering with
        Debug.Log("Potion triggered with: " + other.name);

        // Ignore the player and trigger explosion if colliding with any other object
        if (other.CompareTag("Player"))
        {
            Debug.Log("Potion passed through the player.");
            return; // Ignore collision with the player
        }

        // Spawn the explosion object if not colliding with the player
        SpawnExplosion();

        // Destroy the potion
        Destroy(gameObject);
    }

    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Explosion prefab not assigned!");
        }
    }
}
