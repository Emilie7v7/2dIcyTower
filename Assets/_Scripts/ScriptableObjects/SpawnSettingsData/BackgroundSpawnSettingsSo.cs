using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Background Spawn Data/Base Data", fileName = "newBackgroundSpawnData")]
    public class BackgroundSpawnSettingsSo : ScriptableObject
    {
        [Header("Background Settings")]
        public TileBase[] backgroundTiles; // Background tile variations
        public bool changeBackgroundAtHeight = true; // Change backgrounds at different altitudes
        public int backgroundChangeInterval = 5000; // Change background every X meters
    }
}
