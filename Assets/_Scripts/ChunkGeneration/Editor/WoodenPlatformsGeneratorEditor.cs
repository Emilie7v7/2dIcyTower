using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(WoodenPlatformsGenerator))]
    public class WoodenPlatformsGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _platformRuleTile;

        private void OnEnable()
        {
            _platformRuleTile = serializedObject.FindProperty("platformRuleTile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Platform Settings
            EditorGUILayout.LabelField("Platform Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_platformRuleTile);

            // Information Box
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "Platform Generation Parameters:\n" +
                "• Min Y Distance: 7\n" +
                "• Max Y Distance: 8\n" +
                "• Min Platform Length: 9\n" +
                "• Max Platform Length: 14\n" +
                "• Starting Y Position: 10",
                MessageType.Info);

            // Validation Warning
            if (!_platformRuleTile.objectReferenceValue)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(
                    "Platform Rule Tile must be assigned! Make sure it is placed in the 'Resources/Tiles' folder.", 
                    MessageType.Warning);
            }

            // Debug Tools
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear Platform Rule Tile"))
            {
                _platformRuleTile.objectReferenceValue = null;
            }

            var generator = (WoodenPlatformsGenerator)target;
            if (GUILayout.Button("Generate Test Full Width Platform"))
            {
                if (Selection.activeGameObject)
                {
                    var tilemap = Selection.activeGameObject.GetComponent<Tilemap>();
                    if (tilemap)
                    {
                        generator.GenerateFullWidthPlatform(tilemap);
                    }
                    else
                    {
                        Debug.LogWarning("Please select a GameObject with a Tilemap component");
                    }
                }
                else
                {
                    Debug.LogWarning("Please select a GameObject in the scene");
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
