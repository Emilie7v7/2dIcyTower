using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace _Scripts.Editor
{
    public class ChunkGeneratorEditor : EditorWindow
    {
        // Chunk generation settings
        private int _chunkWidth = 40; // Width of the chunk in grid cells
        private int _chunkHeight = 99; // Height of the chunk in grid cells
        private bool _generateWalls = true; // Toggle for walls
        private bool _generateWallDecorations = true; // Toggle for wall decorations
        private float _platformDensity; // Percentage of chunk filled with platforms ([0-1]) 
        private float _solidPlatformDensity; // Percentage of chunk filled with solid platforms ([0-1])
        private GameObject _chunkPrefab; // Prefab to be created or modified
        
        // Chunk accessible fields
        private Tilemap _wallsTilemap;

        [MenuItem("Level Design/Chunk Generator")]
        public static void ShowWindow()
        {
            GetWindow<ChunkGeneratorEditor>("Chunk Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Chunk Generation Settings", EditorStyles.boldLabel);

            _chunkWidth = EditorGUILayout.IntField("Chunk Width", _chunkWidth);
            _chunkHeight = EditorGUILayout.IntField("Chunk Height", _chunkHeight);

            _generateWalls = EditorGUILayout.Toggle("Generate Walls", _generateWalls);
            _generateWallDecorations = EditorGUILayout.Toggle("Generate Wall Decorations", _generateWallDecorations);

            _platformDensity = EditorGUILayout.Slider("Platform Density", _platformDensity, 0, 1);
            _solidPlatformDensity = EditorGUILayout.Slider("Solid Platform Density", _solidPlatformDensity, 0, 1);

            _chunkPrefab =
                (GameObject)EditorGUILayout.ObjectField("Chunk Prefab", _chunkPrefab, typeof(GameObject), false);

            if (GUILayout.Button("Generate Chunks"))
            {
                if (_chunkPrefab)
                {
                    GenerateChunk();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select a valid chunk prefab.", "OK");
                    Debug.LogWarning("Please assign a Chunk Prefab to generate!");
                }
            }
        }

        #region Chunk Generation

        private void GenerateChunk()
        {
            if (!_chunkPrefab) return;

            // Create a new instance of the chunk prefab
            var chunkInstance = Instantiate(_chunkPrefab, new Vector3(-20, -5, 0), Quaternion.identity);
            chunkInstance.name = "GeneratedChunk";

            // Find the grid and tilemaps
            var gridTransform = chunkInstance.transform.Find("Grid");

            if (!gridTransform)
            {
                Debug.LogError("Chunk prefab is missing a grid object!");
                return;
            }

            // Generate Background
            var backgroundTilemap = gridTransform.Find("Tilemap_Background")?.GetComponent<Tilemap>();
            if (backgroundTilemap)
            {
                GenerateBackground(backgroundTilemap);
            }
            
            // Generate Walls
            if (_generateWalls)
            {
                 _wallsTilemap = gridTransform.Find("Tilemap_Collision_Walls")?.GetComponent<Tilemap>();

                if (_wallsTilemap)
                {
                    GenerateWalls(_wallsTilemap);
                }
            }

            // Generate Wall Decorations
            if (_generateWallDecorations)
            {
                var wallDecorTilemap = gridTransform.Find("Tilemap_WallDeco")?.GetComponent<Tilemap>();

                if (wallDecorTilemap)
                {
                    GenerateWallDecorations(wallDecorTilemap);
                }
            }
            
            // Generate Platforms

            var platformsTilemap = gridTransform.Find("Tilemap_Collision_Platforms")?.GetComponent<Tilemap>();

            if (platformsTilemap)
            {
                GenerateBasicPlatforms(platformsTilemap, _platformDensity);
            }

            // Generate Solid Platforms

            var solidPlatformsTilemap = gridTransform.Find("Tilemap_Collision_SolidPlatforms")?.GetComponent<Tilemap>();
            if (solidPlatformsTilemap)
            {
                GenerateSolidPlatforms(solidPlatformsTilemap, _solidPlatformDensity);
            }
            
            Debug.Log("Chunk generated successfully!");
        }

        #endregion
        
        // TODO - Add a function to generate a chunk from a given seed
        // Implement a function to spawn dart traps at the gaps of the walls
        #region Walls Generation

        private void GenerateWalls(Tilemap wallsTilemap)
        {
            wallsTilemap.ClearAllTiles();
            
            var wallRuleTile = Resources.Load<RuleTile>("Tiles/WallRuleTile");

            for (var y = 0; y < _chunkHeight; y++)
            {
                // bool createGap = (y == 10 || y == 11) || // Two-block gap
                //                  y is >= 20 and <= 22 ||   // Three-block gap
                //                  y == 30;                  // Single-block gap

                
                for (var i = 0; i < 4; i++)
                {
                    // For testing, only create gaps in the outermost column (i == 3)
                    // if (createGap && i == 3)
                    //     continue;
                    
                    // Left wall
                    wallsTilemap.SetTile(new Vector3Int(0 + i, y, 0), wallRuleTile);
                    // Right wall
                    wallsTilemap.SetTile(new Vector3Int(_chunkWidth - i, y, 0), wallRuleTile);
                }
            }

            Debug.Log("Walls Generated");
        }
        
        private void GenerateWallDecorations(Tilemap wallDecorTilemap)
        {
            wallDecorTilemap.ClearAllTiles();
            
            // Load wall decor tiles (left and right)

            var leftWallDecorTile = Resources.Load<TileBase>("Tiles/WallDecor_Left");
            var rightWallDecorTile = Resources.Load<TileBase>("Tiles/WallDecor_Right");

            if (!leftWallDecorTile || !rightWallDecorTile)
            {
                Debug.LogError("Failed to load wall decoration tiles. Make sure the tiles are in the Resources folder.");
                return;
            }
            
            // Place wall decorations next to the walls
            for (var y = 0; y < _chunkHeight; y++)
            {
                // Check each column of the left wall (0 to 3)
                var leftWallFound = false;
                for (var x = 3; x >= 0; x--)
                {
                    if (_wallsTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        // Place left wall decoration right after the last wall tile
                        wallDecorTilemap.SetTile(new Vector3Int(x + 1, y, 0), leftWallDecorTile);
                        leftWallFound = true;
                        break;
                    }
                }
                
                // If no wall was found (complete gap), place decoration at the default position
                if (!leftWallFound)
                {
                    wallDecorTilemap.SetTile(new Vector3Int(4, y, 0), leftWallDecorTile);;
                }
                
                var rightWallFound = false;
                for (var x = _chunkWidth - 3; x <= _chunkWidth; x++)
                {
                    if (_wallsTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        // Place right wall decoration right before the first wall tile
                        wallDecorTilemap.SetTile(new Vector3Int(x - 1, y, 0), rightWallDecorTile);
                        rightWallFound = true;
                        break;
                    }
                }
                
                // If no wall was found (complete gap), place decoration at the default position
                if (!rightWallFound)
                {
                    wallDecorTilemap.SetTile(new Vector3Int(_chunkWidth - 4, y, 0), rightWallDecorTile);
                }
            }
            
            Debug.Log("Wall Decorations Generated");
        }
        #endregion
        
        #region Basic Platforms Generation

        private void GenerateBasicPlatforms(Tilemap platformsTilemap, float density)
        {
            platformsTilemap.ClearAllTiles();

            var platformRuleTile = Resources.Load<RuleTile>("Tiles/PlatformRuleTile");
            if (!platformRuleTile)
            {
                Debug.LogError("PlatformRuleTile not found. Ensure it is placed in the 'Resources/Tiles' folder.");
                return;
            }

            // Generate the initial full-width platform at the start
            for (var x = 4; x < _chunkWidth - 3; x++)
            {
                platformsTilemap.SetTile(new Vector3Int(x, 4, 0), platformRuleTile);
            }

            // Platform generation parameters
            float minYDistance = 6f;
            float maxYDistance = 8f;
            
            // Start position for the next platform
            int currentY = 4;
            int lastX = 4;
            bool goingRight = Random.value < 0.5f; // Random initial direction

            while (currentY < _chunkHeight -7)
            {
                // Calculate next Y position with smaller, more consistent steps
                currentY += Mathf.RoundToInt(Random.Range(minYDistance, maxYDistance));
                GenerateStaircaseStep(platformsTilemap, platformRuleTile, ref lastX, currentY, ref goingRight);

                // Change direction if we're too close to the walls
                if (goingRight && lastX > _chunkWidth - 15)
                {
                    goingRight = false;
                    // Add a connecting platform when changing direction
                    GenerateConnectingPlatform(platformsTilemap, platformRuleTile, lastX, currentY);
                }
                else if (!goingRight && lastX < 15)
                {
                    goingRight = true;
                    // Add a connecting platform when changing direction
                    GenerateConnectingPlatform(platformsTilemap, platformRuleTile, lastX, currentY);
                }
            }
        }

        private void GenerateStaircaseStep(Tilemap tilemap, RuleTile tile, ref int lastX, int y, ref bool goingRight)
        {
            int platformLength = 7; // Fixed length for staircase platforms
            int horizontalStep = 5; // Distance between platforms horizontally
            
            // Calculate start position based on direction
            int startX;
            if (goingRight)
            {
                startX = lastX;
                lastX += horizontalStep; // Move right for next platform
            }
            else
            {
                startX = lastX - platformLength;
                lastX -= horizontalStep; // Move left for next platform
            }

            // Ensure we're within bounds
            if (startX < 4)
            {
                startX = 4;
                goingRight = true;
            }
            else if (startX > _chunkWidth - platformLength - 4)
            {
                startX = _chunkWidth - platformLength - 4;
                goingRight = false;
            }

            // Place platform if not colliding with walls
            bool canPlace = true;
            for (int x = startX; x < startX + platformLength; x++)
            {
                if (!IsPositionValid(x, y))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                for (int x = startX; x < startX + platformLength; x++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        private void GenerateConnectingPlatform(Tilemap tilemap, RuleTile tile, int x, int y)
        {
            // Generate a longer platform when changing direction to create a landing
            int connectingLength = 12;
            int startX = x - connectingLength / 2;
            
            // Ensure we're within bounds
            startX = Mathf.Clamp(startX, 4, _chunkWidth - connectingLength - 4);

            // Check if we can place the platform
            bool canPlace = true;
            for (int i = startX; i < startX + connectingLength; i++)
            {
                if (!IsPositionValid(i, y))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                for (int i = startX; i < startX + connectingLength; i++)
                {
                    tilemap.SetTile(new Vector3Int(i, y, 0), tile);
                }
            }
        }

        private bool IsPositionValid(int x, int y)
        {
            // Check if position collides with walls
            return !_wallsTilemap.HasTile(new Vector3Int(x, y, 0));
        }


        #endregion

        #region Solid Platforms Generation

        private void GenerateSolidPlatforms(Tilemap solidPlatformsTilemap, float density)
        {
            solidPlatformsTilemap.ClearAllTiles();

            var solidPlatformRuleTile = Resources.Load<RuleTile>("Tiles/SolidPlatformsRuleTile");
            if (solidPlatformRuleTile == null)
            {
                Debug.LogError("SolidPlatformRuleTile not found. Ensure it is placed in the 'Resources/Tiles' folder.");
                return;
            }

            // Total number of solid platforms to generate
            var totalSolidPlatforms = Mathf.RoundToInt(_chunkWidth * _chunkHeight * density);

            for (var i = 0; i < totalSolidPlatforms; i++)
            {
                // Choose a random starting position
                var startX = Random.Range(2, _chunkWidth - 6); // Leave space for the shape to fit
                var startY = Random.Range(2, _chunkHeight - 6);

                // Randomly choose a shape
                var shapeType = Random.Range(0, 3); // 0 = cube, 1 = stairs, 2 = reversed T
                switch (shapeType)
                {
                    case 0:
                        GenerateCube(solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                        break;
                    case 1:
                        GenerateStairs(solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                        break;
                    case 2:
                        GenerateReversedT(solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                        break;
                }
            }

            Debug.Log($"Solid Platforms Generated: {totalSolidPlatforms} platforms placed.");
        }

        // Cube shape generator
        private static void GenerateCube(Tilemap tilemap, RuleTile tile, int startX, int startY)
        {
            var size = Random.Range(2, 5); // Random cube size
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), tile);
                }
            }
        }

        // Stairs shape generator
        private static void GenerateStairs(Tilemap tilemap, RuleTile tile, int startX, int startY)
        {
            var height = Random.Range(3, 6); // Random height of stairs
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j <= i; j++) // Create the "step effect"
                {
                    tilemap.SetTile(new Vector3Int(startX + j, startY + i, 0), tile);
                }
            }
        }

        // Reversed T shape generator
        private static void GenerateReversedT(Tilemap tilemap, RuleTile tile, int centerX, int centerY)
        {
            // Center column (vertical part of the T)
            var height = Random.Range(3, 6); // Random height
            for (var y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(centerX, centerY + y, 0), tile);
            }

            // Horizontal part (top of the T)
            var armLength = Random.Range(2, 5); // Arm length
            for (var x = -armLength; x <= armLength; x++)
            {
                tilemap.SetTile(new Vector3Int(centerX + x, centerY + height - 1, 0), tile);
            }
        }

        #endregion
        
        #region Background Generation

        private void GenerateBackground(Tilemap backgroundTilemap)
        {
            backgroundTilemap.ClearAllTiles();

            var backgroundTile = Resources.Load<RuleTile>("Tiles/BackgroundBrickWallRuleTile");
            if (!backgroundTile)
            {
                Debug.LogError("BackgroundRuleTile not found. Ensure it is in the 'Resources/Tiles' folder.");
                return;
            }

            // Populate the entire background
            for (var x = 0; x < _chunkWidth; x++)
            {
                for (var y = 0; y < _chunkHeight; y++)
                {
                    backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
                }
            }

            // Introduce random deletions
            //RandomlyDeleteBackgroundTiles(backgroundTilemap);

            Debug.Log("Background Generated with random removals.");
        }

        private void RandomlyDeleteBackgroundTiles(Tilemap backgroundTilemap)
        {
            // Deletion parameters
            var totalDeletions = Mathf.RoundToInt(_chunkWidth * _chunkHeight * 0.2f); // Change 0.2f to specify density of deletion
            var snakeDeletionChance = 4; // Chance that deletion continues as a snake (out of 10)

            for (var i = 0; i < totalDeletions; i++)
            {
                // Choose a random starting point for deletion
                var x = Random.Range(1, _chunkWidth - 1);
                var y = Random.Range(1, _chunkHeight - 1);

                Vector3Int start = new Vector3Int(x, y, 0);

                // Random chance: use snake deletion
                if (Random.Range(0, 10) < snakeDeletionChance)
                {
                    PerformSnakeDeletion(backgroundTilemap, start, Random.Range(4, 10)); // Random snake length
                }
                else
                {
                    PerformClusterDeletion(backgroundTilemap, start, Random.Range(1, 4)); // Random cluster size
                }
            }
        }

        // Create an irregular snake-like deletion
        private void PerformSnakeDeletion(Tilemap tilemap, Vector3Int start, int length)
        {
            var current = start;

            for (var i = 0; i < length; i++)
            {
                tilemap.SetTile(current, null); // Remove tile at the current position

                // Randomize the next direction
                var direction = Random.Range(0, 4); // 0 = up, 1 = right, 2 = down, 3 = left
                switch (direction)
                {
                    case 0: current += Vector3Int.up; break;
                    case 1: current += Vector3Int.right; break;
                    case 2: current += Vector3Int.down; break;
                    case 3: current += Vector3Int.left; break;
                }

                // Avoid going out of bounds
                if (current.x < 1 || current.x >= _chunkWidth - 1 || current.y < 1 || current.y >= _chunkHeight - 1)
                {
                    break;
                }
            }
        }

        // Create a circular cluster deletion
        private void PerformClusterDeletion(Tilemap tilemap, Vector3Int center, int radius)
        {
            for (var x = center.x - radius; x <= center.x + radius; x++)
            {
                for (var y = center.y - radius; y <= center.y + radius; y++)
                {
                    var pos = new Vector3Int(x, y, 0);

                    // Check for bounds
                    if (x > 0 && x < _chunkWidth && y > 0 && y < _chunkHeight)
                    {
                        // Random chance of deleting within cluster
                        if (Vector3Int.Distance(center, pos) <= radius && Random.Range(0, 2) == 0)
                        {
                            tilemap.SetTile(pos, null);
                        }
                    }
                }
            }
        }

        #endregion

        #region Gizmos

        // Define debug visualization color options
        private Color platformGizmoColor = Color.green;
        private Color wallGizmoColor = Color.red;
        private Color solidPlatformGizmoColor = Color.blue;

        // Call this inside the GenerateChunk function (optional) to save positions for visualization
        private List<Vector3Int> platformTiles = new List<Vector3Int>();
        private List<Vector3Int> wallTiles = new List<Vector3Int>();
        private List<Vector3Int> solidPlatformTiles = new List<Vector3Int>();

        private void GenerateGizmoVisualization(Tilemap platformsTilemap, Tilemap wallsTilemap, Tilemap solidPlatformsTilemap)
        {
            // Clear previous visualizations
            platformTiles.Clear();
            wallTiles.Clear();
            solidPlatformTiles.Clear();

            // Store tile positions for platforms
            foreach (var pos in platformsTilemap.cellBounds.allPositionsWithin)
            {
                if (platformsTilemap.HasTile(pos)) platformTiles.Add(pos);
            }

            // Store tile positions for walls
            foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
            {
                if (wallsTilemap.HasTile(pos)) wallTiles.Add(pos);
            }

            // Store tile positions for solid platforms
            foreach (var pos in solidPlatformsTilemap.cellBounds.allPositionsWithin)
            {
                if (solidPlatformsTilemap.HasTile(pos)) solidPlatformTiles.Add(pos);
            }
        }

        private void OnDrawGizmos()
        {
            // Visualize Platform Tiles
            Gizmos.color = platformGizmoColor;
            foreach (var pos in platformTiles)
            {
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one * 0.9f); // Draw cubes for tiles
            }

            // Visualize Wall Tiles
            Gizmos.color = wallGizmoColor;
            foreach (var pos in wallTiles)
            {
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one * 0.9f);
            }

            // Visualize Solid Platform Tiles
            Gizmos.color = solidPlatformGizmoColor;
            foreach (var pos in solidPlatformTiles)
            {
                Gizmos.DrawCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Vector3.one * 0.9f);
            }
        }

        #endregion
        
    }
}