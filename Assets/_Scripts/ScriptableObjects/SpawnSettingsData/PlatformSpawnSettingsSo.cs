using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Platform Spawn Data/Base Data", fileName = "newPlatformSpawnData")]
    public class PlatformSpawnSettingsSo : ScriptableObject
    {
        [Header("Platform Settings")]
        public RuleTile[] platformTiles; // Different tile types
        public int minWidth = 10;
        public int maxWidth = 15;
        public Vector2 spawnHeightRange = new Vector2(0, 10000); // Platforms spawn within this height range
    }
}
