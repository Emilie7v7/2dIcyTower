using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(SolidPlatformGenerator))]
    public class SolidPlatformGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _solidPlatformRuleTile;
        private SerializedProperty _solidPlatformType;
        private SerializedProperty _amountOfPlatforms;
        private SerializedProperty _platformSpacing;
        private SerializedProperty _placeIndividualType;

        private void OnEnable()
        {
            _solidPlatformRuleTile = serializedObject.FindProperty("solidPlatformRuleTile");
            _solidPlatformType = serializedObject.FindProperty("solidPlatformType");
            _amountOfPlatforms = serializedObject.FindProperty("amountOfPlatforms");
            _platformSpacing = serializedObject.FindProperty("platformSpacing");
            _placeIndividualType = serializedObject.FindProperty("placeIndividualType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Platform Settings
            EditorGUILayout.LabelField("Platform Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_solidPlatformRuleTile);
            EditorGUILayout.PropertyField(_amountOfPlatforms);
            EditorGUILayout.PropertyField(_platformSpacing);
            
            EditorGUILayout.PropertyField(_placeIndividualType);
            
            if (_placeIndividualType.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_solidPlatformType);
                EditorGUILayout.Space(5);
                
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
                // Platform Type Information
                EditorGUILayout.Space(5);
                DisplayPlatformTypeInfo((SolidPlatformType)_solidPlatformType.enumValueIndex);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            
            // Validation Warning
            if (!_solidPlatformRuleTile.objectReferenceValue)
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

            

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void DisplayPlatformTypeInfo(SolidPlatformType platformType)
        {
            string infoText;
            var messageType = MessageType.Info;

            switch (platformType)
            {
                case SolidPlatformType.Cube:
                    infoText = "Cube Platform Properties:\n" +
                              "• Width: 10-15 tiles\n" +
                              "• Height: 7-9 tiles";
                    break;
                case SolidPlatformType.SmallCube:
                    infoText = "Small Cube Platform Properties:\n" +
                                "• Width: 3-4 tiles\n" +
                                "• Height: 3-4 tiles";
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
