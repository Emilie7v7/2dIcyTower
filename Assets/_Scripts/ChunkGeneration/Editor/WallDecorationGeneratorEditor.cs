using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(WallDecorationGenerator))]
    public class WallDecorationGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _wallDecorTileLeft;
        private SerializedProperty _wallDecorTileRight;
        private SerializedProperty _leftTorchPrefab;
        private SerializedProperty _rightTorchPrefab;
        private SerializedProperty _torchSpacing;
        private SerializedProperty _torchLeftWallCount;
        private SerializedProperty _torchRightWallCount;

        private void OnEnable()
        {
            _wallDecorTileLeft = serializedObject.FindProperty("wallDecorTileLeft");
            _wallDecorTileRight = serializedObject.FindProperty("wallDecorTileRight");
            _leftTorchPrefab = serializedObject.FindProperty("leftTorchPrefab");
            _rightTorchPrefab = serializedObject.FindProperty("rightTorchPrefab");
            _torchSpacing = serializedObject.FindProperty("torchSpacing");
            _torchLeftWallCount = serializedObject.FindProperty("torchLeftWallCount");
            _torchRightWallCount = serializedObject.FindProperty("torchRightWallCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Wall Decoration Tiles Section
            EditorGUILayout.LabelField("Wall Decoration Tiles", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_wallDecorTileLeft);
            EditorGUILayout.PropertyField(_wallDecorTileRight);

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(_leftTorchPrefab);
            EditorGUILayout.PropertyField(_rightTorchPrefab);
            _torchSpacing.intValue = EditorGUILayout.IntField("Torch Spacing", _torchSpacing.intValue);
            _torchLeftWallCount.intValue = EditorGUILayout.IntField("Torch Left Wall Count", _torchLeftWallCount.intValue);
            _torchRightWallCount.intValue = EditorGUILayout.IntField("Torch Right Wall Count", _torchRightWallCount.intValue);
            
            // Validation Warning
            if (!_wallDecorTileLeft.objectReferenceValue || !_wallDecorTileRight.objectReferenceValue)
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
                if (_wallDecorTileLeft.objectReferenceValue)
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
