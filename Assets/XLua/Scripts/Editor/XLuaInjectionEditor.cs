using UnityEditor;
using UnityEngine;

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(XLuaInjection), true)]
[CanEditMultipleObjects]
public class XLuaInjectionEditor : Editor
{
    SerializedProperty injectionName;
    SerializedProperty injectionValue;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        injectionName = serializedObject.FindProperty("name");
        injectionValue = serializedObject.FindProperty("value");
    }

    public override void OnInspectorGUI()
    {
        //XLuaInjection tw = target as XLuaInjection;
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        EditorGUILayout.PropertyField(injectionName, new GUIContent("Injection Name"));
        EditorGUILayout.PropertyField(injectionValue, new GUIContent("Injection Value"));
        EditorGUILayout.PropertyField(injectionValue, new GUIContent("Injection Type"));

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

   
}
