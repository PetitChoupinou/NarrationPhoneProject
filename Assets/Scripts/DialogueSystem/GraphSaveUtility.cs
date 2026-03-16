using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;

    private DialogueData _dataCache;

    private List<Edge> _edges => _targetGraphView.edges.ToList();
    private List<BaseNode> _nodes => _targetGraphView.nodes.ToList().Cast<BaseNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if(!_nodes.Any())
        {
            EditorUtility.DisplayDialog("Empty Graph!", "Cannot save an empty graph. Please add nodes before saving.", "OK");
            return;
        }

        var dialogueData = ScriptableObject.CreateInstance<DialogueData>();

        //Check toutes les connections 
        var connectedPorts = _edges.Where(edge => edge.input.node != null).ToArray();
        for(int i = 0;  i < connectedPorts.Length; i++)
        {
           
            var outputNode = connectedPorts[i].output.node as BaseNode;
            var inputNode = connectedPorts[i].input.node as BaseNode;

            
            dialogueData.nodeLinks.Add(new NodeLinkData
            {
                baseNodeGuid = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGuid = inputNode.GUID
            });
        }
        foreach(var graphNode in _nodes.Where(node => !node.isEntryPoint))
        {
            
            dialogueData.nodes.Add(new NodeData
            {
                nodeInfos = graphNode,
                position = graphNode.GetPosition().position,
                nodeType = graphNode.nodeType
            });
        }
        if (!AssetDatabase.IsValidFolder("Assets/Resources")){
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        AssetDatabase.CreateAsset(dialogueData, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

    }

    public void LoadGraph(string fileName)
    {
        _dataCache = Resources.Load<DialogueData>(fileName);
        if (_dataCache == null)
        {
            EditorUtility.DisplayDialog("File not found", $"No dialogue container found at path: {fileName}", "OK");
            return;
        }

        ClearGraph();

        CreateNodes();

        CreateLinks();
    }

    private void CreateLinks()
    {
        throw new NotImplementedException();
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _dataCache.nodes)
        {
            var tempNode = _targetGraphView.CreateFromData(_dataCache, nodeData);

        }
    }

    private void ClearGraph()
    {
        //_nodes.Find(x => x.isEntryPoint).GUID = _dataCache.nodeLinks[0].baseNodeGuid;

        foreach (var node in _nodes)
        {
            if (node.isEntryPoint) return;
            _edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}
