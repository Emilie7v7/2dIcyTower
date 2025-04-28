using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(DartTrapGenerator))]
    public class DartTrapGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _dartTrapsGaps;
        private SerializedProperty _dartsOnTheLeftWall;
        private SerializedProperty _dartsOnTheRightWall;
        private SerializedProperty _fillDartTrapGaps;
        private SerializedProperty _dartTrapPrefabLeftWall;
        private SerializedProperty _dartTrapPrefabRightWall;
        private SerializedProperty _amountOfGapsOnLeftWall;
        private SerializedProperty _amountOfGapsOnRightWall;

        private void OnEnable()
        {
            _dartTrapsGaps = serializedObject.FindProperty("dartTrapsGaps");
            _dartsOnTheLeftWall = serializedObject.FindProperty("dartsOnTheLeftWall");
            _dartsOnTheRightWall = serializedObject.FindProperty("dartsOnTheRightWall");
            _fillDartTrapGaps = serializedObject.FindProperty("fillDartTrapGaps");
            _dartTrapPrefabLeftWall = serializedObject.FindProperty("dartTrapPrefabLeftWall");
            _dartTrapPrefabRightWall = serializedObject.FindProperty("dartTrapPrefabRightWall");
            _amountOfGapsOnLeftWall = serializedObject.FindProperty("amountOfGapsOnLeftWall");
            _amountOfGapsOnRightWall = serializedObject.FindProperty("amountOfGapsOnRightWall");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Main Settings Section
            EditorGUILayout.LabelField("Main Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_dartTrapsGaps);
            EditorGUILayout.PropertyField(_fillDartTrapGaps);

            // Left Wall Settings
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Left Wall Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_dartsOnTheLeftWall);
            
            if (_dartsOnTheLeftWall.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_dartTrapPrefabLeftWall);
                EditorGUILayout.PropertyField(_amountOfGapsOnLeftWall);
                EditorGUI.indentLevel--;
            }

            // Right Wall Settings
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Right Wall Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_dartsOnTheRightWall);
            
            if (_dartsOnTheRightWall.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_dartTrapPrefabRightWall);
                EditorGUILayout.PropertyField(_amountOfGapsOnRightWall);
                EditorGUI.indentLevel--;
            }

            // Debug Tools
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Reset Gap Amounts"))
            {
                _amountOfGapsOnLeftWall.intValue = 0;
                _amountOfGapsOnRightWall.intValue = 0;
            }

            if (GUILayout.Button("Toggle All Dart Traps"))
            {
                var currentState = _dartTrapsGaps.boolValue;
                _dartTrapsGaps.boolValue = !currentState;
                _dartsOnTheLeftWall.boolValue = !currentState;
                _dartsOnTheRightWall.boolValue = !currentState;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}