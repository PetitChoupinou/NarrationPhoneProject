using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;

    [MenuItem("Window/Dialogue Graph")]
    public static void OpenDialogueGraph()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void ConstructGraph()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var nodeCreateButton = new Button(() => { _graphView.CreateNode(NodeType.Choice); });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(toolbar);
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
