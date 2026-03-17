using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
        foreach(var graphNode in _nodes)
        {
            if (graphNode.isEntryPoint)
            {
                dialogueData.entryPointNodeGuid = graphNode.GUID;
                continue;
            }
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

        ConnectNodes();
    }

    //TODO: _nodes get corrupted so GUID arent the same as the ones in _dataCache, need to find a way to fix this
    private void ConnectNodes()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            var connections = _dataCache.nodeLinks.Where(x => x.baseNodeGuid == _nodes[i].GUID).ToList();
            if (_nodes[i].isEntryPoint)
            {
                var targetNodeGuid = connections.First(x=>x.baseNodeGuid == _dataCache.entryPointNodeGuid).targetNodeGuid;
                var targetNode = _nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(_nodes[i].outputContainer[0].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
            else
            {
                var nodeData = _dataCache.nodes.First(x => x.nodeInfos.GUID == _nodes[i].GUID).nodeInfos;
                int j = 0;
                int connectionID = 0;
                foreach (var outputElementData in nodeData.outputContainer.Children())
                {
                    
                    Port portData = outputElementData.Q<Port>();
                    var connectionsData = portData.connections.ToList();
                    if (portData == null)
                    {
                        
                        continue;
                    }
                    if (portData.connections.Count() > 0)
                    {
                        var targetNodeGuid = connections[connectionID].targetNodeGuid;
                        var targetNode = _nodes.First(x => x.GUID == targetNodeGuid);
                        LinkNodes(_nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                        j++;
                        connectionID++;
                    }
                    else
                    {
                        j++;
                    }


                }
            }
           
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var newEdge = new Edge
        {
            output = output,
            input = input
        };

        newEdge?.input.Connect(newEdge);
        newEdge?.output.Connect(newEdge);

        _targetGraphView.Add(newEdge);
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
        if(_dataCache.nodeLinks.Count > 0 && _dataCache.nodeLinks[0].baseNodeGuid == _dataCache.entryPointNodeGuid) 
        {
            _nodes.Find(x => x.isEntryPoint).GUID = _dataCache.nodeLinks[0].baseNodeGuid;
        }
        foreach (var node in _nodes)
        {
            if (node.isEntryPoint) continue;
            _edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}
