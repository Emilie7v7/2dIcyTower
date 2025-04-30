using System;
using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(WallsGenerator))]
    public class WallGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _wallThickness;
        private SerializedProperty _wallRuleTile;
        private SerializedProperty _wallDecorTileLeft;
        private SerializedProperty _wallDecorTileRight;


        private void OnEnable()
        {
            _wallThickness = serializedObject.FindProperty("wallThickness");
            _wallRuleTile = serializedObject.FindProperty("wallRuleTile");
            _wallDecorTileLeft = serializedObject.FindProperty("wallDecorTileLeft");
            _wallDecorTileRight = serializedObject.FindProperty("wallDecorTileRight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Draw properties
            EditorGUILayout.PropertyField(_wallRuleTile);
            EditorGUILayout.PropertyField(_wallDecorTileLeft);
            EditorGUILayout.PropertyField(_wallDecorTileRight);
            EditorGUILayout.IntField("Wall Thickness", _wallThickness.intValue);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Reset Wall Thickness"))
            {
                _wallThickness.intValue = 4;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }


    }
}