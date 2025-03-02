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
        [SerializeField] private int activeChunkLimit = 3;
        
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
            var playerChunk = Mathf.FloorToInt(playerPosition.transform.position.y / chunkHeight);

            if (playerChunk > _lastGeneratedChunk)
            {
                GenerateChunk(playerChunk);
            }
        }

        private void GenerateChunk(int chunkIndex)
        {
            if (_activeChunks.Contains(chunkIndex)) return;
            
            Debug.Log($"Generating chunk {chunkIndex}");
            
            var chunkPosition = new Vector3Int(0, chunkIndex * chunkHeight, 0);
            _chunkPositions[chunkIndex] = chunkPosition;
            
            if (chunkIndex == 0)
            {
                GenerateFullWidthPlatform(chunkPosition);
            }
            
            //Generate Walls
            GenerateWalls(chunkIndex, chunkPosition);
            
            //Generate Background
            GenerateBackground(chunkIndex, chunkPosition);
            
            //Generate platforms
            GeneratePlatforms(chunkIndex, chunkPosition);
            
            //Generate Hazards
            GenerateHazards(chunkIndex, chunkPosition);
            
            //Generate Decorations
            GenerateDecorations(chunkIndex, chunkPosition);
            
            //Generate Objects
            GenerateObjects(chunkIndex, chunkPosition);
            
            
            _activeChunks.Enqueue(chunkIndex);
            _lastGeneratedChunk = chunkIndex;

            if (_activeChunks.Count > activeChunkLimit)
            {
                var oldChunk = _activeChunks.Dequeue();
                ClearChunks(oldChunk);
            }
        }
        
        private void GenerateFullWidthPlatform(Vector3Int chunkPosition)
        {
            var startX = wallSettings.leftWallX + 1; // Avoid placing inside walls
            var endX = wallSettings.rightWallX - 1;  // Avoid placing inside walls

            var platformY = chunkPosition.y - 1; // 🔹 Place slightly below the player

            for (var x = startX; x <= endX; x++)
            {
                TileBase selectedTile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                platformTilemap.SetTile(new Vector3Int(x, platformY, 0), selectedTile);
            }
        }
        
        private void GenerateWalls(int chunkIndex, Vector3Int chunkPosition)
        {
            for (var y = chunkPosition.y; y < chunkPosition.y + chunkHeight; y++)
            {
                // Select correct RuleTile based on height
                var selectedWallTile = wallSettings.wallRuleTiles[0];

                if (wallSettings.changeWallStyleAtHeight && wallSettings.wallRuleTiles.Length > 1)
                {
                    var styleIndex = (y / wallSettings.wallStyleChangeInterval) % wallSettings.wallRuleTiles.Length;
                    selectedWallTile = wallSettings.wallRuleTiles[styleIndex];
                }

                // Place 4 tiles wide on each side
                for (var i = 0; i < 4; i++)
                {
                    wallTilemap.SetTile(new Vector3Int(wallSettings.leftWallX + i, y, 0), selectedWallTile);
                    wallTilemap.SetTile(new Vector3Int(wallSettings.rightWallX - i, y, 0), selectedWallTile);
                }
            }
        }

        private void GenerateBackground(int chunkIndex, Vector3Int chunkPosition)
        {
            for (var x = -20; x <= 20; x++) // Covers the whole screen width
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
        
        private void GeneratePlatforms(int chunkIndex, Vector3Int chunkPosition)
        {
            var platformCount = Random.Range(2, 5);

            // Randomly select a platform type (RuleTile) for this chunk
            var selectedPlatform = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];

            for (var i = 0; i < platformCount; i++)
            {
                var width = Random.Range((int) platformSettings.minWidth, (int) platformSettings.maxWidth);
                var x = Random.Range(-5, 5);
                var y = chunkPosition.y + Random.Range(5, chunkHeight - 5);

                // Place tiles using RuleTile
                for (var j = 0; j < width; j++)
                {
                    platformTilemap.SetTile(new Vector3Int(x + j, y, 0), selectedPlatform);
                }
            }
        }

        private void GenerateHazards(int chunkIndex, Vector3Int chunkPosition)
        {
            if (Random.value <= hazardSettings.spawnProbability)
            {
                var x = Random.Range(-5, 5);
                var y = chunkPosition.y + Random.Range(5, chunkHeight - 5);

                if (hazardSettings.ruleTileHazards.Length > 0 && Random.value < 0.5f) // 50% chance to use RuleTile hazard
                {
                    var ruleTile = hazardSettings.ruleTileHazards[Random.Range(0, hazardSettings.ruleTileHazards.Length)];
                    hazardTilemap.SetTile(new Vector3Int(x, y, 0), ruleTile);
                }
                else if (hazardSettings.singleTileHazards.Length > 0)
                {
                    var singleTile = hazardSettings.singleTileHazards[Random.Range(0, hazardSettings.singleTileHazards.Length)];
                    hazardTilemap.SetTile(new Vector3Int(x, y, 0), singleTile);
                }
            }
        }
        private void GenerateDecorations(int chunkIndex, Vector3Int chunkPosition)
        {   
            var decorationCount = Random.Range(1, decorationSettings.maxDecorationsPerChunk);

            for (var i = 0; i < decorationCount; i++)
            {
                    var x = Random.Range(-5, 5);
                    var y = chunkPosition.y + Random.Range(5, chunkHeight - 5);
                    var spawnPosition = new Vector3Int(x, y, 0);

                // Platform Decorations (RuleTile)
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

                // Platform Decorations (Single Tile)
                if (decorationSettings.platformSingleTileDecorations.Length > 0)
                {
                    int index = Random.Range(0, decorationSettings.platformSingleTileDecorations.Length);
                    TileBase selectedTile = decorationSettings.platformSingleTileDecorations[index];

                    Vector3Int platformCheckPos = new Vector3Int(x, y - 1, 0);
                    if (platformTilemap.HasTile(platformCheckPos)) 
                    {
                        decorationTilemap.SetTile(spawnPosition, selectedTile);
                    }
                }

                // Air Decorations (RuleTile)
                if (decorationSettings.airRuleTileDecorations.Length > 0)
                {
                    int index = Random.Range(0, decorationSettings.airRuleTileDecorations.Length);
                    RuleTile selectedTile = decorationSettings.airRuleTileDecorations[index];

                    // No platform check needed for air decorations
                    decorationTilemap.SetTile(spawnPosition, selectedTile);
                }

                // Air Decorations (Single Tile)
                if (decorationSettings.airSingleTileDecorations.Length > 0)
                {
                    int index = Random.Range(0, decorationSettings.airSingleTileDecorations.Length);
                    TileBase selectedTile = decorationSettings.airSingleTileDecorations[index];

                    // No platform check needed for air decorations
                    decorationTilemap.SetTile(spawnPosition, selectedTile);
                }
            }
        }

        private void GenerateObjects(int chunkIndex, Vector3Int chunkPosition)
        {
            if (Random.value <= objectSettings.spawnProbability)
            {
                var objPrefab = objectSettings.objectPrefabs[Random.Range(0, objectSettings.objectPrefabs.Length)];
                var x = Random.Range(-4f, 4f);
                var y = chunkPosition.y + Random.Range(5f, chunkHeight - 5f);
                var spawnPosition = new Vector3(x, y, 0);

                if (objectSettings.objectType == ObjectType.Enemy)
                {
                    // If it's an enemy, it must spawn on a platform
                    var platformCheckPosition = new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y - 1), 0);
                    if (!platformTilemap.HasTile(platformCheckPosition)) return; // Skip spawn if no platform
                }

                Instantiate(objPrefab, spawnPosition, Quaternion.identity);
            }
        }

        
        
        private void ClearChunks(int chunkIndex)
        {
            if(!_chunkPositions.ContainsKey(chunkIndex)) return;
            
            var chunkPosition = _chunkPositions[chunkIndex];

            for (var x = -10; x <= 10 ; x++)
            {
                for (var j = 0; j < chunkHeight; j++)
                {
                    platformTilemap.SetTile(new Vector3Int(x, chunkPosition.y + j, 0), null);
                    hazardTilemap.SetTile(new Vector3Int(x, chunkPosition.y + j, 0), null);
                    decorationTilemap.SetTile(new Vector3Int(x, chunkPosition.y + j, 0), null);
                }
            }
            
            _chunkPositions.Remove(chunkIndex);
        }
    }
}
