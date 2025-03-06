using System;
using System.Collections.Generic;
using _Scripts.Entities.EntitySpecific;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Pickups;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace _Scripts.Managers.SpawnManager
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Tilemap References")]
        [SerializeField] private Tilemap platformTilemap;
        [SerializeField] private Tilemap hazardTilemap;
        [SerializeField] private Tilemap decorationTilemap;
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private Tilemap backgroundTilemap;

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
        private readonly Queue<int> _activeChunks = new Queue<int>();
        private readonly Dictionary<int, Vector3Int> _chunkPositions = new Dictionary<int, Vector3Int>();

        private void Start()
        {
            for (var i = 0; i <= 3; i++)
            {
                GenerateChunk(i);
            }
        }

        private void Update()
        {
            var playerChunk = Mathf.FloorToInt(playerPosition.position.y / chunkHeight);

            if (playerChunk > _lastGeneratedChunk - 3)
            {
                GenerateChunk(playerChunk + 3);
            }

            while (_activeChunks.Count > 7) //Maintain 7 active chunks
            {
                var oldestChunk = _activeChunks.Peek();
                if (oldestChunk < playerChunk - 3)
                {
                    _activeChunks.Dequeue();
                    ClearChunks(oldestChunk);
                }
                else
                {
                    break;
                }
            }
        }

        private void GenerateChunk(int chunkIndex)
        {
            if (_activeChunks.Contains(chunkIndex)) return;
            
            var chunkPosition = new Vector3Int(0, (chunkIndex * chunkHeight) - 1, 0);
            _chunkPositions[chunkIndex] = chunkPosition;
            
            GenerateWalls(chunkIndex, chunkPosition);
            GeneratePlatforms(chunkIndex, chunkPosition);
            GenerateHazards(chunkIndex, chunkPosition);
            GenerateDecorations(chunkIndex, chunkPosition);
            GenerateObjects(chunkIndex, chunkPosition);
            GenerateBackground(chunkIndex, chunkPosition);

            _activeChunks.Enqueue(chunkIndex);
            _lastGeneratedChunk = chunkIndex;

            var playerChunk = Mathf.FloorToInt(playerPosition.position.y / chunkHeight);

            while (_activeChunks.Count > 6)
            {
                var oldestChunk = _activeChunks.Peek();
                if (oldestChunk < playerChunk - 3)
                {
                    _activeChunks.Dequeue();
                    ClearChunks(oldestChunk);
                }
                else
                {
                    break;
                }
            }
        }

        #region Generating tiles and Objects
        
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
        private void GeneratePlatforms(int chunkIndex, Vector3Int chunkPosition)
        {
            const int platformSpacing = 5; //Platforms are spaced every 5 Y units
            var numPlatforms = chunkHeight / platformSpacing; //Ensure spacing aligns with full-width platforms
            
            //Generate a full-width platform at chunk 0 and every second chunk
            if (chunkIndex % 2 == 0)
            {
                var startX = wallSettings.leftWallX + 4; //Avoid placing inside walls
                var endX = wallSettings.rightWallX - 4;  //Avoid placing inside walls
                var platformY = chunkPosition.y; //Place at the bottom of the chunk

                for (var x = startX; x <= endX; x++)
                {
                    var selectedTile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                    platformTilemap.SetTile(new Vector3Int(x, platformY, 0), selectedTile);
                }
            }
            
            for (var i = 0; i < numPlatforms; i++)
            {
                var width = Random.Range(platformSettings.minWidth, platformSettings.maxWidth);
                var x = Random.Range(wallSettings.leftWallX + 4, wallSettings.rightWallX - width - 4);
                var y = chunkPosition.y + i * platformSpacing;
                
                //Adjust for chunks without full-width platforms
                if (chunkIndex % 2 != 0 && i == numPlatforms - 1)
                {
                    y = chunkPosition.y + chunkHeight - platformSpacing; //Ensure last platform aligns before new chunk
                }
                
                for (var j = 0; j < width; j++)
                {
                    var tile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                    platformTilemap.SetTile(new Vector3Int(x + j, y, 0), tile);
                }
            }
        }
        //Work in progress------------------------------------------------------------
        private void GenerateObjects(int chunkIndex, Vector3Int chunkPosition)
        {
            #region Coins Spawner

            var coinsToSpawn = objectSettings.GetRandomCoinsPerChunk();
            const int bottomOffset = 5;
            const int startOffset = 30;

            for (var i = 0; i < coinsToSpawn; i++)
            {
                var x = Random.Range(wallSettings.leftWallX + 5, wallSettings.rightWallX - 5);
                var y = chunkPosition.y + bottomOffset + (i * (chunkHeight - bottomOffset) / coinsToSpawn);
                if (chunkIndex == 0)
                {
                    y = chunkPosition.y + startOffset + (i * (chunkHeight - bottomOffset) / coinsToSpawn);
                }
                if (i == coinsToSpawn - 1)
                {
                    y = chunkPosition.y + chunkHeight - (bottomOffset * 2);
                }

                var coin = CoinPool.Instance.GetObject(new Vector3Int(x, y, 0));
                coin?.gameObject.SetActive(true);
            }

            #endregion

            #region Enemy Spawner On Platforms

            var enemiesToSpawn = objectSettings.GetRandomEnemiesPerChunk();
            var mustSpawnOnPlatform = objectSettings.enemiesMustSpawnOnPlatform;
            
            if(chunkIndex == 0) return;
            if (!mustSpawnOnPlatform) return;
            {
                //Get all valid spawn positions on platforms
                var availableSpawnPositions = GetPlatformSpawnPositions(chunkPosition, enemiesToSpawn);

                for (var i = 0; i < enemiesToSpawn; i++)
                {
                    if (availableSpawnPositions.Count == 0)
                    {
                        break;
                    }

                    //Select a random platform position and remove it to prevent stacking
                    var randomIndex = Random.Range(0, availableSpawnPositions.Count);
                    var spawnPosition = availableSpawnPositions[randomIndex];
                    availableSpawnPositions.RemoveAt(randomIndex);

                    var enemy = EnemyPool.Instance.GetObject(spawnPosition);

                    enemy?.gameObject.SetActive(true);
                }
            }

            #endregion
        }
        private void GenerateHazards(int chunkIndex, Vector3Int chunkPosition)
        {
            if (Random.value <= hazardSettings.spawnProbability)
            {
                var x = Random.Range(wallSettings.leftWallX + 2, wallSettings.rightWallX - 2);
                var y = Random.Range(chunkPosition.y, chunkPosition.y + chunkHeight - 1); //FIXED HEIGHT RANGE
                
                var tile = hazardSettings.ruleTileHazards[Random.Range(0, hazardSettings.ruleTileHazards.Length)];
                hazardTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        private void GenerateDecorations(int chunkIndex, Vector3Int chunkPosition)
        {
            var decorationCount = Random.Range(1, decorationSettings.maxDecorationsPerChunk);

            for (var i = 0; i < decorationCount; i++)
            {
                var x = Random.Range(wallSettings.leftWallX + 2, wallSettings.rightWallX - 2);
                var y = Random.Range(chunkPosition.y, chunkPosition.y + chunkHeight - 1); //FIXED HEIGHT RANGE
                
                var spawnPosition = new Vector3Int(x, y, 0);

                if (decorationSettings.platformRuleTileDecorations.Length > 0)
                {
                    var index = Random.Range(0, decorationSettings.platformRuleTileDecorations.Length);
                    var selectedTile = decorationSettings.platformRuleTileDecorations[index];

                    var platformCheckPos = new Vector3Int(x, y - 1, 0);
                    if (platformTilemap.HasTile(platformCheckPos))
                    {
                        decorationTilemap.SetTile(spawnPosition, selectedTile);
                    }
                }
            }
        }
        //-----------------------------------------------------------------------------
        private void GenerateBackground(int chunkIndex, Vector3Int chunkPosition)
        {
            for (var x = wallSettings.leftWallX; x <= wallSettings.rightWallX; x++) 
            {
                for (var y = chunkPosition.y; y < chunkPosition.y + chunkHeight; y++) //FIXED HEIGHT RANGE
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
                    hazardTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    decorationTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
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
                    if (coin != null)
                    {
                        CoinPool.Instance.ReturnObject(coin);
                    }
                }
                else if (obj.CompareTag("Enemy"))
                {
                    var enemy = obj.GetComponent<Entity>(); // Adjust if you have multiple enemy types
                    if (enemy != null)
                    {
                        EnemyPool.Instance.ReturnObject(enemy);
                    }
                }
                else if (obj.CompareTag("PowerUp"))
                {
                    var powerUp = obj.GetComponent<PowerUp>();
                    if (powerUp != null)
                    {
                        PowerUpPool.Instance.ReturnObject(powerUp);
                    }
                }
            }
        }
        private List<Vector3Int> GetPlatformSpawnPositions(Vector3Int chunkPosition, int maxPositions)
        {
            var platformPositions = new List<Vector3Int>();
            
            for (var x = wallSettings.leftWallX + 5; x <= wallSettings.rightWallX - 5; x += 3) //Spread search horizontally
            {
                for (var y = chunkPosition.y + 1; y <= chunkPosition.y + chunkHeight - 1; y++) //Iterate over all Y positions
                {
                    var checkPosition = new Vector3Int(x, y - 1, 0); //Check if a platform exists BELOW

                    if (platformTilemap.HasTile(checkPosition)) //If platform exists at y - 1
                    {
                        //Find platform width & center
                        var left = x;
                        var right = x;

                        //Expand left until no tile is found
                        while (platformTilemap.HasTile(new Vector3Int(left - 1, y - 1, 0)))
                        {
                            left--;
                        }

                        //Expand right until no tile is found
                        while (platformTilemap.HasTile(new Vector3Int(right + 1, y - 1, 0)))
                        {
                            right++;
                        }

                        //Calculate center of the platform
                        var centerX = (left + right) / 2;

                        platformPositions.Add(new Vector3Int(centerX, y, 0)); //Enemy spawns exactly in the center
                    }
                }
            }

            return platformPositions; // Returns all valid spawn positions
        }
        
        private void OnDrawGizmos()
        {
            if (_chunkPositions == null) return;

            Gizmos.color = Color.green; //Color for active chunks

            foreach (var chunk in _chunkPositions)
            {
                var chunkBottomLeft = new Vector3(-10, chunk.Value.y, 0); // Left boundary
                var chunkBottomRight = new Vector3(10, chunk.Value.y, 0); // Right boundary
                var chunkTopLeft = new Vector3(-10, chunk.Value.y + chunkHeight, 0); // Upper left
                var chunkTopRight = new Vector3(10, chunk.Value.y + chunkHeight, 0); // Upper right

                //Draw chunk boundaries in Scene View
                Gizmos.DrawLine(chunkBottomLeft, chunkBottomRight); // Bottom
                Gizmos.DrawLine(chunkBottomRight, chunkTopRight); // Right side
                Gizmos.DrawLine(chunkTopRight, chunkTopLeft); // Top
                Gizmos.DrawLine(chunkTopLeft, chunkBottomLeft); // Left side
            }
        }
    }
}
