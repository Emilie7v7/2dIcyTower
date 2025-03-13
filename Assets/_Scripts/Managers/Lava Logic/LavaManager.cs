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
        [SerializeField] private int startOffset = 10; // How far below the player lava starts
        [SerializeField] private int maxLavaTiles = 12; // Maximum rows of lava before clearing old ones
        [SerializeField] private float lavaStartDelay = 5f; // How long before lava starts rising
        [SerializeField] private float minLavaSpeed = 10f; // Slowest speed (when close to player)
        [SerializeField] private float maxLavaSpeed = 10f; // Fastest speed (when far from player)
        [SerializeField] private int wallLeft = -16;
        [SerializeField] private int wallRight = 16;

        private int _lavaHeight; //Current lava Y position
        private bool _lavaActive = false;
        private float _timeSinceStart = 0f;
        private float _nextLavaRiseTime = 0f;

        private void Start()
        {
            InitializeLava();
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
            _lavaHeight = Mathf.FloorToInt(player.position.y) - startOffset; // Start below player

            for (var y = _lavaHeight; y < _lavaHeight + startOffset; y++)
            {
                for (var x = wallLeft; x <= wallRight; x++) // Adjust for walls
                {
                    hazardTilemap.SetTile(new Vector3Int(x, y, 0), lavaTile);
                }
            }

            _lavaHeight += startOffset - 1; //FIX: Ensure lavaHeight starts at the topmost lava row
        }

        private void UpdateLava()
        {
            var playerY = Mathf.FloorToInt(player.position.y);

            //Adjust Lava Speed: Faster if far, slower if close
            float distanceFromPlayer = playerY - _lavaHeight;
            const float maxLavaCatchupDistance = 15f;
            var lavaSpeed = Mathf.Lerp(minLavaSpeed, maxLavaSpeed, Mathf.Clamp01(distanceFromPlayer / maxLavaCatchupDistance));

            if (Time.time >= _nextLavaRiseTime)
            {
                _nextLavaRiseTime = Time.time + (1f / lavaSpeed);

                _lavaHeight++; //Move lava up by 1 tile

                for (int x = wallLeft; x <= wallRight; x++) //Adjust for walls
                {
                    hazardTilemap.SetTile(new Vector3Int(x, _lavaHeight, 0), lavaTile);
                }

                //FIX: Ensure old lava is removed properly
                ClearOldLava();
            }
        }

        private void ClearOldLava()
        {
            var clearY = _lavaHeight - maxLavaTiles;

            //FIX: Ensure it clears the initial lava rows as well
            if (clearY < _lavaHeight - startOffset) 
            {
                clearY = _lavaHeight - startOffset; //Start clearing from the first placed lava row
            }

            for (var x = wallLeft; x <= wallRight; x++)
            {
                hazardTilemap.SetTile(new Vector3Int(x, clearY, 0), null);
            }
        }
    }
}