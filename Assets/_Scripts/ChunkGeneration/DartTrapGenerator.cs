using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/DartTrap Generator")]
    public class DartTrapGenerator: ScriptableObject, IChunkGenerator
    {
        [SerializeField] private bool dartTrapsGaps;
        [SerializeField] private bool dartsOnTheLeftWall;
        [SerializeField] private bool dartsOnTheRightWall;
        [SerializeField] private bool fillDartTrapGaps;
        [SerializeField] private GameObject dartTrapPrefabLeftWall;
        [SerializeField] private GameObject dartTrapPrefabRightWall;
        [SerializeField] private int amountOfGapsOnLeftWall;
        [SerializeField] private int amountOfGapsOnRightWall;

        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _wallsTilemap;
        private GameObject _chunkInstance;
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            _wallsTilemap = tilemap;
        }

        
        public void Generate()
        {
            GenerateDartTraps(_wallsTilemap);
        }
        
        public void GetChunkInstance(GameObject chunkInstance)
        {
            _chunkInstance = chunkInstance;
        }
        
        #region DartTrap Generation
        
        private void GenerateDartTraps(Tilemap wallsTilemap)
        {
            var spawnPoint = _chunkInstance;
            var left = spawnPoint.transform.Find("SpawnPoints");
            var right = spawnPoint.transform.Find("SpawnPoints");
            
            if (spawnPoint)
            {
                
                var leftDart = left.transform.Find("LeftWallDartTrap");
                var rightDart = right.transform.Find("RightWallDartTrap");
                        
                // Destroy all children of left dart trap parent
                if (leftDart)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in leftDart.transform)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(DestroyImmediate);
                }
                // Destroy all children of right dart trap parent
                if (rightDart)
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in rightDart.transform)
                    {
                        children.Add(child.gameObject);
                    }
                    children.ForEach(DestroyImmediate);
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
            if (dartsOnTheLeftWall)
            {
                possibleHeights = possibleHeights.OrderBy(_ => Random.value).ToList();
                GenerateWallGaps(wallsTilemap, true, amountOfGapsOnLeftWall, possibleHeights, leftWallGapPositions);
            }
            
            // Generate gaps for the right wall
            if (dartsOnTheRightWall)
            {
                possibleHeights = possibleHeights.OrderBy(_ => Random.value).ToList();
                GenerateWallGaps(wallsTilemap, false, amountOfGapsOnRightWall, possibleHeights, rightWallGapPositions);
            }
            
            if (dartTrapsGaps)
            {
                var leftDartTrap = left.transform.Find("LeftWallDartTrap");
                var rightDartTrap = right.transform.Find("RightWallDartTrap");
                
                if (dartsOnTheLeftWall)
                {
                    InstantiateDartTraps(leftWallGapPositions, true, leftDartTrap?.transform);
                }
                    
                if (dartsOnTheRightWall)
                {
                    InstantiateDartTraps(rightWallGapPositions, false, rightDartTrap?.transform);
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
                        wallsTilemap.SetTile(new Vector3Int(_chunkWidth - 4, currentHeight, 0), null);
                        wallsTilemap.SetTile(new Vector3Int(_chunkWidth - 4, currentHeight + 1, 0), null);
                    }
                }
                currentIndex++;
            }
        }
        
        private void InstantiateDartTraps(List<int> gapPositions, bool isLeftWall, Transform parentTransform)
        {
            foreach (var gapPosition in gapPositions)
            {
                var xPosition = isLeftWall ? 3.25f : _chunkWidth - 3.25f;
        
                var trapPosition = new Vector3(xPosition - 20f, gapPosition - 4f, 0);
        
                var trapRotation = isLeftWall ? 
                    Quaternion.Euler(0, 0, 0) :    // Left wall traps face right
                    Quaternion.Euler(0, 180, 0);      // Right wall traps face left
        
                if (fillDartTrapGaps)
                {
                    Instantiate(isLeftWall ? dartTrapPrefabLeftWall : dartTrapPrefabRightWall, trapPosition,
                        trapRotation, parentTransform);
                }
            }
        }
        
        #endregion
    }
}