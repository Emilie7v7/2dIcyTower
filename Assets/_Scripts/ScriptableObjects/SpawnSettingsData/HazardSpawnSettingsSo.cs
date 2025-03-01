using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Hazard Spawn Data/Base Data", fileName = "newHazardSpawnData")]
    public class HazardSpawnSettingsSo : ScriptableObject
    {
        [Header("Hazard Settings")]
        public TileBase[] hazardTiles;
        public float spawnProbability = 0.3f; // Default 30% chance
        public Vector2 triggerHeight = new Vector2(500, 10000); // Hazards start appearing in this range
        public bool risesOverTime = false; // If true, hazard (lava) rises over time
    }
}
