using System;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editors
{
    [CustomEditor(typeof(ObjectSpawnSettingsSo))]
    public class ObjectSpawnSettingsSoEditor : Editor
    {
        private SerializedProperty _coinsPerChunk;

        private void OnEnable()
        {
            _coinsPerChunk = serializedObject.FindProperty("coinsPerChunk");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            
            _coinsPerChunk.intValue = EditorGUILayout.IntSlider("Coins per Chunk", _coinsPerChunk.intValue, 5, 10);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
