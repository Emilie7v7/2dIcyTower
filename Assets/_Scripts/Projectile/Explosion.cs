using _Scripts.PlayerComponent;
using UnityEngine;

namespace _Scripts.Projectile
{
    public class Explosion : MonoBehaviour
    {
        [Header("Explosion Settings")]
        public float explosionForce = 2000f;
        public float explosionRadius = 1.05f; // Adjusted to match your debug log
        public LayerMask playerLayer;

        private void Start()
        {
            // Log explosion details
            Debug.Log($"Explosion at {transform.position} with Radius {explosionRadius}. LayerMask Value: {playerLayer.value}");

            // Find the player within the explosion radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
            Debug.Log($"Total Colliders found within explosion radius: {colliders.Length}");

            // Check each collider found
            foreach (Collider2D collider in colliders)
            {
                Debug.Log($"Collider found: {collider.name}, Layer: {LayerMask.LayerToName(collider.gameObject.layer)}");

                var player = collider.GetComponent<PlayerComponent.Player>();
                if (player != null)
                {
                    Debug.Log($"Player found at {player.transform.position}");
                    // Calculate explosion direction (from explosion to player)
                    Vector2 explosionDirection = (player.transform.position - transform.position).normalized;
                    Debug.Log($"Explosion Direction: {explosionDirection}");

                    // Apply explosion force (push player away from explosion)
                    player.ApplyExplosionForce(explosionDirection, explosionForce);  
                    Debug.Log($"Applied explosion force to player at {player.transform.position}");
                }
                else
                {
                    Debug.LogWarning($"Collider found but no PlayerController component: {collider.name}");
                }
            }

            // If no colliders were found, log detailed information
            if (colliders.Length == 0)
            {
                Debug.LogWarning("No colliders found within explosion radius. Check:");
                Debug.LogWarning($"- Player Layer: {LayerMask.LayerToName(3)}"); // Assuming 3 is playerLayer
                Debug.LogWarning($"- Explosion Position: {transform.position}");
                Debug.LogWarning($"- Explosion Radius: {explosionRadius}");
                Debug.LogWarning($"- Player's Current Position: {GameObject.FindObjectOfType<PlayerComponent.Player>()?.transform.position}");
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
}