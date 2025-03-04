using UnityEditor;
using UnityEngine;

namespace _Scripts.EditorHelper
{
    public static class EditorHelper
    {
        public static void DrawFoldout(ref bool foldoutState, string title, GUIStyle foldoutStyle, System.Action drawAction)
        {
            foldoutState = EditorGUILayout.Foldout(foldoutState, title, true, foldoutStyle);
            if (foldoutState)
            {
                EditorGUI.indentLevel++;
                drawAction?.Invoke();
                EditorGUI.indentLevel--;
            }
        }
        
        public static void DrawUiLine(Color color, int thickness = 2, int padding = 10)
        {
            var rect = EditorGUILayout.GetControlRect(false, thickness + padding);
            rect.height = thickness;
            rect.y += padding * 0.5f;
            EditorGUI.DrawRect(rect, color);
        }
    }
}