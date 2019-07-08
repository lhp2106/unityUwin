using UnityEditor;
using UnityEditor.EventSystems;

[CustomEditor(typeof(VKDragEvent))]
public class VKDragEventEditor : EventTriggerEditor
{
    public override void OnInspectorGUI()
    {
        VKDragEvent component = (VKDragEvent)target;

        base.OnInspectorGUI();

        var rectContent = serializedObject.FindProperty("rectContent");
        serializedObject.Update();
        EditorGUILayout.PropertyField(rectContent, true);
        serializedObject.ApplyModifiedProperties();

        var distance = serializedObject.FindProperty("distance");
        serializedObject.Update();
        EditorGUILayout.PropertyField(distance, true);
        serializedObject.ApplyModifiedProperties();
    }
}
