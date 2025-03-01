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

        [Header("Spawn Settings")]
        [SerializeField] private PlatformSpawnSettingsSo platformSettings;
        [SerializeField] private HazardSpawnSettingsSo hazardSettings;
        [SerializeField] private DecorationSpawnSettingsSo decorationSettings;
        [SerializeField] private ObjectSpawnSettingsSo objectSettings;

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

        private void GeneratePlatforms(int chunkIndex, Vector3Int chunkPosition)
        {
            var platformCount = Random.Range(2, 5);

            for (var i = 0; i < platformCount; i++)
            {
                var width = Random.Range((int) platformSettings.minWidth, (int) platformSettings.maxWidth);
                var x = Random.Range(-5, 5);
                var y = chunkPosition.y + Random.Range(5, chunkHeight - 5);

                for (var j = 0; j < width; j++)
                {
                    var tile = platformSettings.platformTiles[Random.Range(0, platformSettings.platformTiles.Length)];
                    platformTilemap.SetTile(new Vector3Int(x + j, y, 0), tile);
                }
            }
        }

        private void GenerateHazards(int chunkIndex, Vector3Int chunkPosition)
        {
            if (Random.value <= hazardSettings.spawnProbability)
            {
                var x = Random.Range(-5, 5);
                var y = chunkPosition.y + Random.Range(5, chunkHeight - 5);
                var tile = hazardSettings.hazardTiles[Random.Range(0, hazardSettings.hazardTiles.Length)];
                hazardTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        private void GenerateDecorations(int chunkIndex, Vector3Int chunkPosition)
        {
            var decorationCount = Random.Range(1, decorationSettings.maxDecorationsPerChunk);

            for (var i = 0; i < decorationCount; i++)
            {
                var x = Random.Range(-5, 5);
                var y = chunkPosition.y + Random.Range(5, chunkHeight - 5);
                var tile = decorationSettings.decorationTiles[Random.Range(0, decorationSettings.decorationTiles.Length)];
                decorationTilemap.SetTile(new Vector3Int(x, y, 0), tile);
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

                if (objectSettings.mustSpawnOnPlatform)
                {
                    var platformCheckPosition = new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y -1), 0);
                    if(!platformTilemap.HasTile(platformCheckPosition)) return;
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
