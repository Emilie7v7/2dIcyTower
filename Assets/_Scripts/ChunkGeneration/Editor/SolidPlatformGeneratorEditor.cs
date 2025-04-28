using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(SolidPlatformGenerator))]
    public class SolidPlatformGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _solidPlatformRuleTile;
        private SerializedProperty _solidPlatformType;

        private void OnEnable()
        {
            _solidPlatformRuleTile = serializedObject.FindProperty("solidPlatformRuleTile");
            _solidPlatformType = serializedObject.FindProperty("solidPlatformType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Platform Settings
            EditorGUILayout.LabelField("Platform Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_solidPlatformRuleTile);
            EditorGUILayout.PropertyField(_solidPlatformType);

            // Platform Type Information
            EditorGUILayout.Space(5);
            DisplayPlatformTypeInfo((SolidPlatformType)_solidPlatformType.enumValueIndex);

            // Validation Warning
            if (_solidPlatformRuleTile.objectReferenceValue == null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("Solid Platform Rule Tile must be assigned!", MessageType.Warning);
            }

            // Debug Tools
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear Platform Rule Tile"))
            {
                _solidPlatformRuleTile.objectReferenceValue = null;
            }

            var generator = (SolidPlatformGenerator)target;
            if (GUILayout.Button("Add Single Platform"))
            {
                if (_solidPlatformType.enumValueIndex == (int)SolidPlatformType.None)
                {
                    EditorGUILayout.HelpBox("Please select a platform type first!", MessageType.Warning);
                }
                else
                {
                    generator.AddSinglePlatform();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DisplayPlatformTypeInfo(SolidPlatformType platformType)
        {
            string infoText;
            MessageType messageType = MessageType.Info;

            switch (platformType)
            {
                case SolidPlatformType.Cube:
                    infoText = "Cube Platform Properties:\n" +
                              "• Width: 10-15 tiles\n" +
                              "• Height: 7-9 tiles";
                    break;

                case SolidPlatformType.UpsideDownT:
                    infoText = "Upside Down T Platform Properties:\n" +
                              "• Top Bar Width: 12-16 tiles\n" +
                              "• Top Bar Height: 2-3 tiles\n" +
                              "• Stem Width: 3-5 tiles\n" +
                              "• Stem Height: 7-8 tiles";
                    break;

                case SolidPlatformType.ShapeT:
                    infoText = "T Shape Platform Properties:\n" +
                              "• Top Bar Width: 12-16 tiles\n" +
                              "• Top Bar Height: 2-3 tiles\n" +
                              "• Stem Width: 3-5 tiles\n" +
                              "• Stem Height: 7-8 tiles";
                    break;

                case SolidPlatformType.None:
                    infoText = "No platform type selected.\nSelect a platform type to generate platforms.";
                    messageType = MessageType.Warning;
                    break;

                default:
                    infoText = "Invalid platform type selected.";
                    messageType = MessageType.Error;
                    break;
            }

            EditorGUILayout.HelpBox(infoText, messageType);
        }
    }
}
