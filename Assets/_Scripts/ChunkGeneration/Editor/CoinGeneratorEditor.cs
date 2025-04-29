using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(CoinGenerator))]
    public class CoinGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _coinPrefab;
        private SerializedProperty _minCoinsPerChunk;
        private SerializedProperty _maxCoinsPerChunk;
        private SerializedProperty _minDistanceBetweenCoins;
        
        private bool _showDebugTools;

        private void OnEnable()
        {
            // Make sure these match exactly with the variable names in CoinGenerator
            _coinPrefab = serializedObject.FindProperty("coinPrefab");
            _minCoinsPerChunk = serializedObject.FindProperty("minCoinsPerChunk");
            _maxCoinsPerChunk = serializedObject.FindProperty("maxCoinsPerChunk");
            _minDistanceBetweenCoins = serializedObject.FindProperty("minDistanceBetweenCoins");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Coin Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_coinPrefab, new GUIContent("Coin Prefab"));
            EditorGUILayout.PropertyField(_minCoinsPerChunk, new GUIContent("Min Coins Per Chunk"));
            EditorGUILayout.PropertyField(_maxCoinsPerChunk, new GUIContent("Max Coins Per Chunk"));
            EditorGUILayout.PropertyField(_minDistanceBetweenCoins, new GUIContent("Min Distance Between Coins"));
            
            EditorGUILayout.Space(10);
            
            _showDebugTools = EditorGUILayout.Foldout(_showDebugTools, "Debug Tools", true);
            if (_showDebugTools)
            {
                EditorGUILayout.Space(5);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (GUILayout.Button("Clear Coin Prefab"))
                {
                    _coinPrefab.objectReferenceValue = null;
                }

                if (GUILayout.Button("Reset Min/Max Coins Per Chunk"))
                {
                    _minCoinsPerChunk.intValue = 0;
                    _maxCoinsPerChunk.intValue = 0;
                }
                EditorGUILayout.EndVertical();
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
