using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration
{
    public interface IChunkGenerator
    {
        void Setup(int chunkWidth, int chunkHeight, Tilemap tilemap);
        void Generate();
    }
}