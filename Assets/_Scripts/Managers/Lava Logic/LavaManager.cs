using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Managers.Lava_Logic
{
    public class LavaManager : MonoBehaviour
    {
        [Header("Lava Settings")]
        [SerializeField] private Tilemap hazardTilemap;
        [SerializeField] private RuleTile lavaTile;
        [SerializeField] private Transform player;
        [SerializeField] private int startOffset = 10;
        [SerializeField] private int maxLavaTiles = 12;
        [SerializeField] private float lavaStartDelay = 5f;
        [SerializeField] private float minLavaSpeed = 10f;
        [SerializeField] private float maxLavaSpeed = 10f;
        [SerializeField] private int wallLeft = -16;
        [SerializeField] private int wallRight = 16;

        [Header("Light Settings")]
        [SerializeField] private UnityEngine.Rendering.Universal.Light2D lavaLight; // Reference to the Light2D component
        [SerializeField] private float lightIntensity = 1f; // Base intensity of the light
        [SerializeField] private Color lightColor = Color.red; // Color of the light

        private int _lavaHeight;
        private bool _lavaActive = false;
        private float _timeSinceStart = 0f;
        private float _nextLavaRiseTime = 0f;

        private void Start()
        {
            InitializeLava();
            InitializeLight();
        }

        private void InitializeLight()
        {
            if (lavaLight == null)
            {
                // Create a new GameObject for the light if none is assigned
                GameObject lightObject = new GameObject("LavaLight");
                lavaLight = lightObject.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
                lavaLight.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Point;
                lavaLight.intensity = lightIntensity;
                lavaLight.color = lightColor;
                lavaLight.pointLightOuterRadius = 5f; // Adjust this value to change light radius
                lavaLight.pointLightInnerRadius = 0f;
            }

            // Position the light at the current lava height
            UpdateLightPosition();
        }

        private void UpdateLightPosition()
        {
            if (lavaLight != null)
            {
                // Position light at the center of the lava horizontally and at current height
                Vector3 lightPos = new Vector3(0f, _lavaHeight + 0.5f, 0f);
                lavaLight.transform.position = lightPos;
            }
        }

        private void Update()
        {
            _timeSinceStart += Time.deltaTime;

            if (!_lavaActive && _timeSinceStart >= lavaStartDelay)
            {
                _lavaActive = true;
            }

            if (_lavaActive)
            {
                UpdateLava();
            }
        }

        private void InitializeLava()
        {
            _lavaHeight = Mathf.FloorToInt(player.position.y) - startOffset;

            for (var y = _lavaHeight; y < _lavaHeight + startOffset; y++)
            {
                for (var x = wallLeft; x <= wallRight; x++)
                {
                    hazardTilemap.SetTile(new Vector3Int(x, y, 0), lavaTile);
                }
            }

            _lavaHeight += startOffset - 1;
        }

        private void UpdateLava()
        {
            var playerY = Mathf.FloorToInt(player.position.y);

            float distanceFromPlayer = playerY - _lavaHeight;
            const float maxLavaCatchupDistance = 15f;
            var lavaSpeed = Mathf.Lerp(minLavaSpeed, maxLavaSpeed, Mathf.Clamp01(distanceFromPlayer / maxLavaCatchupDistance));

            if (Time.time >= _nextLavaRiseTime)
            {
                _nextLavaRiseTime = Time.time + (1f / lavaSpeed);

                _lavaHeight++;

                for (int x = wallLeft; x <= wallRight; x++)
                {
                    hazardTilemap.SetTile(new Vector3Int(x, _lavaHeight, 0), lavaTile);
                }

                ClearOldLava();
                UpdateLightPosition(); // Update light position when lava moves
            }
        }

        private void ClearOldLava()
        {
            var clearY = _lavaHeight - maxLavaTiles;

            if (clearY < _lavaHeight - startOffset)
            {
                clearY = _lavaHeight - startOffset;
            }

            for (var x = wallLeft; x <= wallRight; x++)
            {
                hazardTilemap.SetTile(new Vector3Int(x, clearY, 0), null);
            }
        }
    }
}