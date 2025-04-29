using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Coin Generator")]
    public class CoinGenerator : ScriptableObject, IChunkGenerator
    {
        [SerializeField] private int minCoinsPerChunk;
        [SerializeField] private int maxCoinsPerChunk;
        [SerializeField] private float minDistanceBetweenCoins;
        [SerializeField] private GameObject coinPrefab;
        
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _wallTilemap;
        private Tilemap _solidPlatformsTilemap;
        private Tilemap _platformsTilemap;
        private readonly List<Vector2> _placedCoinsPositions = new List<Vector2>();
        private GameObject _chunkInstance;
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            _placedCoinsPositions.Clear();
        }

        public void GetTilemapsByType(Tilemap wallTilemap, Tilemap solidPlatformsTilemap, Tilemap platformsTilemap)
        {
            _wallTilemap = wallTilemap;
            _solidPlatformsTilemap = solidPlatformsTilemap;
            _platformsTilemap = platformsTilemap;
        }

        public void GetChunkInstance(GameObject chunkInstance)
        {
            _chunkInstance = chunkInstance;
        }
        
        public void Generate()
        {
            GenerateCoins();
        }
        
        private void GenerateCoins()
        {
            // Setup parent for coins and remove the old ones for regeneration of the chunk
            var spawnPoint = _chunkInstance;
            var spawnPointParent = spawnPoint.transform.Find("SpawnPoints");
            var coinsParent = spawnPointParent.transform.Find("Coins");

            if (spawnPoint && spawnPointParent)
            {
                if (coinsParent)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in coinsParent.transform)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(DestroyImmediate);   
                }
            }
            
            // Create a dictionary to group positions by Y coordinate
            var positionsByHeight = new Dictionary<int, List<Vector2Int>>();
            
            // Find all valid positions and group them by Y coordinate
            for (var x = 1; x < _chunkWidth - 1; x++)
            {
                for (var y = 1; y < _chunkHeight - 1; y++)
                {
                    if (CanPlaceCoin(x, y))
                    {
                        if (!positionsByHeight.ContainsKey(y))
                        {
                            positionsByHeight[y] = new List<Vector2Int>();
                        }
                        positionsByHeight[y].Add(new Vector2Int(x, y));
                    }
                }
            }

            // Convert dictionary values to a list for easier random selection
            var availableHeights = new List<int>(positionsByHeight.Keys);
            
            // Determine how many coins to place
            var coinsToPlace = Random.Range(minCoinsPerChunk, maxCoinsPerChunk + 1);
            coinsToPlace = Mathf.Min(coinsToPlace, availableHeights.Count); // Don't try to place more coins than available heights
            
            // Shuffle the heights
            for (var i = availableHeights.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (availableHeights[i], availableHeights[randomIndex]) = (availableHeights[randomIndex], availableHeights[i]);
            }

            // Place coins at different heights
            for (var i = 0; i < coinsToPlace; i++)
            {
                if (i >= availableHeights.Count) break;
                
                var heightY = availableHeights[i];
                var possiblePositions = positionsByHeight[heightY];
                
                if (possiblePositions.Count > 0)
                {
                    // Pick a random X position at this height
                    var randomIndex = Random.Range(0, possiblePositions.Count);
                    var selectedPosition = possiblePositions[randomIndex];
                    PlaceCoin(selectedPosition.x, selectedPosition.y, coinsParent);
                }
            }
        }
        
        private bool CanPlaceCoin(int x, int y)
        {
            // Check if a position is clear of tiles
            var positionCheck = new Vector3Int(x, y, 0);
            if (x < 6 || x > _chunkWidth - 6 || y < 10 || y > _chunkHeight - 10) return false;
            if (_wallTilemap.HasTile(positionCheck)) return false;
            if (_solidPlatformsTilemap.HasTile(positionCheck)) return false;
            if (_platformsTilemap.HasTile(positionCheck)) return false;
            
            // Check if there's a platform or wall below
            var belowPosition = new Vector3Int(x, y - 1, 0);
            var hasSupportBelow = _solidPlatformsTilemap.HasTile(belowPosition) ||
                                  _platformsTilemap.HasTile(belowPosition);
            if (!hasSupportBelow) return false;
            
            // Check minimum distance from other coins
            var newPos = new Vector2(x, y);
            foreach (var existingPosition in _placedCoinsPositions)
            {
                if (Vector2.Distance(newPos, existingPosition) < minDistanceBetweenCoins) return false;
            }
            return true;
        }

        
        private void PlaceCoin(int x, int y, Transform parenTransform)
        {
            Instantiate(coinPrefab, new Vector3(x - 20, y - 4, 0), Quaternion.identity, parenTransform);
            _placedCoinsPositions.Add(new Vector2(x, y));
        }
    }
}