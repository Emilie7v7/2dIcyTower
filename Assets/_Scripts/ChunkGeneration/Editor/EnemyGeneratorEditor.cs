using System;
using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(EnemyGenerator))]
    public class EnemyGeneratorEditor : UnityEditor.Editor
    {
        
        private SerializedProperty _minEnemiesPerChunk;
        private SerializedProperty _maxEnemiesPerChunk;
        private SerializedProperty _minDistanceBetweenEnemies;
        private SerializedProperty _enemyPrefab;
        
        private bool _showDebugTools;

        private void OnEnable()
        {
            _minEnemiesPerChunk = serializedObject.FindProperty("minEnemiesPerChunk");
            _maxEnemiesPerChunk = serializedObject.FindProperty("maxEnemiesPerChunk");
            _minDistanceBetweenEnemies = serializedObject.FindProperty("distanceBetweenEnemies");
            _enemyPrefab = serializedObject.FindProperty("enemyPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_enemyPrefab);
            EditorGUILayout.PropertyField(_minEnemiesPerChunk);
            EditorGUILayout.PropertyField(_maxEnemiesPerChunk);
            EditorGUILayout.PropertyField(_minDistanceBetweenEnemies);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);
            
            _showDebugTools = EditorGUILayout.Foldout(_showDebugTools, "Debug Tools", true);
            if (_showDebugTools)
            {
                EditorGUILayout.Space(5);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (GUILayout.Button("Clear Enemy Prefab"))
                {
                    _enemyPrefab.objectReferenceValue = null;
                }

                if (GUILayout.Button("Reset Min/Max Enemies Per Chunk"))
                {
                    _minEnemiesPerChunk.intValue = 0;
                    _maxEnemiesPerChunk.intValue = 0;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}