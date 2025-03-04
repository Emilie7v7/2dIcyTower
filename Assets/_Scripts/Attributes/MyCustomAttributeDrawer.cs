using UnityEditor;
using UnityEngine;

namespace _Scripts.Attributes
{
    public class MyCustomAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(MyCustomAttribute))]
    public class MyCustomAttributeDrawer : PropertyDrawer
    {
        // Example dropdown options; you could populate this dynamically.
        private string[] options = new string[] { "Setting A", "Setting B", "Setting C" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the label
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width * 0.3f, position.height), label);
            // Draw a popup dropdown for the text field
            int currentIndex = System.Array.IndexOf(options, property.stringValue);
            if (currentIndex < 0) currentIndex = 0;
            int selectedIndex = EditorGUI.Popup(new Rect(position.x + position.width * 0.3f, position.y, position.width * 0.7f, position.height),
                currentIndex, options);
            property.stringValue = options[selectedIndex];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}