using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace _Scripts.Editor
{
    internal enum ChunkGenerationType {Default, Easy, Medium, Hard}
    internal enum SolidPlatformType {None, Cube, UpsideDownT, ShapeT}
    public class ChunkGeneratorEditor : EditorWindow
    {

        #region Variables

        // Chunk generation settings
        private ChunkGenerationType selectedType = ChunkGenerationType.Default; // Controls the type of chunk to be generated
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
        private Tilemap _backgroundTilemap;
        private Tilemap _wallsTilemap;
        private Tilemap _platformsTilemap;
        private Tilemap _solidPlatformsTilemap;
        private Tilemap _wallDecorTilemap;
        
        
        // Trap settings
        private bool _dartTrapsGaps;
        private bool _fillDartTrapGaps; // Controls whether gaps are filled with traps or not
        private bool _dartsOnTheLeftWall; // Controls whether traps are on the left wall or not
        private bool _dartsOnTheRightWall; // Controls whether traps are on the right wall or not
        private int _amountOfGapsOnLeftWall = 1;
        private int _amountOfGapsOnRightWall = 1;
        private GameObject _dartTrapPrefabLeftWall; // Prefab to be used for traps
        private GameObject _dartTrapPrefabRightWall; // Prefab to be used for traps
        
        // Solid Platform settings
        private SolidPlatformType _solidPlatformType = SolidPlatformType.None; // Type of solid platform to be generated
        private Vector2Int newPlatformPosition = Vector2Int.zero;
        
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
            #region Core Chunk Generation Settings
            
            GUILayout.Label("Chunk Generation Settings", EditorStyles.boldLabel);

            // Create an enum popup in the inspector to select the chunk type
            selectedType = (ChunkGenerationType)EditorGUILayout.EnumPopup("Chunk Type", selectedType);

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
            
            #endregion
            
            GUILayout.Space(10);
            
            #region Trap Settings
            
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
            
            #endregion
            
            GUILayout.Space(10);
            
            #region Solid Platforms Settings
            
            GUILayout.Label("Solid Platforms Settings", EditorStyles.boldLabel);
            _solidPlatformType = (SolidPlatformType)EditorGUILayout.EnumPopup("Solid Platform Type", _solidPlatformType);
            
            // Position controls
            newPlatformPosition = EditorGUILayout.Vector2IntField("Position:", newPlatformPosition);
    
            if (GUILayout.Button("Add at Position"))
            {
                if (_solidPlatformsTilemap && _solidPlatformType != SolidPlatformType.None)
                {
                    var solidPlatformRuleTile = Resources.Load<RuleTile>("Tiles/SolidPlatformsRuleTile");
                    if (!solidPlatformRuleTile)
                    {
                        Debug.LogError("SolidPlatformRuleTile not found. Ensure it is placed in the 'Resources/Tiles' folder.");
                        return;
                    }
                    // Check if the position is valid
                    if (IsSufficientSpaceForPlatform(newPlatformPosition.x, newPlatformPosition.y, _solidPlatformsTilemap))
                    {
                        switch (_solidPlatformType)
                        {
                            case SolidPlatformType.Cube:
                                GenerateCube(_solidPlatformsTilemap, solidPlatformRuleTile, newPlatformPosition.x, newPlatformPosition.y);
                                break;
                            case SolidPlatformType.UpsideDownT:
                                GenerateShapeT(_solidPlatformsTilemap, solidPlatformRuleTile, newPlatformPosition.x, newPlatformPosition.y, true);
                                break;
                            case SolidPlatformType.ShapeT:
                                GenerateShapeT(_solidPlatformsTilemap, solidPlatformRuleTile, newPlatformPosition.x, newPlatformPosition.y, false);
                                break;
                            case SolidPlatformType.None:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Not enough space at specified position");
                    }
                }
            }

            if (GUILayout.Button("Add at Random Position"))
            {
                if (_solidPlatformsTilemap && _solidPlatformType != SolidPlatformType.None)
                {
                    AddSinglePlatform();
                }
            }

            
            #endregion
            
            GUILayout.Space(10);
            
            #region Chunk Customization Buttons Settings
            
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

                if (GUILayout.Button("Rebuild Whole Chunk"))
                {
                    RebuildCurrentChunk();
                }

                if (GUILayout.Button("Rebuild Background"))
                {
                    // Generate Background
                    if (_backgroundTilemap)
                    {
                        GenerateBackground(_backgroundTilemap);
                    }
                }

                if (GUILayout.Button("Rebuild Walls"))
                {
                    // Generate Walls
                    if (_wallsTilemap)
                    {
                        GenerateWalls(_wallsTilemap);
                    }
                }

                if (GUILayout.Button("Rebuild Basic Platforms"))
                {
                    // Generate Basic Platforms
                    if (_platformsTilemap)
                    {
                        if (_generatePlatforms)
                        {
                            GenerateBasicPlatforms(_platformsTilemap);
                        }
                        else
                        {
                            _platformsTilemap.ClearAllTiles();
                            GenerateFullWidthPlatform(_platformsTilemap);
                        }
                    }
                }

                if (GUILayout.Button("Rebuild Solid Platforms"))
                {
                    // Generate Solid Platforms
                    if (_solidPlatformsTilemap)
                    {
                        if (_generateSolidPlatforms)
                        {
                            GenerateSolidPlatforms(_solidPlatformsTilemap);
                        }
                        else
                        {
                            _solidPlatformsTilemap.ClearAllTiles();
                        }

                    }
                }
                
                if (GUILayout.Button("Rebuild Dart Traps"))
                {
                    // Generate base structure
                    if (!_dartTrapsGaps) return;
                    if (!_generateWalls) return;
                    if (!_wallsTilemap) return;
                    // First, remove existing dart traps
                    if (_currentChunkInstance)
                    {
                        var spawnPoint = _currentChunkInstance.transform.Find("SpawnPoints");
                        if (spawnPoint)
                        {
                            var leftDart = spawnPoint.Find("LeftWallDartTrap");
                            var rightDart = spawnPoint.Find("RightWallDartTrap");
                        
                            // Destroy all children of left dart trap parent
                            if (leftDart)
                            {
                                var children = new List<GameObject>();
                                foreach (Transform child in leftDart)
                                {
                                    children.Add(child.gameObject);
                                }
                                children.ForEach(child => DestroyImmediate(child));
                            }
                            // Destroy all children of right dart trap parent
                            if (rightDart)
                            {
                                var children = new List<GameObject>();
                                foreach (Transform child in rightDart)
                                {
                                    children.Add(child.gameObject);
                                }
                                children.ForEach(child => DestroyImmediate(child));
                            }
                        }
                    }
                    // Now proceed with generating new dart traps
                    GenerateWalls(_wallsTilemap);
                    GenerateDartTrapGaps(_wallsTilemap);
                    GenerateWallDecorations(_wallDecorTilemap);
                }
            }
            
            #endregion
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
            var chunkName = selectedType switch
            {
                ChunkGenerationType.Default => "GeneratedChunk",
                ChunkGenerationType.Easy => "EasyChunk",
                ChunkGenerationType.Medium => "MediumChunk",
                ChunkGenerationType.Hard => "HardChunk",
                _ => "GeneratedChunk"
            };
            
            _currentChunkInstance.name = chunkName;

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
            _backgroundTilemap = gridTransform.Find("Tilemap_Background")?.GetComponent<Tilemap>();
            if (_backgroundTilemap)
            {
                GenerateBackground(_backgroundTilemap);
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
                _wallDecorTilemap = gridTransform.Find("Tilemap_WallDeco")?.GetComponent<Tilemap>();
                if (_wallDecorTilemap)
                {
                    GenerateWallDecorations(_wallDecorTilemap);
                }
            }

            if (_generatePlatforms)
            {
                _platformsTilemap = gridTransform.Find("Tilemap_Collision_Platforms")?.GetComponent<Tilemap>();
                if (_platformsTilemap)
                {
                    GenerateBasicPlatforms(_platformsTilemap);
                }
            }
            
            if(!_platformsTilemap) _platformsTilemap = gridTransform.Find("Tilemap_Collision_Platforms")?.GetComponent<Tilemap>();
            GenerateFullWidthPlatform(_platformsTilemap);
            
            if (_generateSolidPlatforms)
            {
                _solidPlatformsTilemap = gridTransform.Find("Tilemap_Collision_SolidPlatforms")?.GetComponent<Tilemap>();
                if (_solidPlatformsTilemap)
                {
                    GenerateSolidPlatforms(_solidPlatformsTilemap);
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
            var spawnPoint = _currentChunkInstance.transform.Find("SpawnPoints");
            
            if (spawnPoint)
            {
                var leftDart = spawnPoint.Find("LeftWallDartTrap");
                var rightDart = spawnPoint.Find("RightWallDartTrap");
                        
                // Destroy all children of left dart trap parent
                if (leftDart)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in leftDart)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(child => DestroyImmediate(child));
                }
                // Destroy all children of right dart trap parent
                if (rightDart)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in rightDart)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(child => DestroyImmediate(child));
                }
            }
        }
        
        #endregion
        
        // TODO - Add a function to generate a chunk from a given seed
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
                        // Place the right wall decoration right before the first wall tile
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

        private void GenerateFullWidthPlatform(Tilemap platformsTilemap)
        {
            var platformRuleTile = Resources.Load<RuleTile>("Tiles/PlatformRuleTile");
            
            // Generate the initial full-width platform at the start
            for (var x = 4; x < _chunkWidth - 3; x++)
            {
                platformsTilemap.SetTile(new Vector3Int(x, 4, 0), platformRuleTile);
            }
        }
        private void GenerateBasicPlatforms(Tilemap platformsTilemap)
        {
            platformsTilemap.ClearAllTiles();

            GenerateFullWidthPlatform(_platformsTilemap);

            var platformRuleTile = Resources.Load<RuleTile>("Tiles/PlatformRuleTile");
            if (!platformRuleTile)
            {
                Debug.LogError("PlatformRuleTile not found. Ensure it is placed in the 'Resources/Tiles' folder.");
                return;
            }
            
            // Platform generation parameters
            const float minYDistance = 7f;
            const float maxYDistance = 8f;
            const int platformMinLength = 9;
            const int platformMaxLength = 14;
            const int maxAttempts = 100; // Prevent infinite loops
            
            var currentY = 10; // Starting Y position
            
            while (currentY < _chunkHeight - 4) // Leave space at the top
            {
                var attempts = 0;
                var platformPlaced = false;
                
                while (!platformPlaced && attempts < maxAttempts)
                {
                    // Randomize platform length
                    var platformLength = Random.Range(platformMinLength, platformMaxLength);
                    
                    // Try to find a valid X position
                    var startX = Random.Range(4, _chunkWidth - platformLength - 2);
                    
                    // Check if we can place a platform here
                    if (IsSufficientSpaceForBasicPlatform(startX, currentY, platformsTilemap, platformLength))
                    {
                        // Place the platform
                        for (var x = 0; x < platformLength; x++)
                        {
                            platformsTilemap.SetTile(new Vector3Int(startX + x, currentY, 0), platformRuleTile);
                        }

                        platformPlaced = true;
                    }
                    
                    attempts++;
                }
                
                if (!platformPlaced)
                {
                    Debug.LogWarning($"Could not place platform at Y: {currentY} after {maxAttempts} attempts");
                }
                
                // Calculate next Y position
                var yIncrease = Random.Range(minYDistance, maxYDistance);
                currentY += Mathf.RoundToInt(yIncrease);
            }
        }


        private bool IsSufficientSpaceForBasicPlatform(int startX, int startY, Tilemap basicPlatformTilemap, int platformLength)
        {
            // Check the area around the platform (including some padding)
            const int verticalPadding = 2; // Space to check above and below
    
            // Check the entire area where the platform might be placed
            for (var x = -1; x <= platformLength; x++) // Check one tile before and after
            {
                for (var y = -verticalPadding; y <= verticalPadding; y++)
                {
                    var positionCheck = new Vector3Int(startX + x, startY + y, 0);
            
                    // Check if the position is within chunk bounds
                    if (startX + x < 0 || startX + x >= _chunkWidth || 
                        startY + y < 0 || startY + y >= _chunkHeight)
                    {
                        return false;
                    }
    
                    // Check if a position already has any tiles
                    if (_wallsTilemap && _wallsTilemap.HasTile(positionCheck))
                        return false;
        
                    if (_solidPlatformsTilemap && _solidPlatformsTilemap.HasTile(positionCheck))
                        return false;
        
                    if (basicPlatformTilemap && basicPlatformTilemap.HasTile(positionCheck))
                        return false;
                }
            }
            return true;
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

            // Try to find a valid position for the platform
            const int maxAttempts = 50; // Prevent infinite loops
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                // Generate random position within chunk bounds
                var startX = Random.Range(4, _chunkWidth - 3);
                var startY = Random.Range(7, _chunkHeight - 10);

                // Check if we can place the platform here
                if (IsSufficientSpaceForPlatform(startX, startY, solidPlatformsTilemap))
                {
                    switch (_solidPlatformType)
                    {
                        case SolidPlatformType.Cube:
                            GenerateCube(solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                            return; // Cube platform generated successfully
                        
                        case SolidPlatformType.UpsideDownT:
                            GenerateShapeT(solidPlatformsTilemap, solidPlatformRuleTile, startX, startY, true);
                            return;
                        case SolidPlatformType.ShapeT:
                            GenerateShapeT(solidPlatformsTilemap, solidPlatformRuleTile, startX, startY, false);
                            return;
                        case SolidPlatformType.None:
                            return; // No platform generated
                        default:
                            Debug.LogError("Invalid solid platform type selected.");
                            return;
                    }
                }

                attempts++;
            }
            
            Debug.LogWarning("Could not find suitable position for solid platform after " + maxAttempts + " attempts");
        }
        
        // Solid platform types
        
        // Generate Cube Shapes Solid Platform
        private static void GenerateCube(Tilemap tilemap, RuleTile tile, int startX, int startY)
        {
            var sizeX = Random.Range(10, 15); // Random cube size
            var sizeY = Random.Range(7, 9); // Random cube size
            
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), tile);
                }
            }
        }

        private static void GenerateShapeT(Tilemap tilemap, RuleTile tile, int startX, int startY, bool isUpsideDown)
        {
            // Generate top bar width first
            var topBarWidth = Random.Range(12, 16); // Random top bar width
            var topBarHeight = Random.Range(2, 3);  // Random top bar height

            // Determine stem width based on the top bar width parity
            const int minStemWidth = 3;
            const int maxStemWidth = 5;
    
            // Ensure stem width has the same parity as top bar width
            var stemWidth = Random.Range(minStemWidth, maxStemWidth);
            if (topBarWidth % 2 != stemWidth % 2)
            {
                // If parities don't match, adjust stem width by 1
                stemWidth += 1;
                // Make sure we don't exceed our max range
                if (stemWidth > maxStemWidth)
                    stemWidth -= 2;
            }
    
            var stemHeight = Random.Range(7, 8); // Random stem height

            
            // Calculate center alignment
            var stemStartX = startX + (topBarWidth - stemWidth) / 2;
            
            // Generate the top horizontal bar
            for (var x = 0; x < topBarWidth; x++)
            {
                for (var y = 0; y < topBarHeight; y++)
                {
                    tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), tile);
                }
            }
            
            // Generate the vertical stem
            for (var x = 0; x < stemWidth; x++)
            {
                for (var y = 0; y < stemHeight; y++)
                {
                    var yPos = isUpsideDown ? startY + y : startY - stemHeight + y;
                    tilemap.SetTile(new Vector3Int(stemStartX + x, yPos, 0), tile);
                }
            }
        }
        
        // End of Section for Solid Platforms types

        private void AddSinglePlatform()
        {
            var solidPlatformRuleTile = Resources.Load<RuleTile>("Tiles/SolidPlatformsRuleTile");
            if (!solidPlatformRuleTile)
            {
                Debug.LogError("SolidPlatformRuleTile not found. Ensure it is placed in the 'Resources/Tiles' folder.");
                return;
            }

            // Calculate safe boundaries based on a platform type
            int minX, maxX, minY, maxY;
            switch (_solidPlatformType)
            {
                case SolidPlatformType.Cube:
                    minX = 4;
                    maxX = _chunkWidth - 12; // 8 for cube width + 4 padding
                    minY = 4;
                    maxY = _chunkHeight - 12;
                    break;
                
                case SolidPlatformType.ShapeT:
                case SolidPlatformType.UpsideDownT:
                    minX = 4;
                    maxX = _chunkWidth - 20; // 16 for max width + 4 padding
                    minY = 12; // Ensure enough space for the full T shape
                    maxY = _chunkHeight - 12;
                    break;
                    
                default:
                    Debug.LogError("Invalid platform type");
                    return;
            }

            const int maxAttempts = 50;
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                var startX = Random.Range(minX, maxX);
                var startY = Random.Range(minY, maxY);

                if (IsSufficientSpaceForPlatform(startX, startY, _solidPlatformsTilemap))
                {
                    switch (_solidPlatformType)
                    {
                        case SolidPlatformType.Cube:
                            GenerateCube(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                            return;
                        
                        case SolidPlatformType.UpsideDownT:
                            GenerateShapeT(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY, true);
                            return;
                        
                        case SolidPlatformType.ShapeT:
                            GenerateShapeT(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY, false);
                            return;
                    }
                }
                
                attempts++;
            }
            
            Debug.LogWarning("Could not find suitable space for new platform after " + maxAttempts + " attempts");
        }

        private bool IsSufficientSpaceForPlatform(int startX, int startY, Tilemap solidPlatformsTilemap)
        {
            // Define dimensions based on a platform type
            int width, height;
            switch (_solidPlatformType)
            {
                case SolidPlatformType.Cube:
                    width = 15;  // Your cube width
                    height = 9; // Your cube height
                    break;
                    
                case SolidPlatformType.ShapeT:
                case SolidPlatformType.UpsideDownT:
                    width = 16;  // Maximum top bar width
                    height = 10; // Maximum total height (top bar and stem)
                    break;
                    
                default:
                    return false;
            }

            // Add some padding to prevent merging
            const int padding = 1;
            
            // Adjust check area based on a shape type
            var checkStartY = startY;
            var checkHeight = height;
            
            // For regular T shape, we need to check below the start position
            if (_solidPlatformType == SolidPlatformType.ShapeT)
            {
                checkStartY = startY - height + 2; // +2 for the top bar height
                checkHeight = height + 1; // Add 1 for better separation
            }

            // Check the entire area where the platform might be placed, including padding
            for (var x = -padding; x < width + padding; x++)
            {
                for (var y = -padding; y < checkHeight + padding; y++)
                {
                    var positionCheck = new Vector3Int(startX + x, checkStartY + y, 0);
                    
                    // Check if the position is within valid bounds
                    if (positionCheck.x < 0 || positionCheck.x >= _chunkWidth ||
                        positionCheck.y < 0 || positionCheck.y >= _chunkHeight)
                    {
                        return false;
                    }

                    // Check if a position already has any tiles
                    if (_wallsTilemap && _wallsTilemap.HasTile(positionCheck))
                        return false;
                    
                    if (_platformsTilemap && _platformsTilemap.HasTile(positionCheck))
                        return false;
                    
                    if (solidPlatformsTilemap && solidPlatformsTilemap.HasTile(positionCheck))
                        return false;
                }
            }
            return true;
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

            Debug.Log("Background Generated with random removals.");
        }

        #endregion

        #region DartTrap Generation

        private void GenerateDartTrapGaps(Tilemap wallsTilemap)
        {
            var spawnPoint = _currentChunkInstance.transform.Find("SpawnPoints");
            if (spawnPoint)
            {
                var leftDart = spawnPoint.Find("LeftWallDartTrap");
                var rightDart = spawnPoint.Find("RightWallDartTrap");
                        
                // Destroy all children of left dart trap parent
                if (leftDart)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in leftDart)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(child => DestroyImmediate(child));
                }
                // Destroy all children of right dart trap parent
                if (rightDart)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in rightDart)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(child => DestroyImmediate(child));
                }
            }

            
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
                var leftDartTrap = spawnPoint.Find("LeftWallDartTrap");
                var rightDartTrap = spawnPoint.Find("RightWallDartTrap");
                
                if (_dartsOnTheLeftWall)
                {
                    InstantiateDartTraps(leftWallGapPositions, true, leftDartTrap);
                }
                    
                if (_dartsOnTheRightWall)
                {
                    InstantiateDartTraps(rightWallGapPositions, false, rightDartTrap);
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

        #endregion
       
    }
}