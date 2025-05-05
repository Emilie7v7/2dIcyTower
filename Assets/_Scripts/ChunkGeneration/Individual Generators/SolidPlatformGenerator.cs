using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    public enum SolidPlatformType {None, Cube, SmallCube, UpsideDownT, ShapeT}

    [CreateAssetMenu(menuName = "Chunk Generation/Solid Platform Generator")]
    public class SolidPlatformGenerator : ScriptableObject, IChunkGenerator
    {
        [SerializeField] private RuleTile solidPlatformRuleTile;
        [SerializeField] private SolidPlatformType solidPlatformType = SolidPlatformType.None;
        [SerializeField] private int amountOfPlatforms = 1;
        [SerializeField] private float platformSpacing = 1f;
        [SerializeField] private bool placeIndividualType;
            
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _solidPlatformsTilemap;
        private Tilemap _wallsTilemap;
        private Tilemap _platformsTilemap;
            
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            _solidPlatformsTilemap = tilemap;
        }

        public void Generate()
        {
            GenerateSolidPlatforms();
        }

        public void GetTilemapsByType(Tilemap wallsTilemap, Tilemap platformsTilemap)
        {
            _wallsTilemap = wallsTilemap;
            _platformsTilemap = platformsTilemap;
        }
        
        private void GenerateSolidPlatforms()
        {
            _solidPlatformsTilemap.ClearAllTiles();

            if (!solidPlatformRuleTile)
            {
                Debug.LogError("SolidPlatformRuleTile not found. Ensure it is assigned in the inspector.");
                return;
            }

            for (int i = 0; i < amountOfPlatforms; i++)
            {
                // Randomly select a platform type (excluding None)
                var randomType = (SolidPlatformType)Random.Range(1, 5); // 1 to 4 corresponds to Cube, Small Cube, UpsideDownT, ShapeT
                solidPlatformType = randomType;

                const int maxAttempts = 50;
                var attempts = 0;
                var platformPlaced = false;

                while (attempts < maxAttempts && !platformPlaced)
                {
                    // Calculate safe boundaries based on a platform type
                    int minX, maxX, minY, maxY;
                    switch (solidPlatformType)
                    {
                        case SolidPlatformType.Cube:
                            minX = 4;
                            maxX = _chunkWidth - 12;
                            minY = 4;
                            maxY = _chunkHeight - 12;
                            break;
                        case SolidPlatformType.SmallCube:
                            minX = 4;
                            maxX = _chunkWidth - 8;
                            minY = 4;
                            maxY = _chunkHeight - 8;
                            break;
                        
                        case SolidPlatformType.ShapeT:
                        case SolidPlatformType.UpsideDownT:
                            minX = 4;
                            maxX = _chunkWidth - 20;
                            minY = 12;
                            maxY = _chunkHeight - 12;
                            break;
                            
                        default:
                            continue;
                    }

                    var startX = Random.Range(minX, maxX);
                    var startY = Random.Range(minY, maxY);

                    if (IsSufficientSpaceForPlatform(startX, startY, _solidPlatformsTilemap))
                    {
                        switch (solidPlatformType)
                        {
                            case SolidPlatformType.Cube:
                                GenerateCube(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                                platformPlaced = true;
                                break;
                            
                            case SolidPlatformType.SmallCube:
                                GenerateSmallCube(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                                platformPlaced = true;
                                break;
                            
                            case SolidPlatformType.UpsideDownT:
                                GenerateShapeT(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY, true);
                                platformPlaced = true;
                                break;
                            
                            case SolidPlatformType.ShapeT:
                                GenerateShapeT(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY, false);
                                platformPlaced = true;
                                break;
                        }
                    }
                    
                    attempts++;
                }

                if (!platformPlaced)
                {
                    Debug.LogWarning($"Could not place platform {i + 1} after {maxAttempts} attempts");
                }
            }
        }

        
        // ##### Section for Solid Platforms types #####
        
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
        private static void GenerateSmallCube(Tilemap tilemap, RuleTile tile, int startX, int startY)
        {
            var sizeX = Random.Range(3, 4); // Random cube size
            var sizeY = Random.Range(3, 4); // Random cube size
            
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    tilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), tile);
                }
            }
        }
        
        // Generate Upside Down T Shape Solid Platform
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
        
        // Can add more types in the future
        
        // End of Section for Solid Platforms types
        
        public void AddSinglePlatform()
        {
            if (!solidPlatformRuleTile)
            {
                Debug.LogError("SolidPlatformRuleTile not found. Ensure it is assigned in the inspector.");
                return;
            }
        
            // Calculate safe boundaries based on a platform type
            int minX, maxX, minY, maxY;
            switch (solidPlatformType)
            {
                case SolidPlatformType.Cube:
                    minX = 4;
                    maxX = _chunkWidth - 12; // 8 for cube width + 4 paddings
                    minY = 4;
                    maxY = _chunkHeight - 12;
                    break;
                case SolidPlatformType.SmallCube:
                    minX = 4;
                    maxX = _chunkWidth - 8; // 4 for cube width + 4 paddings
                    minY = 4;
                    maxY = _chunkHeight - 8;
                    break;
                
                case SolidPlatformType.ShapeT:
                case SolidPlatformType.UpsideDownT:
                    minX = 4;
                    maxX = _chunkWidth - 20; // 16 for max width + 4 paddings
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
                    switch (solidPlatformType)
                    {
                        case SolidPlatformType.Cube:
                            GenerateCube(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
                            return;
                        case SolidPlatformType.SmallCube:
                            GenerateSmallCube(_solidPlatformsTilemap, solidPlatformRuleTile, startX, startY);
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
            switch (solidPlatformType)
            {
                case SolidPlatformType.Cube:
                    width = 15;
                    height = 9;
                    break;
                case SolidPlatformType.SmallCube:
                    width = 4;
                    height = 4;
                    break;
                case SolidPlatformType.ShapeT:
                case SolidPlatformType.UpsideDownT:
                    width = 16;
                    height = 10;
                    break;

                case SolidPlatformType.None:
                default:
                    return false;
            }

            // Use platformSpacing as padding to ensure minimum distance between platforms
            var padding = Mathf.CeilToInt(platformSpacing);
    
            // Adjust the check area based on a shape type
            var checkStartY = startY;
            var checkHeight = height;
    
            if (solidPlatformType == SolidPlatformType.ShapeT)
            {
                checkStartY = startY - height + 2;
                checkHeight = height + 1;
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

    }
}