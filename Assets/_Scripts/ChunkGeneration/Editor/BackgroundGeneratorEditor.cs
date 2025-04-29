using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    [CustomEditor(typeof(BackgroundGenerator))]
    public class BackgroundGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _backgroundRuleTile;
        private SerializedProperty _noise;
        private SerializedProperty _useNoise;
        private SerializedProperty _noiseScale;
        private SerializedProperty _threshold;
        private SerializedProperty _decorationTiles;
        private SerializedProperty _decorationChance;


        private bool _showDebugTools;

        private void OnEnable()
        {
            _backgroundRuleTile = serializedObject.FindProperty("backgroundRuleTile");
            _noise = serializedObject.FindProperty("noise");
            _useNoise = serializedObject.FindProperty("useNoise");
            _noiseScale = serializedObject.FindProperty("noiseScale");
            _threshold = serializedObject.FindProperty("threshold");
            _decorationTiles = serializedObject.FindProperty("decorationTiles");
            _decorationChance = serializedObject.FindProperty("decorationChance");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Background Settings
            EditorGUILayout.LabelField("Background Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_backgroundRuleTile);

            EditorGUILayout.Space(10);
            
            // Noise Settings Section
            EditorGUILayout.LabelField("Noise Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_useNoise, new GUIContent("Use Noise", 
                "Enable to create patterns and holes in the background"));

            using (new EditorGUI.IndentLevelScope())
            {
                if (_useNoise.boolValue)
                {
                    // Noise Scale with slider
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Noise Scale", 
                        "Controls the size of patterns. Lower values = smaller patterns"));
                    _noiseScale.floatValue = EditorGUILayout.Slider(_noiseScale.floatValue, 1f, 20f);
                    EditorGUILayout.EndHorizontal();

                    // Threshold with slider
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent("Threshold", 
                        "Controls how many gaps appear. Higher values = more gaps"));
                    _threshold.floatValue = EditorGUILayout.Slider(_threshold.floatValue, 0f, 1f);
                    EditorGUILayout.EndHorizontal();

                    // Noise variation
                    EditorGUILayout.PropertyField(_noise, new GUIContent("Noise Variation", 
                        "Additional variation factor for the noise pattern"));

                    // Pattern Preview Help Box
                    EditorGUILayout.HelpBox(GetPatternDescription(), MessageType.Info);
                }
            }
            
            EditorGUILayout.Space(10);
            
            // Validation Warning
            if (!_backgroundRuleTile.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("Background Rule Tile must be assigned for the generator to work!", 
                    MessageType.Warning);
            }

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Decoration Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_decorationTiles, true);
            
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(_decorationChance);
            
            // Debug Tools (Foldout)
            _showDebugTools = EditorGUILayout.Foldout(_showDebugTools, "Debug Tools", true);
            if (_showDebugTools)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (GUILayout.Button("Clear Background Rule Tile"))
                {
                    _backgroundRuleTile.objectReferenceValue = null;
                }

                if (GUILayout.Button("Reset Noise Settings"))
                {
                    _noiseScale.floatValue = 10f;
                    _threshold.floatValue = 0.5f;
                    _noise.floatValue = 0.5f;
                }
                EditorGUILayout.EndVertical();
            }

            

            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private string GetPatternDescription()
        {
            var threshold = _threshold.floatValue;
            var scale = _noiseScale.floatValue;

            var patternSize = scale switch
            {
                // Determine pattern size description
                < 5f => "Small",
                < 10f => "Medium",
                _ => "Large"
            };

            var density = threshold switch
            {
                // Determine density based on a threshold
                < 0.3f => "Very Dense",
                < 0.5f => "Dense",
                < 0.7f => "Sparse",
                _ => "Very Sparse"
            };

            // Determine the likely visual effect based on a combination
            var effectType = (scale, threshold) switch
            {
                // Handle invalid values first
                ( < 0f, _) => "Invalid scale value",
                (_, < 0f) => "Invalid threshold value",
                (_, > 1f) => "Invalid threshold value",
                (float.NaN, _) => "Invalid scale value",
                (_, float.NaN) => "Invalid threshold value",

                // Small Scale Patterns
                ( < 5f, < 0.3f) => "Swiss cheese effect",
                ( < 5f, < 0.5f) => "Dotted pattern",
                ( < 5f, < 0.7f) => "Starry background",
                ( < 5f, >= 0.7f) => "Scattered tiny dots",

                // Medium Scale Patterns
                ( < 10f, < 0.3f) => "Cave-like formation",
                ( < 10f, < 0.5f) => "Balanced cave system",
                ( < 10f, < 0.7f) => "Floating islands",
                ( < 10f, >= 0.7f) => "Scattered platforms",

                // Large Scale Patterns
                (_, < 0.3f) => "Large continuous areas",
                (_, < 0.5f) => "Large divided sections",
                (_, < 0.7f) => "Large floating masses",
                (_, >= 0.7f) => "Sparse large platforms",
                
            };

            return $"Pattern Type: {patternSize} {density} patterns\n" +
                   $"Visual Effect: {effectType}\n" +
                   $"Scale: {scale:F1} | Threshold: {threshold:F2}";
        }

    }
}
