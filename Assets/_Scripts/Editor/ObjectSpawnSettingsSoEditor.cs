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
        private SerializedProperty _boostsPrefab;
        private SerializedProperty _dropBoostsChanceFromEnemy;
        private SerializedProperty _mustSpawnOnPlatform;
        private SerializedProperty _canSpawnAnywhere;
        private SerializedProperty _spawnHeightRange;
        
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
            _boostsPrefab = serializedObject.FindProperty("boostsPrefab");
            _dropBoostsChanceFromEnemy = serializedObject.FindProperty("dropBoostsChanceFromEnemy");
            _mustSpawnOnPlatform = serializedObject.FindProperty("mustSpawnOnPlatform");
            _canSpawnAnywhere = serializedObject.FindProperty("canSpawnAnywhere");
            _spawnHeightRange = serializedObject.FindProperty("spawnHeightRange");

            #region Custom Styles

            _redStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.red
                }
            };

            _yellowStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            };

            _greenStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.green
                }
            };

            _blackStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.black
                }
            };

            _centerCyanStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.cyan
                }
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
                EditorGUILayout.PropertyField(_coinsPrefab, new GUIContent("Coins Prefab"));
                _minCoinsPerChunk.intValue = EditorGUILayout.IntSlider("Min Coins Per Chunk", _minCoinsPerChunk.intValue, 1, 10);
                _maxCoinsPerChunk.intValue = EditorGUILayout.IntSlider("Max Coins Per Chunk", _maxCoinsPerChunk.intValue, _minCoinsPerChunk.intValue, 10);
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
                    EditorGUILayout.PropertyField(_boostsPrefab, new GUIContent("Boosts Prefab"));
                    EditorGUILayout.PropertyField(_dropBoostsChanceFromEnemy);
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

