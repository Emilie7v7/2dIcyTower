using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Wall Spawn Data/Base Data", fileName = "newWallSpawnData")]
    public class WallSpawnSettingsSo : ScriptableObject
    {
        [Header("Wall Settings")]
        public RuleTile[] wallRuleTiles; // RuleTiles for walls to handle proper tiling
        public int leftWallX = -7; // X position of the left wall
        public int rightWallX = 7; // X position of the right wall
        public bool changeWallStyleAtHeight = false; // If true, walls change at higher altitudes
        public int wallStyleChangeInterval = 5000; // Change wall style every X meters
    }
}
