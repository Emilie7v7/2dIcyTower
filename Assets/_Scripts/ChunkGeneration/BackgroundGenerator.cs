using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Background Generator")]
    public class BackgroundGenerator : ScriptableObject, IChunkGenerator
    {
        [SerializeField] private RuleTile backgroundRuleTile;
        [SerializeField] private float noise = 1f;
        [SerializeField] private bool useNoise;
        [SerializeField] private float noiseScale = 10f;
        [SerializeField] private float threshold = 0.4f;
        
        [SerializeField] private TileBase[] decorationTiles; // Array of 8 different 3x3 decorations
        [SerializeField] private Tilemap decorTilemap; // Reference to decoration tilemap
        [SerializeField, Range(0f, 1f)] private float decorationChance = 0.1f; // Chance to spawn decoration
        
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap backgroundTilemap;
        private Tilemap decorationsTilemap1;
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            backgroundTilemap = tilemap;
        }

        public void GetDecorationsTilemap1(Tilemap decorTilemapReference)
        {
            decorationsTilemap1 = decorTilemapReference;
        }

        public void Generate()
        {
            if (!backgroundTilemap)
            {
                Debug.LogError("Background Tilemap is not set!");
                return;
            }

            backgroundTilemap.ClearAllTiles();
            if (decorationsTilemap1)
                decorationsTilemap1.ClearAllTiles();

            
            GenerateBackground();
            GenerateDecorations();
        }

        private void GenerateBackground()
        {
            if (!backgroundTilemap || !backgroundRuleTile)
            {
                Debug.LogError("Background Tilemap or Rule Tile is not set!");
                return;
            }
            
            // Random offset for noise to create variety between chunks
            var offsetX = Random.Range(0f, 1000f);
            var offsetY = Random.Range(0f, 1000f);

            // Populate the background with noise-based gaps
            for (var y = 0; y < _chunkHeight; y++)
            {
                for (var x = 0; x < _chunkWidth; x++)
                {
                    if (!useNoise)
                    {
                        // If noise is disabled, place the tile
                        backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundRuleTile);
                        continue;
                    }

                    // Generate Perlin noise value for this position
                    var noiseValue = Mathf.PerlinNoise(
                        (x + offsetX) / noiseScale, 
                        (y + offsetY) / noiseScale
                    ) * noise; // Multiply by noise factor to adjust intensity

                    // Apply the noise threshold
                    if (noiseValue > threshold)
                    {
                        backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundRuleTile);
                    }
                }
            }
        }

        private void GenerateDecorations()
        {
            if (decorationTiles == null || decorationTiles.Length == 0)
            {
                Debug.LogWarning("No decoration tiles assigned!");
                return;
            }

            // Create a list of all possible positions
            var possiblePositions = new List<Vector2Int>();
            
            // Check every Nth position to reduce density
            const int step = 2; // Increase this value to reduce density
            for (var y = 2; y < _chunkHeight - 2; y += step)
            {
                for (var x = 2; x < _chunkWidth - 2; x += step)
                {
                    if (backgroundTilemap.GetTile(new Vector3Int(x, y, 0)))
                    {
                        // Only add position if a random check passes
                        if (Random.value < decorationChance)
                        {
                            possiblePositions.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }

            // Shuffle the positions
            var n = possiblePositions.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                (possiblePositions[k], possiblePositions[n]) = (possiblePositions[n], possiblePositions[k]);
            }

            // Try to place decorations at the selected positions
            foreach (var pos in possiblePositions)
            {
                if (CanPlaceDecoration(pos.x, pos.y))
                {
                    var decorTile = decorationTiles[Random.Range(0, decorationTiles.Length)];
                    PlaceDecoration(pos.x, pos.y, decorTile);
                }
            }
        }

        private bool CanPlaceDecoration(int centerX, int centerY)
        {
            // First, check if there's already a decoration here
            if (decorationsTilemap1.GetTile(new Vector3Int(centerX, centerY, 0)))
                return false;

            // Check if we have a complete 3x3 area of background tiles
            for (var y = centerY - 1; y <= centerY + 1; y++)
            {
                for (var x = centerX - 1; x <= centerX + 1; x++)
                {
                    // Skip if outside bounds
                    if (x < 0 || x >= _chunkWidth || y < 0 || y >= _chunkHeight)
                        return false;

                    // If any background tile is missing in the 3x3 area, return false
                    if (!backgroundTilemap.GetTile(new Vector3Int(x, y, 0)))
                        return false;
                }
            }

            // Check surrounding tiles in a radius to prevent clustering with other decorations
            int checkRadius = 4;
            for (var y = centerY - checkRadius; y <= centerY + checkRadius; y++)
            {
                for (var x = centerX - checkRadius; x <= centerX + checkRadius; x++)
                {
                    // Skip if outside bounds
                    if (x < 0 || x >= _chunkWidth || y < 0 || y >= _chunkHeight)
                        continue;

                    // If we find any decoration within this radius, return false
                    if (decorationsTilemap1.GetTile(new Vector3Int(x, y, 0)))
                        return false;
                }
            }

            return true;
        }
        
        private void PlaceDecoration(int centerX, int centerY, TileBase decorTile)
        {
            // Place only a single decoration tile
            decorationsTilemap1.SetTile(new Vector3Int(centerX, centerY, 0), decorTile);
        }


    }
}