using System.Collections.Generic;
using UnityEngine;

public class SmallMagnetEffect : MonoBehaviour
{
    [SerializeField] private float radius = 5f; // Magnet radius, configurable in the Inspector

    [SerializeField] private float pullSpeed = 10f; // Speed at which objects are pulled toward the player

    [SerializeField] private Transform player; // Reference to the player's transform

    private void Update()
    {
        AttractNearbyPickups();
    }

    private void AttractNearbyPickups()
    {
        // Find all objects within the radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            // Get the object's sprite renderer to check its sorting layer
            SpriteRenderer renderer = hitCollider.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingLayerName == "Pickups")
            {
                // Move the object closer to the player
                Transform pickup = hitCollider.transform;

                Vector3 direction = (player.position - pickup.position).normalized;
                pickup.position += direction * pullSpeed * Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}