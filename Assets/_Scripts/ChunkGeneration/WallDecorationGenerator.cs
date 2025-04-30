using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Wall Decoration Generator")]
    public class WallDecorationGenerator: ScriptableObject, IChunkGenerator
    {
        [SerializeField] private TileBase wallDecorTileLeft;
        [SerializeField] private TileBase wallDecorTileRight;
        
        [SerializeField] private int torchSpacing;
        [SerializeField] private int torchLeftWallCount;
        [SerializeField] private int torchRightWallCount;        
        [SerializeField] private GameObject leftTorchPrefab;
        [SerializeField] private GameObject rightTorchPrefab;
        
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _wallDecorTilemap;
        private Tilemap _wallTilemap;
        private GameObject _chunkInstance;
        
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            _wallDecorTilemap = tilemap;
        }

        public void Generate()
        {
            GenerateWallDecorations(_wallDecorTilemap);
            GenerateTorches();
        }
        
        public void GetTilemapsByType(Tilemap wallTilemap)
        {
            _wallTilemap = wallTilemap;
        }

        public void GetChunkInstance(GameObject chunkInstance)
        {
            _chunkInstance = chunkInstance;
        }
        
        private void GenerateWallDecorations(Tilemap wallDecor)
        {
            wallDecor.ClearAllTiles();
            
            if (!wallDecorTileLeft || !wallDecorTileRight)
            {
                Debug.LogError("Failed to load wall decoration tiles. Make sure the tiles are assigned.");
                return;
            }
            
            // Place wall decorations next to the walls
            for (var y = 0; y < _chunkHeight; y++)
            {
                // Check each column of the left wall (0 to 3)
                var leftWallFound = false;
                for (var x = 3; x >= 0; x--)
                {
                    if (_wallTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        // Place left wall decoration right after the last wall tile
                        wallDecor.SetTile(new Vector3Int(x + 1, y, 0), wallDecorTileLeft);
                        leftWallFound = true;
                        break;
                    }
                }
                
                // If no wall was found (complete gap), place decoration at the default position
                if (!leftWallFound)
                {
                    wallDecor.SetTile(new Vector3Int(4, y, 0), wallDecorTileLeft);
                }
                
                var rightWallFound = false;
                for (var x = _chunkWidth - 4; x <= _chunkWidth; x++)
                {
                    if (_wallTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        // Place the right wall decoration right before the first wall tile
                        wallDecor.SetTile(new Vector3Int(x - 1, y, 0), wallDecorTileRight);
                        rightWallFound = true;
                        break;
                    }
                }
                
                // If no wall was found (complete gap), place decoration at the default position
                if (!rightWallFound)
                {
                    wallDecor.SetTile(new Vector3Int(_chunkWidth - 5, y, 0), wallDecorTileRight);
                }
            }
            Debug.Log("Wall Decorations Generated");
        }
        
        private void GenerateTorches()
        {
            if (!leftTorchPrefab || !rightTorchPrefab)
            {
                Debug.LogError("Prefabs are not assigned. Make sure they are assigned before generating torches.!");
                return;
            }

            var random = new System.Random(); // For random offsets
            const int torchOffsetRange = 10;        // Max random offset for Y position (+/-)
            var leftSidePositions = GetValidWallPositions( true);    // Get valid left wall positions
            var rightSidePositions = GetValidWallPositions(false); // Get valid right wall positions

            var spawnPoint = _chunkInstance;
            var torchSpawn = spawnPoint.transform.Find("AnimatedDecorations");
            
            if (torchSpawn)
            {
                var children = new List<GameObject>();
                foreach (Transform child in torchSpawn.transform)
                {
                    children.Add(child.gameObject);
                }
                children.ForEach(DestroyImmediate);   
            }


            var leftTorchesPlaced = 0;
            for (var i = 0; i < leftSidePositions.Count; i += torchSpacing) // Skip by torchSpacing
            {
                if (leftTorchesPlaced >= torchLeftWallCount) break;

                var baseY = leftSidePositions[i];
                var randomY = GetValidRandomY(baseY);
                var position = new Vector3(4 - 19.5f, randomY - 5, 0); // Instantiate at a calculated random position
                Instantiate(leftTorchPrefab, position, Quaternion.identity, torchSpawn);
                leftTorchesPlaced++;
            }

            // Place torches on the right wall
            var rightTorchesPlaced = 0;
            for (var i = 0; i < rightSidePositions.Count; i += torchSpacing) // Skip by torchSpacing
            {
                if (rightTorchesPlaced >= torchRightWallCount) break;

                var baseY = rightSidePositions[i];
                var randomY = GetValidRandomY(baseY);
                var position = new Vector3(_chunkWidth - 4 - 20.5f, randomY - 5, 0); // Instantiate at a calculated random position
                Instantiate(rightTorchPrefab, position, Quaternion.identity, torchSpawn);
                rightTorchesPlaced++;
            }

            return;

            // Helper function to ensure Y position stays within the valid range
            int GetValidRandomY(int baseY)
            {
                var randomOffset = random.Next(-torchOffsetRange, torchOffsetRange + 1);
                var newY = baseY + randomOffset;
                return Mathf.Clamp(newY, 10, _chunkHeight - 10); // Clamp to ensure Y stays within bounds
            }
        }
        
        private List<int> GetValidWallPositions(bool isLeft)
        {
            var validPositions = new List<int>();
            const int safetySpacing = 3; // How many additional tiles to check above and below

            // Start from y = 10 and end 10 tiles before chunk height
            for (var y = 10; y < _chunkHeight - 10; y++)
            {
                var isValidSpawnPoint = true;
                var consecutiveValidRows = 0;

                // Check more rows above and below to ensure we're not near any gaps
                for (var yOffset = -safetySpacing; yOffset <= safetySpacing; yOffset++)
                {
                    var checkY = y + yOffset;
                    var currentRowValid = true;

                    // Check all 4 tiles in the wall for this row
                    for (var x = 0; x < 4; x++)
                    {
                        var xToCheck = isLeft ? x : (_chunkWidth - 4 + x);
                
                        if (!_wallTilemap.HasTile(new Vector3Int(xToCheck, checkY, 0)))
                        {
                            currentRowValid = false;
                            break;
                        }
                    }

                    if (currentRowValid)
                    {
                        consecutiveValidRows++;
                    }
                    else
                    {
                        isValidSpawnPoint = false;
                        break;
                    }
                }

                // Only add position if we have enough consecutive valid rows
                if (isValidSpawnPoint && consecutiveValidRows == (2 * safetySpacing + 1))
                {
                    validPositions.Add(y);
                }
            }

            return validPositions;
        }

    }
}