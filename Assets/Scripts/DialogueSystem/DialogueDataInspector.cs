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
            dialogueData.isLocked = dialogueData.GetBaseIsLocked();
            foreach (var node in dialogueData.nodes)
            {
                node.isSentCurrent = node.IsSentBase;

            }
        }
    }
}