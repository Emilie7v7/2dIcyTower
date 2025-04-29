using System.Collections.Generic;
using _Scripts.ChunkGeneration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Editor
{
    internal enum ChunkGenerationType {Default, Easy, Medium, Hard}

    public class ChunkGeneratorEditor : EditorWindow
    {
        #region Variables

        private ChunkGenerationType selectedType = ChunkGenerationType.Default;
        private int _chunkWidth = 40;
        private int _chunkHeight = 99;

        [Header("Tilemap Prefab and Instance")]
        private GameObject _chunkPrefab; // Prefab for the chunk
        private GameObject _currentChunkInstance; // Instance of the prefab

        [Header("Generators")] [SerializeField]
        private List<ScriptableObject> generators; // ScriptableObject-based generators (e.g., WallsGenerator)

        #endregion
        
        // Create a window for the Chunk Generator
        [MenuItem("Level Design/Chunk Generator")]
        public static void ShowWindow()
        {
            GetWindow<ChunkGeneratorEditor>("Chunk Generator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Chunk Generator Settings", EditorStyles.boldLabel);

            // Chunk settings
            selectedType = (ChunkGenerationType)EditorGUILayout.EnumPopup("Chunk Type", selectedType);
            _chunkWidth = EditorGUILayout.IntField("Chunk Width", _chunkWidth);
            _chunkHeight = EditorGUILayout.IntField("Chunk Height", _chunkHeight);

            // Chunk prefab selection
            _chunkPrefab =
                (GameObject)EditorGUILayout.ObjectField("Chunk Prefab", _chunkPrefab, typeof(GameObject), false);

            EditorGUILayout.Space();

            // Generators list
            GUILayout.Label("Generator List", EditorStyles.boldLabel);
            generators ??= new List<ScriptableObject>();

            // Draw the generator list in inspector
            for (var i = 0; i < generators.Count; i++)
            {
                generators[i] = (ScriptableObject)EditorGUILayout.ObjectField($"Generator {i + 1}", generators[i],
                    typeof(ScriptableObject), false);
            }

            if (GUILayout.Button("Add Generator"))
            {
                generators.Add(null);
            }

            if (GUILayout.Button("Remove Last Generator"))
            {
                if (generators.Count > 0)
                    generators.RemoveAt(generators.Count - 1);
            }

            EditorGUILayout.Space();

            // Generate or clear buttons
            if (GUILayout.Button("Generate New Chunk"))
            {
                GenerateChunk();
            }

            if (GUILayout.Button("Clear Current Chunk"))
            {
                ClearCurrentChunk();
            }
        }

        #region Chunk Generation

        private void GenerateChunk()
        {
            if (!_chunkPrefab)
            {
                Debug.LogError("No chunk prefab assigned!");
                return;
            }

            // Destroy a previous chunk instance if one exists
            if (_currentChunkInstance)
            {
                DestroyImmediate(_currentChunkInstance);
            }

            // Instantiate a new chunk instance
            _currentChunkInstance = Instantiate(_chunkPrefab, new Vector3(-20, -5, 0), Quaternion.identity);
            var chunkName = selectedType switch
            {
                ChunkGenerationType.Default => "GeneratedChunk",
                ChunkGenerationType.Easy => "Easy",
                ChunkGenerationType.Medium => "Medium",
                ChunkGenerationType.Hard => "Hard",
                _ => "GeneratedChunk"
            };
            _currentChunkInstance.name = chunkName;
            
            // Find the Tilemap objects in the new chunk instance
            var backgroundTilemap = FindTilemap(_currentChunkInstance, "Tilemap_Background");
            var wallsTilemap = FindTilemap(_currentChunkInstance, "Tilemap_Collision_Walls");
            var wallDecorTilemap = FindTilemap(_currentChunkInstance, "Tilemap_WallDeco");
            var platformsTilemap = FindTilemap(_currentChunkInstance, "Tilemap_Collision_Platforms");
            var solidPlatformsTilemap = FindTilemap(_currentChunkInstance, "Tilemap_Collision_SolidPlatforms");
            var decorationsTilemap1 = FindTilemap(_currentChunkInstance, "Tilemap_Decorations");
            

            // Pass relevant Tilemaps to the generators
            foreach (var generatorSo in generators)
            {
                if (generatorSo is IChunkGenerator generator)
                {
                    switch (generatorSo)
                    {
                        case WallsGenerator wallsGenerator:
                            wallsGenerator.Setup(_chunkWidth, _chunkHeight, wallsTilemap);
                            break;
                        case DartTrapGenerator dartTrapGenerator:
                            dartTrapGenerator.GetChunkInstance(_currentChunkInstance);
                            dartTrapGenerator.Setup(_chunkWidth, _chunkHeight, wallsTilemap);
                            break;
                        case WallDecorationGenerator wallDecorationGenerator:
                            wallDecorationGenerator.GetWallTilemap(wallsTilemap);
                            wallDecorationGenerator.Setup(_chunkWidth, _chunkHeight, wallDecorTilemap);
                            break;
                        case BackgroundGenerator backgroundGenerator:
                            backgroundGenerator.Setup(_chunkWidth, _chunkHeight, backgroundTilemap);
                            backgroundGenerator.GetDecorationsTilemap1(decorationsTilemap1);
                            break;
                        case WoodenPlatformsGenerator woodenPlatformsGenerator:
                            woodenPlatformsGenerator.Setup(_chunkWidth, _chunkHeight, platformsTilemap);
                            woodenPlatformsGenerator.GetTilemapsByType(wallsTilemap, solidPlatformsTilemap);
                            woodenPlatformsGenerator.GenerateFullWidthPlatform(platformsTilemap);
                            break;
                        case SolidPlatformGenerator solidPlatformGenerator:
                            solidPlatformGenerator.Setup(_chunkWidth, _chunkHeight, solidPlatformsTilemap);
                            solidPlatformGenerator.GetTilemapsByType(wallsTilemap, platformsTilemap);
                            break;
                        case CoinGenerator coinGenerator:
                            coinGenerator.Setup(_chunkWidth, _chunkHeight, null);
                            coinGenerator.GetChunkInstance(_currentChunkInstance);
                            coinGenerator.GetTilemapsByType(wallsTilemap, solidPlatformsTilemap, platformsTilemap);
                            break;
                        case EnemyGenerator enemyGenerator:
                            enemyGenerator.Setup(_chunkWidth, _chunkHeight, null);
                            enemyGenerator.GetChunkInstance(_currentChunkInstance);
                            enemyGenerator.GetTilemapsByType(wallsTilemap, solidPlatformsTilemap, platformsTilemap);
                            break;
                    }
                    
                    // Generate the chunk elements
                    generator.Generate();
                }
            }
        }

        private void ClearCurrentChunk()
        {
            if (!_currentChunkInstance)
            {
                Debug.LogError("No current chunk to clear!");
                return;
            }

            // Clear all tilemaps in the chunk
            var tilemaps = _currentChunkInstance.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                tilemap.ClearAllTiles();
            }
            // Clear all game objects in the chunk
            DestroyAllSpawnedGameObjects(_currentChunkInstance);
            
        }

        // Utility to find a specific Tilemap by name in the chunk prefab hierarchy
        private static Tilemap FindTilemap(GameObject parent, string tilemapName)
        {
            var gridTransform = parent.transform.Find("Grid");
            if (!gridTransform)
            {
                Debug.LogError("No Grid transform found in chunk prefab!");
                return null;
            }

            return gridTransform.Find(tilemapName)?.GetComponent<Tilemap>();
        }

        private static void DestroyAllSpawnedGameObjects(GameObject parent)
        {
            var spawnPointTransform = parent.transform.Find("SpawnPoints");
            if (!spawnPointTransform)
            {
                Debug.LogError("No SpawnPoints transform found in chunk prefab!");
            }
            
            var coinSpawnPoint = spawnPointTransform.Find("Coins");
            var enemySpawnPoint = spawnPointTransform.Find("Enemies");
            var dartTrapLeftSpawnPoint = spawnPointTransform.Find("LeftWallDartTrap");
            var dartTrapRightSpawnPoint = spawnPointTransform.Find("RightWallDartTrap");

            var listOfTransforms = new List<Transform>
            {
                coinSpawnPoint,
                enemySpawnPoint,
                dartTrapLeftSpawnPoint,
                dartTrapRightSpawnPoint
            };

            foreach (var child in listOfTransforms)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        #endregion

    }
}