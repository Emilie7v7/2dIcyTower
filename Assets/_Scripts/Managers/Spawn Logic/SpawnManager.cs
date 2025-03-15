using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Pickups;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Managers.Spawn_Logic
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Tilemap References")]
        [SerializeField] private Tilemap hazardTilemap, decorationTilemap, wallTilemap, backgroundTilemap;
        
        [field: SerializeField] public Tilemap platformTilemap { get; set; }
        
        [Header("Spawn Settings")]
        [SerializeField] private PlatformSpawnSettingsSo platformSettings;
        [SerializeField] private HazardSpawnSettingsSo hazardSettings;
        [SerializeField] private DecorationSpawnSettingsSo decorationSettings;
        [SerializeField] private ObjectSpawnSettingsSo objectSettings;
        [SerializeField] private WallSpawnSettingsSo wallSettings;
        [SerializeField] private BackgroundSpawnSettingsSo backgroundSettings;

        [Header("Chunk Settings")]
        [SerializeField] private int chunkHeight = 50;
        
        [Header("Player Position")]
        [SerializeField] private Transform playerPosition;

        private int _lastGeneratedChunk = -1;
        private readonly Queue<int> _activeChunks = new();
        private readonly Dictionary<int, Vector3Int> _chunkPositions = new();

        private void Start()
        {
            for (var i = 0; i <= 3; i++) GenerateChunk(i);
        }

        private void Update()
        {
            var playerChunk = Mathf.FloorToInt(playerPosition.position.y / chunkHeight);

            if (playerChunk > _lastGeneratedChunk - 3)
                GenerateChunk(playerChunk + 3);

            CleanupOldChunks(playerChunk);
        }

        private void GenerateChunk(int chunkIndex)
        {
            if (_activeChunks.Contains(chunkIndex)) return;

            var chunkPosition = new Vector3Int(0, (chunkIndex * chunkHeight) - 1, 0);
            _chunkPositions[chunkIndex] = chunkPosition;
            
            GenerateWalls(chunkIndex, chunkPosition);
            GeneratePlatforms(chunkIndex, chunkPosition);
            GenerateDecorations(chunkIndex, chunkPosition);
            GenerateBackground(chunkIndex, chunkPosition);
            
            if (chunkIndex > 0)
                GenerateObjects(chunkIndex, chunkPosition);
            
            _activeChunks.Enqueue(chunkIndex);
            _lastGeneratedChunk = chunkIndex;
        }

        private void CleanupOldChunks(int playerChunk)
        {
            while (_activeChunks.Count > 7)
            {
                var oldestChunk = _activeChunks.Peek();
                if (oldestChunk < playerChunk - 3)
                {
                    _activeChunks.Dequeue();
                    ClearChunks(oldestChunk);
                }
                else break;
            }
        }

        #region Tile & Object Generation
        
        private void GeneratePlatforms(int chunkIndex, Vector3Int chunkPosition)
        {
            const int platformSpacing = 5;
            var numPlatforms = chunkHeight / platformSpacing;
            
            var startX = wallSettings.leftWallX + 4;
            var endX = wallSettings.rightWallX - 4;
            var platformY = chunkPosition.y;
            
            for (var x = startX; x <= endX; x++)
            {
                var selectedTile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                platformTilemap.SetTile(new Vector3Int(x, platformY, 0), selectedTile);
            }
            
            for (var i = 1; i < numPlatforms; i++)
            {
                var width = Random.Range(platformSettings.minWidth, platformSettings.maxWidth);
                var x = Random.Range(wallSettings.leftWallX + 4, wallSettings.rightWallX - width - 4);
                var y = chunkPosition.y + i * platformSpacing;
                
                for (var j = 0; j < width; j++)
                {
                    var tile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                    platformTilemap.SetTile(new Vector3Int(x + j, y, 0), tile);
                }
            }
        }
        
        private List<Vector3Int> GetPlatformSpawnPositions(Vector3Int chunkPosition, int maxPositions)
        {
            var platformPositions = new List<Vector3Int>();
            
            for (var x = wallSettings.leftWallX + 5; x <= wallSettings.rightWallX - 5; x += 3)
            {
                for (var y = chunkPosition.y + 1; y <= chunkPosition.y + chunkHeight - 1; y++)
                {
                    var checkPosition = new Vector3Int(x, y - 1, 0);
                    
                    if (platformTilemap.HasTile(checkPosition))
                    {
                        platformPositions.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
            return platformPositions;
        }
        
        private void GenerateDecorations(int chunkIndex, Vector3Int chunkPosition) 
        {
            var decorationsToSpawn = Random.Range(decorationSettings.minDecorationsPerChunk, decorationSettings.maxDecorationsPerChunk);
            var validPlatformPositions = GetPlatformSpawnPositions(chunkPosition, chunkHeight);

            for (var i = 0; i < decorationsToSpawn; i++)
            {
                var placeOnPlatform = Random.value > 0.5f;
                
                if (placeOnPlatform && validPlatformPositions.Count > 0)
                {
                    var platformPos = validPlatformPositions[Random.Range(0, validPlatformPositions.Count)];
                    var decorationPos = new Vector3Int(platformPos.x, platformPos.y, 0);
                    
                    var selectedTile = decorationSettings.platformSingleTileDecorations.Length > 0 ?
                        decorationSettings.platformSingleTileDecorations[Random.Range(0, decorationSettings.platformSingleTileDecorations.Length)] :
                        null;
                    
                    if (selectedTile is not null)
                        decorationTilemap.SetTile(decorationPos, selectedTile);
                }
            }
        }
        
        
        private void GenerateWalls(int chunkIndex, Vector3Int chunkPosition)
        {
            for (var y = chunkPosition.y; y < chunkPosition.y + chunkHeight; y++)
            {
                var selectedWallTile = wallSettings.wallRuleTiles[0];

                if (wallSettings.changeWallStyleAtHeight && wallSettings.wallRuleTiles.Length > 1)
                {
                    var styleIndex = (y / wallSettings.wallStyleChangeInterval) % wallSettings.wallRuleTiles.Length;
                    selectedWallTile = wallSettings.wallRuleTiles[styleIndex];
                }

                for (var i = 0; i < 4; i++)
                {
                    wallTilemap.SetTile(new Vector3Int(wallSettings.leftWallX + i, y, 0), selectedWallTile);
                    wallTilemap.SetTile(new Vector3Int(wallSettings.rightWallX - i, y, 0), selectedWallTile);
                }
            }
        }
        
        private void GenerateObjects(int chunkIndex, Vector3Int chunkPosition)
        {
            SpawnCoins(chunkIndex, chunkPosition);
            SpawnEnemies(chunkIndex, chunkPosition);
        }
        
        private void SpawnCoins(int chunkIndex, Vector3Int chunkPosition)
        {
            var coinsToSpawn = objectSettings.GetRandomCoinsPerChunk();
            var coinObjects = new List<CoinPickup>();
            for (var i = 0; i < coinsToSpawn; i++)
            {
                var x = Random.Range(wallSettings.leftWallX + 5, wallSettings.rightWallX - 5);
                var y = chunkPosition.y + 5 + (i * (chunkHeight - 5) / coinsToSpawn);
                var coin = CoinPool.Instance.GetObject(new Vector3Int(x, y, 0));
                if (coin is not null)
                {
                    coin.gameObject.SetActive(true);
                    coinObjects.Add(coin);
                }
            }
        }
        
        private void SpawnEnemies(int chunkIndex, Vector3Int chunkPosition)
        {
            if (!objectSettings.enemiesMustSpawnOnPlatform) return;
            
            var enemiesToSpawn = objectSettings.GetRandomEnemiesPerChunk();
            var availableSpawnPositions = GetPlatformSpawnPositions(chunkPosition, enemiesToSpawn);
            
            for (var i = 0; i < enemiesToSpawn && availableSpawnPositions.Count > 0; i++)
            {
                var spawnPosition = availableSpawnPositions[Random.Range(0, availableSpawnPositions.Count)];
                var enemy = EnemyPool.Instance.GetObject(spawnPosition);
                enemy?.gameObject.SetActive(true);
            }
        }
        
        private void GenerateBackground(int chunkIndex, Vector3Int chunkPosition)
        {
            for (var x = wallSettings.leftWallX; x <= wallSettings.rightWallX; x++)
            {
                for (var y = chunkPosition.y; y < chunkPosition.y + chunkHeight; y++)
                {
                    var selectedTile = backgroundSettings.backgroundTiles[0];

                    if (backgroundSettings.changeBackgroundAtHeight)
                    {
                        var styleIndex = (y / backgroundSettings.backgroundChangeInterval) % backgroundSettings.backgroundTiles.Length;
                        selectedTile = backgroundSettings.backgroundTiles[styleIndex];
                    }

                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                }
            }
        }
        
        #endregion
        
        private void ClearChunks(int chunkIndex)
        {
            if (!_chunkPositions.TryGetValue(chunkIndex, out var chunkPosition)) return;

            for (var x = wallSettings.leftWallX; x <= wallSettings.rightWallX; x++)
            {
                for (var y = chunkPosition.y; y < chunkPosition.y + chunkHeight; y++)
                {
                    platformTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    decorationTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    hazardTilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }

            ReturnObjectsInChunk(chunkPosition);
            _chunkPositions.Remove(chunkIndex);
        }
        private void ReturnObjectsInChunk(Vector3Int chunkPosition)
        {
            var objectsInChunk = Physics2D.OverlapBoxAll(
                new Vector2(0, chunkPosition.y + (chunkHeight / 2f)), //Check center of chunk
                new Vector2(wallSettings.rightWallX * 2, chunkHeight), //Cover full chunk size
                0f);

            foreach (var obj in objectsInChunk)
            {
                if (obj.CompareTag("Coin"))
                {
                    var coin = obj.GetComponent<CoinPickup>();
                    if (coin is not null)
                    {
                        CoinPool.Instance.ReturnObject(coin);
                    }
                }
                else if (obj.CompareTag("Enemy"))
                {
                    var enemy = obj.GetComponent<Entity>();
                    if (enemy is not null)
                    {
                        EnemyPool.Instance.ReturnObject(enemy);
                    }
                }
                else if (obj.CompareTag("PowerUp"))
                {
                    var powerUp = obj.GetComponent<PowerUp>();
                    if (powerUp is not null)
                    {
                        PowerUpPool.Instance.ReturnObject(powerUp);
                    }
                }
            }
        }
    }
}