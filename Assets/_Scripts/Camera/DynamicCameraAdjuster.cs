using Cinemachine;
using UnityEngine;

namespace _Scripts.Camera
{
    public class DynamicCameraAdjuster : MonoBehaviour
    {
        [Header("Dynamic Camera")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform playerTransform; // Assign player GameObject
        [SerializeField] private float pixelsPerUnit = 100f; // Set based on your game

        [Header("Camera Settings")]
        [SerializeField] private Vector3 trackedObjectOffset;

        private CinemachineFramingTransposer _framingTransposer;
        
        private void Start()
        {

            _framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            AdjustCameraForMobile();
            LockCameraX();
        }

        private void Update()
        {
            LockCameraX();
        }

        private void AdjustCameraForMobile()
        {
            if (UnityEngine.Camera.main != null && UnityEngine.Camera.main.orthographic)
            {
                float screenHeight = Screen.height;
                UnityEngine.Camera.main.orthographicSize = screenHeight / (2f * pixelsPerUnit);
            }

            // Adjust Framing Transposer for smooth vertical scrolling
            if (_framingTransposer != null)
            {
                _framingTransposer.m_TrackedObjectOffset = new Vector3(trackedObjectOffset.x, trackedObjectOffset.y, trackedObjectOffset.z); // Adjust vertical offset
                _framingTransposer.m_DeadZoneWidth = 1f;  // No horizontal movement
                _framingTransposer.m_DeadZoneHeight = 0.2f; // Allow some vertical movement
                _framingTransposer.m_SoftZoneHeight = 1f;  // Large soft zone for smooth follow
                _framingTransposer.m_YDamping = 0.4f;  // Smooth but responsive follow
            }
        }

        private void LockCameraX()
        {
            if (playerTransform is null) return;

            var camPos = virtualCamera.transform.position;
            camPos.x = 0; // Locks X position
            virtualCamera.transform.position = camPos;
        }
    }
}