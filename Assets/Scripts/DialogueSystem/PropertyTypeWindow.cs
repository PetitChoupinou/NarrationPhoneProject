using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PropertyTypeWindow : ScriptableObject, ISearchWindowProvider
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
            new SearchTreeGroupEntry(new GUIContent("Property Type")) { level = 0 },
            new SearchTreeEntry(new GUIContent("Bool", _indentationIcon)) { level = 1, userData = typeof(bool) },
            new SearchTreeEntry(new GUIContent("Int", _indentationIcon)) { level = 1, userData = typeof(int) },
            new SearchTreeEntry(new GUIContent("Float", _indentationIcon)) { level = 1, userData = typeof(float) },
            new SearchTreeEntry(new GUIContent("String", _indentationIcon)) { level = 1, userData = typeof(string) },
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        _graphView.AddPropertyToBlackboard((Type)SearchTreeEntry.userData);
        //_graphView.CreateNode((NodeType)SearchTreeEntry.userData, localMousePosition);
        return true;
        /*switch (SearchTreeEntry.userData)
        {
            case NodeType.Dialogue:
                _graphView.CreateNode(NodeType.Dialogue, localMousePosition);
                return true;
            case NodeType.Choice:
                _graphView.CreateNode(NodeType.Choice, localMousePosition);
                return true;
            case NodeType.Affinity:
                _graphView.CreateNode(NodeType.Affinity, localMousePosition);
                return true;
            default:
                return false;
        }*/
    }
}
