using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.PlayerData
{
    [CreateAssetMenu(menuName = "Data/Player Data/Base Data", fileName = ("newPlayerData"))]
    public class PlayerDataSo : ScriptableObject
    {
        [Header("Player Movement Properties")] 
        public float playerMovementSpeed;
        public float decelerationRate;
        
        [Header("ProjectilePrefab")]
        public GameObject projectilePrefab;

        [Header("Speed Rotation Of Indicator")]
        public float rotationSpeed;
        
        [Header("Gravity & Drag Settings")]
        public float defaultGravityScale = 1f;  // Normal gravity when grounded
        public float gravityScaleUp = 0.5f;  // Gravity scale when moving up
        public float gravityScaleDown = 3f;  // Gravity scale when falling
        public float dragUp = 0.8f;          // Drag when moving up
        public float dragDown = 0f;          // Drag when falling
        public float gravityIncreaseFactor = 0.25f; // Adjust this for slower or faster increase
        
        [Header("Velocity & Gravity Adjustments")]
        public float upwardVelocityThreshold = 2f;  // When velocity slows to this, snap into fall mode
        public float velocitySnapToFall = -3f;      // Instantly set velocity to this when switching to fall
        
        [Header("Upward Movement Control")]
        public float maxUpwardDistance = 5f;   // Short but fast vertical boost

        [Header("Smooth Fall Transition")]
        public float smoothFallTime = 0.2f;    // **Reduce transition time for quicker fall**
    }
        
}
