using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(XLuaBehaviour))]
[CanEditMultipleObjects]
public class XLuaBehaviourEditor : Editor
{
    SerializedProperty luaScriptName;
    SerializedProperty assetBundleName;
    //SerializedProperty injections;
    XLuaBehaviour _target;
    bool injectionCollapse = true;

    void OnEnable()
    {
        _target = target as XLuaBehaviour;
        // Setup the SerializedProperties.
        luaScriptName = serializedObject.FindProperty("luaScriptName");
        assetBundleName = serializedObject.FindProperty("assetBundleName");
        //injections = serializedObject.FindProperty("injections");
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();
        EditorGUILayout.PropertyField(assetBundleName, new GUIContent("Asset Bundle Name"));
        EditorGUILayout.PropertyField(luaScriptName, new GUIContent("Lua Script Name"));
        EditorGUILayout.Separator();
        injectionCollapse = EditorGUILayout.Foldout(injectionCollapse, "Injection");
        if (injectionCollapse)
        {
            for (int i = 0; i < _target.injections.Count; i++)
            {
                var e = _target.injections[i];
                EditorGUILayout.BeginHorizontal();
                e.name = EditorGUILayout.TextField("Name", e.name);
                if (GUILayout.Button("-", GUILayout.Width(16), GUILayout.Height(14)))
                {
                    _target.injections.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
                e.value = EditorGUILayout.ObjectField("Value", e.value, typeof(Object), true);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Separator();
            if (GUILayout.Button("+", GUILayout.Width(80)))
            {
                _target.injections.Add(new XLuaInjection());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_target);
            EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
        }
    }

}
