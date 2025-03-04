using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editors
{
    [CustomEditor(typeof(ObjectSpawnSettingsSo))]
    public class ObjectSpawnSettingsSoEditor : Editor
    {
        private SerializedProperty _coinsPrefab;
        private SerializedProperty _minCoinsPerChunk;
        private SerializedProperty _maxCoinsPerChunk;
        private SerializedProperty _enemiesPrefab;
        private SerializedProperty _minEnemiesPerChunk;
        private SerializedProperty _maxEnemiesPerChunk;
        private SerializedProperty _boostsPrefab;
        private SerializedProperty _dropBoostsChanceFromEnemy;
        private SerializedProperty _mustSpawnOnPlatform;
        private SerializedProperty _canSpawnAnywhere;
        private SerializedProperty _spawnHeightRange;

        private void OnEnable()
        {
            _coinsPrefab = serializedObject.FindProperty("coinsPrefab");
            _minCoinsPerChunk = serializedObject.FindProperty("minCoinsPerChunk");
            _maxCoinsPerChunk = serializedObject.FindProperty("maxCoinsPerChunk");
            _enemiesPrefab = serializedObject.FindProperty("enemiesPrefab");
            _minEnemiesPerChunk = serializedObject.FindProperty("minEnemiesPerChunk");
            _maxEnemiesPerChunk = serializedObject.FindProperty("maxEnemiesPerChunk");
            _boostsPrefab = serializedObject.FindProperty("boostsPrefab");
            _dropBoostsChanceFromEnemy = serializedObject.FindProperty("dropBoostsChanceFromEnemy");
            _mustSpawnOnPlatform = serializedObject.FindProperty("mustSpawnOnPlatform");
            _canSpawnAnywhere = serializedObject.FindProperty("canSpawnAnywhere");
            _spawnHeightRange = serializedObject.FindProperty("spawnHeightRange");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Manually control the layout
            EditorGUILayout.LabelField("Object Spawn Settings", EditorStyles.boldLabel);
        
            // Coins Prefab
            EditorGUILayout.PropertyField(_coinsPrefab, new GUIContent("Coins Prefab"));

            // Min and Max Coins Per Chunk Sliders
            _minCoinsPerChunk.intValue = EditorGUILayout.IntSlider("Min Coins Per Chunk", _minCoinsPerChunk.intValue, 1, 10);
            _maxCoinsPerChunk.intValue = EditorGUILayout.IntSlider("Max Coins Per Chunk", _maxCoinsPerChunk.intValue, _minCoinsPerChunk.intValue, 10);
            
            // Enemies Prefab Section
            EditorGUILayout.PropertyField(_enemiesPrefab, new GUIContent("Enemies Prefab"));
            EditorGUILayout.PropertyField(_minEnemiesPerChunk);
            EditorGUILayout.PropertyField(_maxEnemiesPerChunk);

            // Boosts Prefab Section
            EditorGUILayout.PropertyField(_boostsPrefab, new GUIContent("Boosts Prefab"));
            EditorGUILayout.PropertyField(_dropBoostsChanceFromEnemy);

            // Other Spawn Settings
            EditorGUILayout.PropertyField(_mustSpawnOnPlatform);
            EditorGUILayout.PropertyField(_canSpawnAnywhere);
            EditorGUILayout.PropertyField(_spawnHeightRange);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
