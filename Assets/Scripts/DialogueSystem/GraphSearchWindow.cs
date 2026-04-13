using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView _graphView;
    private EditorWindow _window;
    private Texture2D _indentationIcon;

    public void Init(EditorWindow window, DialogueGraphView graphView)
    {
        _graphView = graphView;
        _window = window;

        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _indentationIcon.Apply();
    }
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            new SearchTreeEntry(new GUIContent("Affinity Node", _indentationIcon)) { level = 1, userData = NodeType.Affinity },
            new SearchTreeEntry(new GUIContent("Block Node", _indentationIcon)) { level = 1, userData = NodeType.Block },
            new SearchTreeEntry(new GUIContent("Choice Node", _indentationIcon)) { level = 1, userData = NodeType.Choice },
            new SearchTreeEntry(new GUIContent("Conditional Node", _indentationIcon)) { level = 1, userData = NodeType.Condition },
            new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon)) { level = 1, userData = NodeType.Dialogue },
            new SearchTreeEntry(new GUIContent("Note Node", _indentationIcon)) { level = 1, userData = NodeType.Note },
            new SearchTreeEntry(new GUIContent("Set Property Node", _indentationIcon)) { level = 1, userData = NodeType.Set },
            new SearchTreeEntry(new GUIContent("Thinking Node", _indentationIcon)) { level = 1, userData = NodeType.Thinking },
            new SearchTreeEntry(new GUIContent("Unlock Node", _indentationIcon)) { level = 1, userData = NodeType.Unlock },
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        _graphView.CreateNode((NodeType)SearchTreeEntry.userData, localMousePosition);
        return true;
    }
}
