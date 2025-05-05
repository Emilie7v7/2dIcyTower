using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Chunk Generation/Walls Generator")]
    public class WallsGenerator : ScriptableObject, IChunkGenerator
    {
        [SerializeField] private RuleTile wallRuleTile;
        [SerializeField] private TileBase wallDecorTileLeft;
        [SerializeField] private TileBase wallDecorTileRight;
        
        public int wallThickness = 4;

        private int _chunkWidth;
        private int _chunkHeight;
        private Tilemap wallsTilemap;

        public void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap)
        {
            _chunkWidth = chunkWidth;
            _chunkHeight = chunkHeight;
            wallsTilemap = tilemap;
        }
        
        public void Generate()
        {
            GenerateWalls(wallsTilemap);
        }
        
        
        // TODO - Add a function to generate a chunk from a given seed
        private void GenerateWalls(Tilemap walls)
        {
            if (!walls || !wallRuleTile)
            {
                Debug.LogError("Walls Tilemap or Rule Tile is not set!");
                return;
            }

            walls.ClearAllTiles();

            for (var y = 0; y < _chunkHeight; y++)
            {
                for (var x = 0; x < wallThickness; x++)
                {
                    //Left wall
                    walls.SetTile(new Vector3Int(0 + x, y, 0), wallRuleTile);
                    //Right wall
                    walls.SetTile(new Vector3Int(_chunkWidth - 1 - x, y, 0), wallRuleTile);
                }
            }

            Debug.Log("Walls generated successfully.");
        }
        
        
    }
}