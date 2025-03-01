using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Decoration Spawn Data/Base Data", fileName = "newDecorationSpawnData")]
    public class DecorationSpawnSettingsSo : ScriptableObject
    {
        [Header("Decoration Settings")]
        public TileBase[] decorationTiles;
        public float spawnProbability = 0.7f; // Default 70% chance
        public Vector2 decorationHeightRange = new Vector2(0, 10000); // Where decorations spawn
        public int maxDecorationsPerChunk = 5; // Limit per chunk
    }
}
