using System;
using System.Collections.Generic;
using _Scripts.ScriptableObjects.SpawnSettingsData;
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
        [SerializeField] private int activeChunkLimit = 6;
        
        [Header("Player Position")]
        [SerializeField] private Transform playerPosition;

        private int _lastGeneratedChunk = -1;
        private Queue<int> _activeChunks = new Queue<int>();
        private Dictionary<int, Vector3Int> _chunkPositions = new Dictionary<int, Vector3Int>();

        private void Start()
        {
            GenerateChunk(0);
        }

        private void Update()
        {
            int playerChunk = Mathf.FloorToInt(playerPosition.position.y / chunkHeight);

            // 🔹 Only generate a chunk when the player TRULY enters a new one
            if (playerChunk > _lastGeneratedChunk)
            {
                _lastGeneratedChunk = playerChunk; // ✅ Update last generated chunk
                GenerateChunk(playerChunk);
            }
        }

        private void GenerateChunk(int chunkIndex)
        {
            if (_activeChunks.Contains(chunkIndex)) return;
            
            Debug.Log($"🆕 Generating chunk {chunkIndex} at Y = {chunkIndex * chunkHeight}");
            Vector3Int chunkPosition = new Vector3Int(0, chunkIndex * chunkHeight, 0);
            _chunkPositions[chunkIndex] = chunkPosition;

            // 🔹 Ensure a full-width starting platform at 0m
            if (chunkIndex == 0)
            {
                GenerateFullWidthPlatform(chunkPosition);
            }
            if (_activeChunks.Contains(chunkIndex))
            {
                Debug.LogWarning($"⚠️ Skipping chunk {chunkIndex}, already exists!");
                return;
            }
            
            
            GenerateWalls(chunkIndex, chunkPosition);
            GeneratePlatforms(chunkIndex, chunkPosition);
            GenerateHazards(chunkIndex, chunkPosition);
            GenerateDecorations(chunkIndex, chunkPosition);
            GenerateObjects(chunkIndex, chunkPosition);
            GenerateBackground(chunkIndex, chunkPosition);

            _activeChunks.Enqueue(chunkIndex);
            _lastGeneratedChunk = chunkIndex;

            int playerChunk = Mathf.FloorToInt(playerPosition.position.y / chunkHeight);

            while (_activeChunks.Count > 6)
            {
                int oldestChunk = _activeChunks.Peek();
                if (oldestChunk < playerChunk - 3)
                {
                    Debug.Log($"❌ Clearing chunk {oldestChunk}");
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
        
        private void GenerateFullWidthPlatform(Vector3Int chunkPosition)
        {
            var startX = wallSettings.leftWallX + 1; // Avoid placing inside walls
            var endX = wallSettings.rightWallX - 1;  // Avoid placing inside walls

            var platformY = chunkPosition.y - 1; //Place slightly below the player

            for (var x = startX; x <= endX; x++)
            {
                var selectedTile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                platformTilemap.SetTile(new Vector3Int(x, platformY, 0), selectedTile);
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
        
                //Debug.Log($"🧱 Wall at Y={y}, Chunk {chunkIndex} (Chunk Y range: {chunkPosition.y} to {chunkPosition.y + chunkHeight - 1})");
            }
        }

        
        private void GeneratePlatforms(int chunkIndex, Vector3Int chunkPosition)
        {
            int platformCount = Random.Range(5, 8);

            for (int i = 0; i < platformCount; i++)
            {
                int width = Random.Range((int)platformSettings.minWidth + 2, (int)platformSettings.maxWidth + 3);
                int x = Random.Range(wallSettings.leftWallX + 2, wallSettings.rightWallX - width - 2);
                int y = Random.Range(chunkPosition.y, chunkPosition.y + chunkHeight - 1); // 🔹 FIXED HEIGHT RANGE

                //Debug.Log($"🟢 Platform at Y={y}, Chunk {chunkIndex} (Chunk Y range: {chunkPosition.y} to {chunkPosition.y + chunkHeight - 1})");

                for (int j = 0; j < width; j++)
                {
                    TileBase tile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                    platformTilemap.SetTile(new Vector3Int(x + j, y, 0), tile);
                }
            }
        }
        
        private void GenerateHazards(int chunkIndex, Vector3Int chunkPosition)
        {
            if (Random.value <= hazardSettings.spawnProbability)
            {
                int x = Random.Range(wallSettings.leftWallX + 2, wallSettings.rightWallX - 2);
                int y = Random.Range(chunkPosition.y, chunkPosition.y + chunkHeight - 1); // 🔹 FIXED HEIGHT RANGE

                //Debug.Log($"🔥 Hazard at Y={y}, Chunk {chunkIndex} (Chunk Y range: {chunkPosition.y} to {chunkPosition.y + chunkHeight - 1})");

                TileBase tile = hazardSettings.ruleTileHazards[Random.Range(0, hazardSettings.ruleTileHazards.Length)];
                hazardTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        
        private void GenerateDecorations(int chunkIndex, Vector3Int chunkPosition)
        {
            int decorationCount = Random.Range(1, decorationSettings.maxDecorationsPerChunk);

            for (int i = 0; i < decorationCount; i++)
            {
                int x = Random.Range(wallSettings.leftWallX + 2, wallSettings.rightWallX - 2);
                int y = Random.Range(chunkPosition.y, chunkPosition.y + chunkHeight - 1); // 🔹 FIXED HEIGHT RANGE

                //Debug.Log($"🌿 Decoration at Y={y}, Chunk {chunkIndex} (Chunk Y range: {chunkPosition.y} to {chunkPosition.y + chunkHeight - 1})");

                var spawnPosition = new Vector3Int(x, y, 0);

                if (decorationSettings.platformRuleTileDecorations.Length > 0)
                {
                    int index = Random.Range(0, decorationSettings.platformRuleTileDecorations.Length);
                    RuleTile selectedTile = decorationSettings.platformRuleTileDecorations[index];

                    Vector3Int platformCheckPos = new Vector3Int(x, y - 1, 0);
                    if (platformTilemap.HasTile(platformCheckPos))
                    {
                        decorationTilemap.SetTile(spawnPosition, selectedTile);
                    }
                }
            }
        }
        
        private void GenerateObjects(int chunkIndex, Vector3Int chunkPosition)
        {
            if (Random.value <= objectSettings.spawnProbability)
            {
                GameObject objPrefab = objectSettings.objectPrefabs[Random.Range(0, objectSettings.objectPrefabs.Length)];
                int x = Random.Range(wallSettings.leftWallX + 2, wallSettings.rightWallX - 2);
                int y = Random.Range(chunkPosition.y + 2, chunkPosition.y + chunkHeight - 2); // 🔹 FIXED HEIGHT RANGE
                
                Instantiate(objPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
        
        private void GenerateBackground(int chunkIndex, Vector3Int chunkPosition)
        {
            for (var x = -20; x <= 20; x++) 
            {
                for (var y = chunkPosition.y; y < chunkPosition.y + chunkHeight; y++) // 🔹 FIXED HEIGHT RANGE
                {
                    var selectedTile = backgroundSettings.backgroundTiles[0];

                    if (backgroundSettings.changeBackgroundAtHeight)
                    {
                        var styleIndex = (y / backgroundSettings.backgroundChangeInterval) % backgroundSettings.backgroundTiles.Length;
                        selectedTile = backgroundSettings.backgroundTiles[styleIndex];
                    }

                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                    //Debug.Log($"🌌 Background at Y={y}, Chunk {chunkIndex} (Chunk Y range: {chunkPosition.y} to {chunkPosition.y + chunkHeight - 1})");
                }
            }
        }
        
        #endregion
        
        private void ClearChunks(int chunkIndex)
        {
            if (!_chunkPositions.ContainsKey(chunkIndex)) return;

            Vector3Int chunkPosition = _chunkPositions[chunkIndex];

            Debug.Log($"🗑 Clearing chunk {chunkIndex} (Y range: {chunkPosition.y} to {chunkPosition.y + chunkHeight})");

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

            platformTilemap.RefreshAllTiles();
            hazardTilemap.RefreshAllTiles();
            decorationTilemap.RefreshAllTiles();
            wallTilemap.RefreshAllTiles();
            backgroundTilemap.RefreshAllTiles();

            Debug.Log($"✅ Successfully cleared chunk {chunkIndex}");
            _chunkPositions.Remove(chunkIndex);
        }
        
        private void OnDrawGizmos()
        {
            if (_chunkPositions == null) return;

            Gizmos.color = Color.green; // ✅ Color for active chunks

            foreach (var chunk in _chunkPositions)
            {
                Vector3 chunkBottomLeft = new Vector3(-10, chunk.Value.y, 0); // Left boundary
                Vector3 chunkBottomRight = new Vector3(10, chunk.Value.y, 0); // Right boundary
                Vector3 chunkTopLeft = new Vector3(-10, chunk.Value.y + chunkHeight, 0); // Upper left
                Vector3 chunkTopRight = new Vector3(10, chunk.Value.y + chunkHeight, 0); // Upper right

                // ✅ Draw chunk boundaries in Scene View
                Gizmos.DrawLine(chunkBottomLeft, chunkBottomRight); // Bottom
                Gizmos.DrawLine(chunkBottomRight, chunkTopRight); // Right side
                Gizmos.DrawLine(chunkTopRight, chunkTopLeft); // Top
                Gizmos.DrawLine(chunkTopLeft, chunkBottomLeft); // Left side
            }
        }
    }
}
