using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor
{
    [CustomEditor(typeof(ObjectSpawnSettingsSo))]
    public class ObjectSpawnSettingsSoEditor : UnityEditor.Editor
    {
        private SerializedProperty _coinsPrefab;
        private SerializedProperty _minCoinsPerChunk;
        private SerializedProperty _maxCoinsPerChunk;
        private SerializedProperty _enemiesPrefab;
        private SerializedProperty _minEnemiesPerChunk;
        private SerializedProperty _maxEnemiesPerChunk;
        private SerializedProperty _mustSpawnOnPlatform;
        private SerializedProperty _canSpawnAnywhere;
        private SerializedProperty _spawnHeightRange;
        private SerializedProperty _boostsProperty;
        
        private GUIStyle _redStyle;
        private GUIStyle _yellowStyle;
        private GUIStyle _greenStyle;
        private GUIStyle _blackStyle;
        private GUIStyle _centerCyanStyle;

        private bool _showCoins;
        private bool _showEnemies;
        private bool _showBoosts;
        private bool _showOtherSettings;
        
        private void OnEnable()
        {
            _coinsPrefab = serializedObject.FindProperty("coinsPrefab");
            _minCoinsPerChunk = serializedObject.FindProperty("minCoinsPerChunk");
            _maxCoinsPerChunk = serializedObject.FindProperty("maxCoinsPerChunk");
            _enemiesPrefab = serializedObject.FindProperty("enemiesPrefab");
            _minEnemiesPerChunk = serializedObject.FindProperty("minEnemiesPerChunk");
            _maxEnemiesPerChunk = serializedObject.FindProperty("maxEnemiesPerChunk");
            _mustSpawnOnPlatform = serializedObject.FindProperty("mustSpawnOnPlatform");
            _canSpawnAnywhere = serializedObject.FindProperty("canSpawnAnywhere");
            _spawnHeightRange = serializedObject.FindProperty("spawnHeightRange");
            _boostsProperty = serializedObject.FindProperty("boosts");
            
            
            

            #region Custom Styles

            _redStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.red },
                focused = { textColor = Color.red }
            };

            _yellowStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow },
                focused = { textColor = Color.yellow }
            };

            _greenStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.green },
                focused = { textColor = Color.green }
            };

            _blackStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.black },
                focused = { textColor = Color.black }
            };

            _centerCyanStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.cyan },
                focused = { textColor = Color.cyan }
            };
            
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Title
            
            EditorGUILayout.LabelField("Object Spawn Settings", _centerCyanStyle);
            
            #endregion
            
            EditorGUILayout.Space(20);
            
            #region Coin Settings

            EditorHelper.EditorHelper.DrawUiLine(color: Color.yellow);

            EditorHelper.EditorHelper.DrawFoldout(ref _showCoins, "   Coin Settings", _yellowStyle, () =>
            {
                GUILayout.Space(5);

                // Draw the Coins Prefab property field.
                EditorGUILayout.PropertyField(_coinsPrefab, new GUIContent("Coins Prefab"));

                GUILayout.Space(5);

                // --- Min Coins Per Chunk ---
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(
                    "Min Coins Per Chunk",
                    new GUIStyle(EditorStyles.label) { normal = { textColor = new Color(0f, 1f, 0.5f) } },
                    GUILayout.Width(170)
                );
                // Minus button for decreasing the min enemies value
                if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.red } },
                        GUILayout.Width(25)))
                {
                    _minCoinsPerChunk.intValue = Mathf.Max(0, _minCoinsPerChunk.intValue - 1);
                }

                GUILayout.Space(-15);

                _minCoinsPerChunk.intValue = EditorGUILayout.IntField(_minCoinsPerChunk.intValue, GUILayout.Width(50));

                // Plus button for increasing the min enemies value
                if (GUILayout.Button("+",
                        new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.green } },
                        GUILayout.Width(25)))
                {
                    _minCoinsPerChunk.intValue++;
                }

                EditorGUILayout.EndHorizontal();

                // --- Max Coins Per Chunk ---
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(
                    "Max Coins Per Chunk",
                    new GUIStyle(EditorStyles.label) { normal = { textColor = new Color(1f, 0.73f, 0f) } },
                    GUILayout.Width(170)
                );
                // Minus button for decreasing the min enemies value
                if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.red } },
                        GUILayout.Width(25)))
                {
                    _maxCoinsPerChunk.intValue = Mathf.Max(0, _maxCoinsPerChunk.intValue - 1);
                }

                GUILayout.Space(-15);

                _maxCoinsPerChunk.intValue = EditorGUILayout.IntField(_maxCoinsPerChunk.intValue, GUILayout.Width(50));

                // Plus button for increasing the min enemies value
                if (GUILayout.Button("+",
                        new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.green } },
                        GUILayout.Width(25)))
                {
                    _maxCoinsPerChunk.intValue++;
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            });
            EditorHelper.EditorHelper.DrawUiLine(color: Color.yellow);

            #endregion
            
            EditorGUILayout.Space(10);
            
            #region Enemy Settings
            EditorHelper.EditorHelper.DrawUiLine(color: Color.red);
            
            EditorHelper.EditorHelper.DrawFoldout(ref _showEnemies, "   Enemy Settings", _redStyle, () =>
            {
                GUILayout.Space(5);
                
                // Draw the Enemies Prefab property field.
                EditorGUILayout.PropertyField(_enemiesPrefab, new GUIContent("Enemies Prefab"));
                
                GUILayout.Space(5);
                
                // --- Min Enemies Per Chunk ---
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(
                    "Min Enemies Per Chunk",
                    new GUIStyle(EditorStyles.label) { normal = { textColor = new Color(0f, 1f, 0.5f) } },
                    GUILayout.Width(170)
                );
                
                // Minus button for decreasing the min enemies value
                if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.red } }, GUILayout.Width(25)))
                {
                    _minEnemiesPerChunk.intValue = Mathf.Max(0, _minEnemiesPerChunk.intValue - 1);
                }
                
                GUILayout.Space(-15);
                
                _minEnemiesPerChunk.intValue = EditorGUILayout.IntField(_minEnemiesPerChunk.intValue, GUILayout.Width(50));
                
                // Plus button for increasing the min enemies value
                if (GUILayout.Button("+", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.green } }, GUILayout.Width(25)))
                {
                    _minEnemiesPerChunk.intValue++;
                }
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5);

                // --- Max Enemies Per Chunk ---
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(
                    "Max Enemies Per Chunk",
                    new GUIStyle(EditorStyles.label) { normal = { textColor = new Color(1f, 0.73f, 0f) } },
                    GUILayout.Width(170)
                );
                
                // Minus button for decreasing the max enemies value
                if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.red } }, GUILayout.Width(25)))
                {
                    _maxEnemiesPerChunk.intValue = Mathf.Max(0, _maxEnemiesPerChunk.intValue - 1);
                }
                
                GUILayout.Space(-15);
                
                _maxEnemiesPerChunk.intValue = EditorGUILayout.IntField(_maxEnemiesPerChunk.intValue, GUILayout.Width(50));
                
                // Plus button for increasing the max enemies value
                if (GUILayout.Button("+", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.green } }, GUILayout.Width(25)))
                {
                    _maxEnemiesPerChunk.intValue++;
                }
                EditorGUILayout.EndHorizontal();
            });
            EditorHelper.EditorHelper.DrawUiLine(color: Color.red);
            #endregion

            EditorGUILayout.Space(10);

            #region Boost settings

            EditorHelper.EditorHelper.DrawUiLine(color: Color.green);
                
            EditorHelper.EditorHelper.DrawFoldout(ref _showBoosts, "   Boost Settings", _greenStyle, () =>
            {
                GUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                    
                GUILayout.Space(13);
                    
                if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.red } }, 
                        GUILayout.Width(25)))
                {
                    _boostsProperty.arraySize = Mathf.Max(0, _boostsProperty.arraySize - 1);
                }
                    
                GUILayout.Space(-17);
                    
                EditorGUILayout.LabelField(new GUIContent($"{_boostsProperty.arraySize}: Elements"),
                    new GUIStyle(EditorStyles.label) { normal = { textColor = Color.white }, alignment = TextAnchor.MiddleCenter},
                    GUILayout.Width(100));
                
                if (GUILayout.Button("+", new GUIStyle(EditorStyles.miniButton) { normal = { textColor = Color.green } },
                        GUILayout.Width(25)))
                {
                    _boostsProperty.arraySize++;
                }
                    
                EditorGUILayout.EndHorizontal();
                    
                // 4) Iterate through the array elements, applying custom labels/colors
                for (var i = 0; i < _boostsProperty.arraySize; i++)
                {
                    var element = _boostsProperty.GetArrayElementAtIndex(i);
                    var nameProp = element.FindPropertyRelative("boostName");
                    var dropChanceProp = element.FindPropertyRelative("dropChance");
                    var boostProp = element.FindPropertyRelative("boostPrefab");

                    var color = i switch
                    {
                        0 => Color.red,
                        1 => Color.cyan,
                        2 => Color.yellow,
                        3 => Color.white,
                        _ => Color.black
                    };
                    var coloredLabelStyle = new GUIStyle(EditorStyles.label) { normal = { textColor = color } };
                    var textFieldStyle = new GUIStyle(EditorStyles.textField) { normal = { textColor = color },
                        focused = { textColor = color },
                        fontStyle = FontStyle.Bold };
                    
                    EditorGUILayout.BeginHorizontal();

                    var newNameValue = EditorGUILayout.TextField(nameProp.stringValue, textFieldStyle, GUILayout.Width(200));
                    nameProp.stringValue = newNameValue;

                    EditorGUILayout.PropertyField(dropChanceProp, GUIContent.none, GUILayout.Width(65));
                    GUILayout.Space(-33);
                    EditorGUILayout.LabelField("%",coloredLabelStyle , GUILayout.Width(50));
                    EditorGUILayout.PropertyField(boostProp, GUIContent.none, GUILayout.Width(200));

                    EditorGUILayout.EndHorizontal();
                }
            });
                EditorHelper.EditorHelper.DrawUiLine(color: Color.green);

            #endregion

            EditorGUILayout.Space(10);
            
            #region Other settings

                EditorHelper.EditorHelper.DrawUiLine(color: Color.black);
                
                EditorHelper.EditorHelper.DrawFoldout(ref _showOtherSettings, "   Other Settings", _blackStyle, () =>
                {
                    EditorGUILayout.PropertyField(_mustSpawnOnPlatform);
                    EditorGUILayout.PropertyField(_canSpawnAnywhere);
                    EditorGUILayout.PropertyField(_spawnHeightRange);
                });
                EditorHelper.EditorHelper.DrawUiLine(color: Color.black);

            #endregion
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

