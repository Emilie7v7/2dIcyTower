using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Decoration Spawn Data/Base Data", fileName = "newDecorationSpawnData")]
    public class DecorationSpawnSettingsSo : ScriptableObject
    {
        [Header("Platform Decorations")]
        [Tooltip("RuleTiles that must be placed on platforms (e.g., fences, vines).")]
        public RuleTile[] platformRuleTileDecorations;

        [Tooltip("TileBase decorations that must be placed on platforms (e.g., torches, rocks).")]
        public TileBase[] platformSingleTileDecorations;

        [Header("Air Decorations")]
        [Tooltip("RuleTiles that can be placed in the air (e.g., floating lanterns, cloud formations).")]
        public RuleTile[] airRuleTileDecorations;

        [Tooltip("TileBase decorations that can be placed in the air (e.g., floating debris, particles).")]
        public TileBase[] airSingleTileDecorations;

        [Header("Spawn Settings")]
        public Vector2 decorationHeightRange = new Vector2(0, 10000); // Where decorations spawn
        public int minDecorationsPerChunk = 5; // Max decorations per chunk
        public int maxDecorationsPerChunk = 20; // Max decorations per chunk
    }
}
