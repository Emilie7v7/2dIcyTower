using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Wooden Platform Generator")]
    public class WoodenPlatformsGenerator : ScriptableObject, IChunkGenerator
    {
        [Header("Tiles")]
        [SerializeField] private RuleTile platformRuleTile;
        
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _platformsTilemap;
        private Tilemap _wallsTilemap;
        private Tilemap _solidPlatformsTilemap;
        
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            _platformsTilemap = tilemap;
        }
        
        public void Generate()
        {
            GenerateBasicPlatforms(_platformsTilemap);
        }
        
        public void GenerateFullWidthPlatform(Tilemap platformsTilemap)
        {
            // Generate the initial full-width platform at the start
            for (var x = 4; x < _chunkWidth - 3; x++)
            {
                platformsTilemap.SetTile(new Vector3Int(x, 4, 0), platformRuleTile);
            }
        }
        public void GetWallsTilemap(Tilemap wallsTilemap)
        {
            _wallsTilemap = wallsTilemap;
        }
        public void GetSolidPlatformsTilemap(Tilemap solidPlatformsTilemap)
        {
            _solidPlatformsTilemap = solidPlatformsTilemap;
        }
        
        #region Basic Platforms Generation
        
        private void GenerateBasicPlatforms(Tilemap platformsTilemap)
        {
            platformsTilemap.ClearAllTiles();
        
            GenerateFullWidthPlatform(_platformsTilemap);
        
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
        
        private bool IsSufficientSpaceForBasicPlatform(int startX, int startY, Tilemap woodenPlatformTilemap, int platformLength)
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
        
                    if (woodenPlatformTilemap && woodenPlatformTilemap.HasTile(positionCheck))
                        return false;
                }
            }
            return true;
        }
        
        #endregion
    }
}