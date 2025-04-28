using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(WallDecorationGenerator))]
    public class WallDecorationGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _wallDecorTileLeft;
        private SerializedProperty _wallDecorTileRight;

        private void OnEnable()
        {
            _wallDecorTileLeft = serializedObject.FindProperty("wallDecorTileLeft");
            _wallDecorTileRight = serializedObject.FindProperty("wallDecorTileRight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Wall Decoration Tiles Section
            EditorGUILayout.LabelField("Wall Decoration Tiles", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_wallDecorTileLeft);
            EditorGUILayout.PropertyField(_wallDecorTileRight);

            // Validation Warning
            if (_wallDecorTileLeft.objectReferenceValue == null || _wallDecorTileRight.objectReferenceValue == null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("Both wall decoration tiles must be assigned for the generator to work properly!", 
                    MessageType.Warning);
            }

            // Debug Tools
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear Wall Decoration Tiles"))
            {
                _wallDecorTileLeft.objectReferenceValue = null;
                _wallDecorTileRight.objectReferenceValue = null;
            }

            // Copy Left to Right
            if (GUILayout.Button("Copy Left Tile to Right"))
            {
                if (_wallDecorTileLeft.objectReferenceValue != null)
                {
                    _wallDecorTileRight.objectReferenceValue = _wallDecorTileLeft.objectReferenceValue;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
