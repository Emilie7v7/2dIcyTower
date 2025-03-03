using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Camera
{
    public class CameraBoundsAdjuster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PolygonCollider2D cameraBoundsCollider;
        [SerializeField] private CinemachineConfiner2D cinemachineConfiner;
        [SerializeField] private Transform playerTransform;
    
        [Header("Boundary Settings")]
        [SerializeField] private float leftBoundOffset = -6f;  // Adjust to match left wall
        [SerializeField] private float rightBoundOffset = 6f;  // Adjust to match right wall
        [SerializeField] private float bottomBuffer = -2f; // Extra space below player
        [SerializeField] private float topBuffer = 8f; // Extra space above player
        [SerializeField] private int chunkHeight = 50; // Extra space above player

        private readonly List<Vector2> _points = new List<Vector2>();

        private void Start()
        {
            UpdateCameraBounds(playerTransform.position.y);
        }

        private void Update()
        {
            UpdateCameraBounds(playerTransform.position.y);
        }

        private void UpdateCameraBounds(float playerY)
        {
            var bottomY = playerY - chunkHeight;  // Keep 3 chunks below
            var topY = playerY + chunkHeight;     // Keep 3 chunks above

            _points.Clear();
            _points.Add(new Vector2(leftBoundOffset, bottomY + bottomBuffer)); // Bottom-left
            _points.Add(new Vector2(rightBoundOffset, bottomY + bottomBuffer)); // Bottom-right
            _points.Add(new Vector2(rightBoundOffset, topY + topBuffer)); // Top-right
            _points.Add(new Vector2(leftBoundOffset, topY + topBuffer)); // Top-left

            cameraBoundsCollider.SetPath(0, _points);

            // Refresh Confiner
            cinemachineConfiner.InvalidateCache();
        }
    }
}
