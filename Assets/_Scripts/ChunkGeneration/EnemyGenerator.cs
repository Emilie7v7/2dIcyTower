using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Enemy Generator")]
    public class EnemyGenerator : ScriptableObject, IChunkGenerator
    {
        [SerializeField] private int minEnemiesPerChunk;
        [SerializeField] private int maxEnemiesPerChunk;
        [SerializeField] private float distanceBetweenEnemies;
        [SerializeField] private GameObject enemyPrefab;
        
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _wallTilemap;
        private Tilemap _platformsTilemap;
        private Tilemap _solidPlatformsTilemap;
        private readonly List<Vector2> _placedEnemiesPositions = new List<Vector2>();
        private GameObject _chunkInstance;
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
        }

        public void Generate()
        {
            GenerateEnemies();
        }

        public void GetChunkInstance(GameObject chunkInstance)
        {
            _chunkInstance = chunkInstance;
        }
        
        public void GetTilemapsByType(Tilemap wallTilemap, Tilemap solidPlatformsTilemap, Tilemap platformsTilemap)
        {
            _wallTilemap = wallTilemap;
            _solidPlatformsTilemap = solidPlatformsTilemap;
            _platformsTilemap = platformsTilemap;
        }
        
        private void GenerateEnemies()
        {
            // Setup parent for enemies and remove the old ones for regeneration of the chunk
            var spawnPoint = _chunkInstance;
            var spawnPointParent = spawnPoint.transform.Find("SpawnPoints");
            var enemiesParent = spawnPointParent.transform.Find("Enemies");

            if (spawnPoint && spawnPointParent)
            {
                if (enemiesParent)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in enemiesParent.transform)
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
                    if (CanPlaceEnemies(x, y))
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
            
            // Determine how many enemies to spawn
            var enemiesToPlace = Random.Range(minEnemiesPerChunk, maxEnemiesPerChunk + 1);
            enemiesToPlace = Mathf.Min(enemiesToPlace, availableHeights.Count); // Don't try to place more enemies than available heights
            
            // Shuffle the heights
            for (var i = availableHeights.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (availableHeights[i], availableHeights[randomIndex]) = (availableHeights[randomIndex], availableHeights[i]);
            }
            
            // Place enemies at different heights
            for (var i = 0; i < enemiesToPlace; i++)
            {
                if (i >= availableHeights.Count) break;
                
                var heightY = availableHeights[i];
                var possiblePositions = positionsByHeight[heightY];
                
                if (possiblePositions.Count > 0)
                {
                    // Pick a random X position at this height
                    var randomIndex = Random.Range(0, possiblePositions.Count);
                    var selectedPosition = possiblePositions[randomIndex];
                    PlaceEnemies(selectedPosition.x, selectedPosition.y, enemiesParent);
                }
            }
        }

        
        
        private bool CanPlaceEnemies(int x, int y)
        {
            // Check if a position is clear of tiles
            var positionCheck = new Vector3Int(x, y, 0);
            if (x < 5 || x > _chunkWidth - 5 || y < 10 || y > _chunkHeight - 10) return false;
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
            foreach (var existingPos in _placedEnemiesPositions)
            {
                if (Vector2.Distance(newPos, existingPos) < distanceBetweenEnemies) return false;
            }
            return true;
        }
        
        private void PlaceEnemies(int x, int y, Transform parenTransform)
        {
            Instantiate(enemyPrefab, new Vector3(x - 20, y - 3.5f, 0), Quaternion.identity, parenTransform);
            _placedEnemiesPositions.Add(new Vector2(x, y));
        }
        
    }
}