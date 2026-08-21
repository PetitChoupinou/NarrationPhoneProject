#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalPropertiesData))]
public class GlobalPropertiesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("globalProperties"), true);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif