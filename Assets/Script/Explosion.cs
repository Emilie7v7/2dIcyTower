using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionForce = 2000f;
    public float explosionRadius = 3f;
    public LayerMask playerLayer;

    private void Start()
    {
        // Find the player within the explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D collider in colliders)
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                // Calculate explosion direction (opposite of player position)
                Vector2 explosionDirection = (player.transform.position - transform.position).normalized;

                // Apply explosion force (reverse the direction to push away)
                player.ApplyExplosionForce(-explosionDirection, explosionForce);  // Negative to push away
            }
        }


        // Destroy the explosion after applying the effect
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the explosion radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}