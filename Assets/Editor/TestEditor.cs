using UnityEditor;
using UnityEngine;

public class TestEditor : EditorWindow
{
    [MenuItem("Tools/Test Editor Window")]
    static void Init()
    {
        TestEditor window = (TestEditor)EditorWindow.GetWindow(typeof(TestEditor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Test Editor Window");
    }
}