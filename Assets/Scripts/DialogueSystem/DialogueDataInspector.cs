#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(DialogueData))]
public class DialogueDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueData dialogueData = (DialogueData)target;

        if (GUILayout.Button("Reset"))
        {
            dialogueData.ResetDialogue();
        }
    }
}
#endif