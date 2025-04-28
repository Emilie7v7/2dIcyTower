using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Wall Decoration Generator")]
    public class WallDecorationGenerator: ScriptableObject, IChunkGenerator
    {
        [SerializeField] private TileBase wallDecorTileLeft;
        [SerializeField] private TileBase wallDecorTileRight;
        
        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap _wallDecorTilemap;
        private Tilemap _wallTilemap;
        
        
        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            _wallDecorTilemap = tilemap;
        }

        public void Generate()
        {
            GenerateWallDecorations(_wallDecorTilemap);
        }
        
        public void GetWallTilemap(Tilemap wallTilemap)
        {
            _wallTilemap = wallTilemap;
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
    }
}