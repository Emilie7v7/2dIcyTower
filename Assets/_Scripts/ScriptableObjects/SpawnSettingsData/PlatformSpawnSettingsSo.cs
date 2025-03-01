using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Platform Spawn Data/Base Data", fileName = "newPlatformSpawnData")]
    public class PlatformSpawnSettingsSo : ScriptableObject
    {
        [Header("Platform Settings")]
        public TileBase[] platformTiles; // Different tile types
        public float minWidth = 2f;
        public float maxWidth = 5f;
        public float spawnProbability = 0.8f; // 80% spawn chance
        public int fullWidthPlatformEvery = 500; // Spawn full-width platform every X meters
        public Vector2 spawnHeightRange = new Vector2(0, 10000); // Platforms spawn within this height range
    }
}
