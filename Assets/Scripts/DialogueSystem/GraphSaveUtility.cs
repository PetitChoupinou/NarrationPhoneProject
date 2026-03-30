using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEditor.Rendering.CameraUI;

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
        SaveBlackboard(dialogueData);
        var connectedPorts = _edges.Where(edge => edge.input.node != null).ToArray();
        
        foreach(var graphNode in _nodes)
        {
            if (graphNode.isEntryPoint)
            {
                dialogueData.entryPointNodeGuid = graphNode.GUID;
            }
            dialogueData.nodes.Add(CreateNodeData(graphNode, connectedPorts));
        }
        if (!AssetDatabase.IsValidFolder("Assets/Resources")){
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        AssetDatabase.CreateAsset(dialogueData, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

    }

    public void SaveBlackboard(DialogueData data)
    {
        data.properties.AddRange(_targetGraphView.exposedProperties);
    }

    public NodeData CreateNodeData(BaseNode node, Edge[] connectedPorts)
    {
        NodeData data = new NodeData
        {
            position = node.GetPosition().position,
            nodeType = node.nodeType,
            nodeGUID = node.GUID,

        };


        switch (node.nodeType)
        {
            case NodeType.Start:
                data.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                break;
            case NodeType.Dialogue:
                var dialogueNode = node as DialogueNode;
                DialogueNodeData dialogueNodeData = new DialogueNodeData(data);

                dialogueNodeData.dialogueText = dialogueNode.dialogueText;

                dialogueNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                dialogueNodeData.isNPC = dialogueNode.isNPC;
                dialogueNodeData.timerSending = dialogueNode.timerSending;
                data = dialogueNodeData;
                break;
            case NodeType.Choice:
                var choiceNode = node as ChoiceNode;
                ChoiceNodeData choiceNodeData = new ChoiceNodeData(data);
                var nodePorts = choiceNode.outputContainer.Query("connector").ToList();
                for (int i = 0; i < nodePorts.Count; i++)
                {

                    var choiceInfos = choiceNode.choices[i];
                    
                    if (choiceInfos != null)
                    {
                        choiceNodeData.outputs.Add(CreateOutputData(connectedPorts, node, choiceInfos.choiceText));
                    }
                }
                choiceNodeData.dialogueText = choiceNode.dialogueText;
                data = choiceNodeData;
                break;
            case NodeType.Affinity:
                var affinityNode = node as AffinityNode;
                AffinityNodeData affinityNodeData = new AffinityNodeData(data);
                affinityNodeData.affinityGain = affinityNode.affinityGain;
                affinityNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = affinityNodeData;
                break;
            case NodeType.Condition:
                var conditionNode = node as ConditionNode;
                ConditionNodeData conditionNodeData = new ConditionNodeData(data);
                conditionNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "True"));
                conditionNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "False"));
                conditionNodeData.conditions = conditionNode.conditions;
                data = conditionNodeData;
                break;
            case NodeType.Set:
                var setPropertyNode = node as SetPropertyNode;
                SetPropertyNodeData setPropertyNodeData = new SetPropertyNodeData(data);
                setPropertyNodeData.property = setPropertyNode.property;
                setPropertyNodeData.valueString = setPropertyNode.valueString;
                setPropertyNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = setPropertyNodeData;
                break;
            default:
                break;
        }
        data.isSent = node.isSent;
        return data;
    }

    public OutputData CreateOutputData(Edge[] connectedPorts, BaseNode node, string outputName)
    {
        BaseNode nextNode = null;
        string nextNodeGUID = "";
        var outputs = connectedPorts.Where(x => x.output.node == node).ToList();
        var nextNodeEdge = outputs.FirstOrDefault(x => x.output.portName == outputName);

        if (nextNodeEdge != null)
        {
            nextNode = nextNodeEdge.input.node as BaseNode;
            nextNodeGUID = nextNode.GUID;
            
        }

        return new OutputData
        {
            portValue = outputName,
            targetNodeGuid = nextNodeGUID
        };
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

        LoadBlackboard();

        CreateNodes();

        ConnectNodes();
    }

    private void ConnectNodes()
    {
        /*for (int i = 0; i < _nodes.Count; i++)
        {
            var connections = _dataCache.nodeLinks.Where(x => x.baseNodeGuid == _nodes[i].GUID).ToList();
            if (_nodes[i].isEntryPoint)
            {
                var targetNodeGuid = connections.First(x => x.baseNodeGuid == _dataCache.entryPointNodeGuid).targetNodeGuid;
                var targetNode = _nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(_nodes[i].outputContainer[0].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
            else
            {
                var nodeData = _dataCache.nodes.First(x => x.nodeGUID == _nodes[i].GUID);
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

        }*/
        
        for (int i = 0; i < _nodes.Count; i++)
        {
            int j = 0;
            var nodeData = _dataCache.nodes.First(x => x.nodeGUID == _nodes[i].GUID);
            
            foreach (var output in _nodes[i].outputContainer.Children())
            {

                Port port = output.Q<Port>();
                
                if (port == null)
                {
                    continue;
                }

                var targetNodeGUID = nodeData.outputs[j].targetNodeGuid;
                if (targetNodeGUID != "")
                {
                    var targetNode = _nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodes(_nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                }


                j++;


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
            if(nodeData.nodeGUID == _dataCache.entryPointNodeGuid) { continue; }
            var tempNode = _targetGraphView.CreateFromData(_dataCache, nodeData);

        }
    }

    private void LoadBlackboard()
    {
        _targetGraphView.ClearBlackboard();
        Debug.Log("Properties count = " + _dataCache.properties.Count);
        foreach (var property in _dataCache.properties)
        {
            if(property.Name == "Affinity")
            {
                continue;

            }
            _targetGraphView.AddPropertyToBlackboard(property);
        }
    }
    private void ClearGraph()
    {
        /*if(_dataCache.nodeLinks.Count > 0 && _dataCache.nodeLinks[0].baseNodeGuid == _dataCache.entryPointNodeGuid) 
        {
            _nodes.Find(x => x.isEntryPoint).GUID = _dataCache.nodeLinks[0].baseNodeGuid;
        }
        foreach (var node in _nodes)
        {
            if (node.isEntryPoint) continue;
            _edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }*/
       
        _nodes.Find(x => x.isEntryPoint).GUID = _dataCache.entryPointNodeGuid;
        foreach (var node in _nodes)
        {
            if (node.isEntryPoint) continue;
            _edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}
