using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace _Scripts.Editor
{
    public class ChunkGeneratorEditor : EditorWindow
    {

        #region Variables

        // Chunk generation settings
        private int _chunkWidth = 40; // Width of the chunk in grid cells
        private int _chunkHeight = 99; // Height of the chunk in grid cells
        private bool _generateWalls = true; // Toggle for walls
        private bool _generateWallDecorations = true; // Toggle for wall decorations
        private bool _generatePlatforms = true; // Toggle for platforms
        private bool _generateSolidPlatforms = true; // Toggle for solid platforms
        private float _platformDensity; // Percentage of chunk filled with platforms ([0-1]) 
        private float _solidPlatformDensity; // Percentage of chunk filled with solid platforms ([0-1])
        private GameObject _chunkPrefab; // Prefab to be created or modified
        
        // Chunk accessible fields
        private Tilemap _wallsTilemap;
        
        // Trap settings
        private bool _dartTrapsGaps;
        private bool _fillDartTrapGaps; // Controls whether gaps are filled with traps or not
        private bool _dartsOnTheLeftWall; // Controls whether traps are on the left wall or not
        private bool _dartsOnTheRightWall; // Controls whether traps are on the right wall or not
        private int _amountOfGapsOnLeftWall = 1;
        private int _amountOfGapsOnRightWall = 1;
        private GameObject _dartTrapPrefabLeftWall; // Prefab to be used for traps
        private GameObject _dartTrapPrefabRightWall; // Prefab to be used for traps

        // Generation steps
        private GameObject _currentChunkInstance;
        private Dictionary<string, bool> _generationSteps = new Dictionary<string, bool>();

        #endregion
        
        #region CustomWindow

        [MenuItem("Level Design/Chunk Generator")]
        public static void ShowWindow()
        {
            GetWindow<ChunkGeneratorEditor>("Chunk Generator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Chunk Generation Settings", EditorStyles.boldLabel);

            // Basic settings
            _chunkWidth = EditorGUILayout.IntField("Chunk Width", _chunkWidth);
            _chunkHeight = EditorGUILayout.IntField("Chunk Height", _chunkHeight);

            GUILayout.Space(10);
            GUILayout.Label("Generation Features", EditorStyles.boldLabel);

            // Core generation toggles
            _generateWalls = EditorGUILayout.Toggle("Generate Walls", _generateWalls);
            _generateWallDecorations = EditorGUILayout.Toggle("Generate Wall Decorations", _generateWallDecorations);
            _generatePlatforms = EditorGUILayout.Toggle("Generate Platforms", _generatePlatforms);
            _generateSolidPlatforms = EditorGUILayout.Toggle("Generate Solid Platforms", _generateSolidPlatforms);

            GUILayout.Space(10);
            GUILayout.Label("Trap Settings", EditorStyles.boldLabel);
            
            // Trap settings
            _dartTrapsGaps = EditorGUILayout.Toggle("Dart Traps Gaps", _dartTrapsGaps);
            if (_dartTrapsGaps)
            {
                _dartsOnTheLeftWall = EditorGUILayout.Toggle("Dart Traps on Left Wall", _dartsOnTheLeftWall);
                if (_dartsOnTheLeftWall)
                {
                    _amountOfGapsOnLeftWall = EditorGUILayout.IntField("Left Wall Dart Trap Amount", _amountOfGapsOnLeftWall);
                }
                _dartsOnTheRightWall = EditorGUILayout.Toggle("Dart Traps on Right Wall", _dartsOnTheRightWall);
                if (_dartsOnTheRightWall)
                {
                    _amountOfGapsOnRightWall = EditorGUILayout.IntField("Right Wall Dart Trap Amount", _amountOfGapsOnRightWall);
                }
                _fillDartTrapGaps = EditorGUILayout.Toggle("Fill Gaps With Dart Traps", _fillDartTrapGaps);

                if (_fillDartTrapGaps)
                {
                    _dartTrapPrefabLeftWall = (GameObject)EditorGUILayout.ObjectField("Dart Trap Prefab", _dartTrapPrefabLeftWall, typeof(GameObject), false);
                    _dartTrapPrefabRightWall = (GameObject)EditorGUILayout.ObjectField("Dart Trap Prefab", _dartTrapPrefabRightWall, typeof(GameObject), false);
                }
            }
            
            GUILayout.Space(10);
            _chunkPrefab = (GameObject)EditorGUILayout.ObjectField("Chunk Prefab", _chunkPrefab, typeof(GameObject), false);

            // Generation buttons
            GUILayout.Space(20);
            if (GUILayout.Button("Generate New Chunk"))
            {
                if (_chunkPrefab)
                {
                    GenerateNewChunk();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select a valid chunk prefab.", "OK");
                }
            }

            // Only show modify buttons if we have an active chunk
            if (_currentChunkInstance)
            {
                GUILayout.Space(10);
                GUILayout.Label("Modify Current Chunk", EditorStyles.boldLabel);

                if (GUILayout.Button("Clear All"))
                {
                    ClearCurrentChunk();
                }

                if (GUILayout.Button("Rebuild With Current Settings"))
                {
                    RebuildCurrentChunk();
                }
            }
        }

        #endregion
        
        #region Chunk Generation
        
        private void GenerateNewChunk()
        {
            // Destroy the previous instance if it exists
            if (_currentChunkInstance)
            {
                DestroyImmediate(_currentChunkInstance);
            }

            // Create a new instance
            _currentChunkInstance = Instantiate(_chunkPrefab, new Vector3(-20, -5, 0), Quaternion.identity);
            _currentChunkInstance.name = "GeneratedChunk";

            // Generate the chunk with current settings
            RebuildCurrentChunk();
        }
        private void RebuildCurrentChunk()
        {
            if (!_currentChunkInstance) return;

            var gridTransform = _currentChunkInstance.transform.Find("Grid");
            
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
            
            // Generate base structure
            if (_generateWalls)
            {
                _wallsTilemap = gridTransform.Find("Tilemap_Collision_Walls")?.GetComponent<Tilemap>();
                if (_wallsTilemap)
                {
                    GenerateWalls(_wallsTilemap);
            
                    // Add traps if enabled
                    if (_dartTrapsGaps)
                    {
                        GenerateDartTrapGaps(_wallsTilemap);
                    }
                }
            }

            // Generate other elements in order
            if (_generateWallDecorations)
            {
                var wallDecorTilemap = gridTransform.Find("Tilemap_WallDeco")?.GetComponent<Tilemap>();
                if (wallDecorTilemap)
                {
                    GenerateWallDecorations(wallDecorTilemap);
                }
            }

            if (_generatePlatforms)
            {
                var platformsTilemap = gridTransform.Find("Tilemap_Collision_Platforms")?.GetComponent<Tilemap>();
                if (platformsTilemap)
                {
                    GenerateBasicPlatforms(platformsTilemap);
                }
            }
            
            if (_generateSolidPlatforms)
            {
                var solidPlatformsTilemap = gridTransform.Find("Tilemap_Collision_SolidPlatforms")?.GetComponent<Tilemap>();
                if (solidPlatformsTilemap)
                {
                    GenerateSolidPlatforms(solidPlatformsTilemap);
                }
            }
            
            Debug.Log("Chunk generated successfully!");
        }
        private void ClearCurrentChunk()
        {
            if (!_currentChunkInstance) return;

            var gridTransform = _currentChunkInstance.transform.Find("Grid");
            if (!gridTransform) return;

            // Clear all tilemaps
            var tilemaps = gridTransform.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                tilemap.ClearAllTiles();
            }
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
                for (var i = 0; i < 4; i++)
                {
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

        private void GenerateBasicPlatforms(Tilemap platformsTilemap)
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
            const float minYDistance = 5f;
            const float maxYDistance = 8f;
            //const float maxHorizontalGap = 7f; // Maximum allowed gap between platforms
            
            var currentY = 4;
            var lastX = Random.Range(4, _chunkWidth - 3);
            var goingRight = Random.value < 0.5f;
            var currentPattern = 0; // Track current pattern type

            while (currentY < _chunkHeight - 8)
            {
                currentY += Mathf.RoundToInt(Random.Range(minYDistance, maxYDistance));
                
                // Randomly change pattern type
                if (Random.value < 0.3f) // 30% chance to change pattern
                {
                    currentPattern = Random.Range(0, 3); // 0: staircase, 1: zigzag, 2: parallel
                }

                var isChangingDirection = false;

                // Direction change check
                if (goingRight && lastX > _chunkWidth - 15)
                {
                    isChangingDirection = true;
                    goingRight = false;
                }
                else if (!goingRight && lastX < 15)
                {
                    isChangingDirection = true;
                    goingRight = true;
                }

                // Generate platform based on current pattern
                switch (currentPattern)
                {
                    case 0:
                        GenerateStaircasePattern(platformsTilemap, platformRuleTile, ref lastX, currentY, ref goingRight, isChangingDirection);
                        break;
                    case 1:
                        GenerateZigzagPattern(platformsTilemap, platformRuleTile, ref lastX, currentY, ref goingRight, isChangingDirection);
                        break;
                    case 2:
                        GenerateParallelPattern(platformsTilemap, platformRuleTile, ref lastX, currentY, ref goingRight, isChangingDirection);
                        break;
                }
            }
        }

        private void GenerateStaircasePattern(Tilemap tilemap, RuleTile tile, ref int lastX, int y, ref bool goingRight, bool isDirectionChange)
        {
            var platformLength = Random.Range(6, 9);
            const int horizontalStep = 4;

            int startX = CalculateStartX(lastX, platformLength, horizontalStep, goingRight, isDirectionChange);
            startX = ClampStartPosition(startX, platformLength, ref goingRight);

            // Safety check for gaps
            if (!IsGapSafe(lastX, startX, y))
            {
                // Adjust position to ensure safe gap
                startX = CalculateSafePosition(lastX, platformLength, goingRight);
            }

            // Place platform
            if (CanPlacePlatform(startX, y, platformLength))
            {
                PlacePlatform(tilemap, tile, startX, y, platformLength);
                lastX = goingRight ? startX : startX + platformLength;
            }
        }

        private bool IsGapSafe(int prevX, int newX, int y)
        {
            const float maxSafeGap = 7f; // Maximum safe jumping distance
            return Mathf.Abs(prevX - newX) <= maxSafeGap;
        }

        private int CalculateSafePosition(int lastX, int platformLength, bool goingRight)
        {
            const float safeGap = 5f; // Ideal safe jumping distance
            return goingRight ? 
                lastX + Mathf.RoundToInt(safeGap) : 
                lastX - platformLength - Mathf.RoundToInt(safeGap);
        }

        private void GenerateZigzagPattern(Tilemap tilemap, RuleTile tile, ref int lastX, int y, ref bool goingRight, bool isDirectionChange)
        {
            var platformLength = Random.Range(4, 7);
            var horizontalOffset = Random.Range(3, 6);

            int startX = CalculateStartX(lastX, platformLength, horizontalOffset, goingRight, isDirectionChange);
            startX = ClampStartPosition(startX, platformLength, ref goingRight);

            // Safety check and platform placement
            if (IsGapSafe(lastX, startX, y) && CanPlacePlatform(startX, y, platformLength))
            {
                PlacePlatform(tilemap, tile, startX, y, platformLength);
                lastX = goingRight ? startX : startX + platformLength;
                goingRight = !goingRight; // Zigzag pattern changes direction after each platform
            }
        }

        private void GenerateParallelPattern(Tilemap tilemap, RuleTile tile, ref int lastX, int y, ref bool goingRight, bool isDirectionChange)
        {
            var platformLength = Random.Range(5, 8);
            
            // Create two parallel platforms
            for (int i = 0; i < 2; i++)
            {
                int startX = lastX + (goingRight ? 4 : -4);
                startX = ClampStartPosition(startX, platformLength, ref goingRight);

                if (IsGapSafe(lastX, startX, y) && CanPlacePlatform(startX, y + (i * 2), platformLength))
                {
                    PlacePlatform(tilemap, tile, startX, y + (i * 2), platformLength);
                    lastX = startX;
                }
            }
        }

        private bool CanPlacePlatform(int startX, int y, int length)
        {
            for (var x = startX; x < startX + length; x++)
            {
                if (!IsPositionValid(x, y))
                    return false;
            }
            return true;
        }

        private void PlacePlatform(Tilemap tilemap, RuleTile tile, int startX, int y, int length)
        {
            for (var x = startX; x < startX + length; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        private int CalculateStartX(int lastX, int platformLength, int horizontalStep, bool goingRight, bool isDirectionChange)
        {
            if (goingRight)
            {
                if (isDirectionChange)
                {
                    // When changing from left to right, start closer to the last platform
                    return lastX - (horizontalStep / 2);
                }
                else
                {
                    return lastX + horizontalStep;
                }
            }
            else
            {
                if (isDirectionChange)
                {
                    // When changing from right to left, start closer to the last platform
                    return lastX - platformLength - (horizontalStep / 2);
                }
                else
                {
                    return lastX - horizontalStep - platformLength;
                }
            }
        }

        private int ClampStartPosition(int startX, int platformLength, ref bool goingRight)
        {
            // Ensure we're within bounds and adjust direction if necessary
            if (startX < 4)
            {
                startX = 4;
                goingRight = true;  // Force direction change if too far left
            }
            else if (startX > _chunkWidth - platformLength - 4)
            {
                startX = _chunkWidth - platformLength - 4;
                goingRight = false;  // Force direction change if too far right
            }
            return startX;
        }

        private bool IsPositionValid(int x, int y)
        {
            // Check if a position collides with walls
            return !_wallsTilemap.HasTile(new Vector3Int(x, y, 0));
        }

        #endregion

        #region Solid Platforms Generation

        private void GenerateSolidPlatforms(Tilemap solidPlatformsTilemap)
        {
            solidPlatformsTilemap.ClearAllTiles();

            var solidPlatformRuleTile = Resources.Load<RuleTile>("Tiles/SolidPlatformsRuleTile");
            if (!solidPlatformRuleTile)
            {
                Debug.LogError("SolidPlatformRuleTile not found. Ensure it is placed in the 'Resources/Tiles' folder.");
                return;
            }

            // Total number of solid platforms to generate
            var totalSolidPlatforms = Mathf.RoundToInt(_chunkWidth * _chunkHeight);

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
        

        private void GenerateDartTrapGaps(Tilemap wallsTilemap)
        {
            // Create lists to track positions for each wall
            var leftWallGapPositions = new List<int>();
            var rightWallGapPositions = new List<int>();
            
            // Create a list of all possible Y positions
            var possibleHeights = new List<int>();
            for (var y = 10; y < _chunkHeight - 12; y++)
            {
                possibleHeights.Add(y);
            }

            // Generate gaps for the left wall
            if (_dartsOnTheLeftWall)
            {
                possibleHeights = possibleHeights.OrderBy(x => Random.value).ToList();
                GenerateWallGaps(wallsTilemap, true, _amountOfGapsOnLeftWall, possibleHeights, leftWallGapPositions);
            }
            
            // Generate gaps for the right wall
            if (_dartsOnTheRightWall)
            {
                possibleHeights = possibleHeights.OrderBy(x => Random.value).ToList();
                GenerateWallGaps(wallsTilemap, false, _amountOfGapsOnRightWall, possibleHeights, rightWallGapPositions);
            }
            
            if (_dartTrapsGaps)
            {
                var spawnPoint = _currentChunkInstance.transform.Find("SpawnPoints");
                var leftDartTrap = spawnPoint.Find("LeftWallDartTrap");
                var rightDartTrap = spawnPoint.Find("RightWallDartTrap");
                
                if (_dartsOnTheLeftWall)
                {
                    InstantiateDartTraps(leftWallGapPositions, true, leftDartTrap);;
                }
                    
                if (_dartsOnTheRightWall)
                {
                    InstantiateDartTraps(rightWallGapPositions, false, rightDartTrap);;
                }
            }
            
            Debug.Log($"Created {leftWallGapPositions.Count} gaps on left wall at heights: {string.Join(", ", leftWallGapPositions)}");
            Debug.Log($"Created {rightWallGapPositions.Count} gaps on right wall at heights: {string.Join(", ", rightWallGapPositions)}");
        }

        private void GenerateWallGaps(Tilemap wallsTilemap, bool isLeftWall, int numberOfGaps, 
            List<int> possibleHeights, List<int> selectedPositions)
        {
            var gapsCreated = 0;
            var currentIndex = 0;

            while (gapsCreated < numberOfGaps && currentIndex < possibleHeights.Count)
            {
                var currentHeight = possibleHeights[currentIndex];
                
                // Check if a position is safe
                var isSafe = true;
                foreach (var existingGap in selectedPositions)
                {
                    if (Mathf.Abs(existingGap - currentHeight) <= 10)
                    {
                        isSafe = false;
                        break;
                    }
                }
                
                if (isSafe)
                {
                    selectedPositions.Add(currentHeight);
                    gapsCreated++;
            
                    // Create a 2-tile high gap
                    if (isLeftWall)
                    {
                        // Create a gap in the left wall
                        wallsTilemap.SetTile(new Vector3Int(3, currentHeight, 0), null);
                        wallsTilemap.SetTile(new Vector3Int(3, currentHeight + 1, 0), null);
                    }
                    else
                    {
                        // Create a gap in the right wall
                        wallsTilemap.SetTile(new Vector3Int(_chunkWidth - 3, currentHeight, 0), null);
                        wallsTilemap.SetTile(new Vector3Int(_chunkWidth - 3, currentHeight + 1, 0), null);
                    }
                }
                currentIndex++;
            }
        }

        private void InstantiateDartTraps(List<int> gapPositions, bool isLeftWall, Transform parentTransform)
        {
            foreach (var gapPosition in gapPositions)
            {
                var xPosition = isLeftWall ? 3.25f : _chunkWidth - 2.25f;
        
                var trapPosition = new Vector3(xPosition - 20f, gapPosition - 4f, 0);
        
                var trapRotation = isLeftWall ? 
                    Quaternion.Euler(0, 0, 0) :    // Left wall traps face right
                    Quaternion.Euler(0, 180, 0);      // Right wall traps face left

                if (_fillDartTrapGaps)
                {
                    if (isLeftWall)
                    {
                        Instantiate(_dartTrapPrefabLeftWall, trapPosition, trapRotation, parentTransform);
                    }
                    else
                    {
                        Instantiate(_dartTrapPrefabRightWall, trapPosition, trapRotation, parentTransform);
                    }
                }
            }
        }
    }
}