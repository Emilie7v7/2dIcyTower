using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Hazard Spawn Data/Base Data", fileName = "newHazardSpawnData")]
    public class HazardSpawnSettingsSo : ScriptableObject
    {
        [Header("Hazard Settings")]
        public RuleTile[] ruleTileHazards; // For hazards like lava
        public TileBase[] singleTileHazards; // For spikes, traps, etc.
        public float spawnProbability = 0.3f;
        public Vector2 triggerHeight = new Vector2(500, 10000);
        public bool risesOverTime = false; // For lava that moves up
    }
}