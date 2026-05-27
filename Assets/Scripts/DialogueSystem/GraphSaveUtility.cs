using System.Collections.Generic;
using System.Linq;
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
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues")){
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
        }

        if (AssetDatabase.AssetPathExists($"Assets/Resources/Dialogues/{fileName}.asset"))
        {
            var asset = AssetDatabase.LoadAssetAtPath<DialogueData>($"Assets/Resources/Dialogues/{fileName}.asset");
            EditorUtility.CopySerializedIfDifferent(dialogueData, asset);
            asset.name = fileName;
            EditorUtility.SetDirty(asset);
        }
        else
        {
            dialogueData.name = fileName;
            AssetDatabase.CreateAsset(dialogueData, $"Assets/Resources/Dialogues/{fileName}.asset");
        }
        
        AssetDatabase.SaveAssets();

    }

   

    public void SaveBlackboard(DialogueData dialogueData)
    {
        /*foreach (var property in _targetGraphView.globalPropertiesData.globalProperties)
        {
            dialogueData.properties.Add(property);
            
        }
*/


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
                var conditionNode = node as ConditionPropertyNode;
                ConditionPropertyNodeData conditionNodeData = new ConditionPropertyNodeData(data);
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
            case NodeType.Unlock:
                var unlockNode = node as UnlockNode;
                UnlockNodeData unlockNodeData = new UnlockNodeData(data);
                unlockNodeData.characterID = unlockNode.IDCharacter;
                unlockNodeData.dialogueID = unlockNode.IDDialogue;
                unlockNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = unlockNodeData;
                break;
            case NodeType.Thinking:
                var thinkingNode = node as ThinkingNode;
                ThinkingNodeData thinkingNodeData = new ThinkingNodeData(data);
                thinkingNodeData.text = thinkingNode.text;
                thinkingNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = thinkingNodeData;
                break;
            case NodeType.Note:
                var noteNode = node as NoteNode;
                NoteNodeData noteNodeData = new NoteNodeData(data);
                foreach(var noteData in noteNode.noteDatas)
                {
                    noteNodeData.notesData.Add(noteData);
                }
                noteNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = noteNodeData;
                break;
            case NodeType.Block:
                var blockNode = node as BlockNode;
                BlockNodeData blockNodeData = new BlockNodeData(data);
                data = blockNodeData;
                break;
            case NodeType.NewApplication:
                var newAppNode = node as NewApplicationNode;
                NewApplicationNodeData newAppNodeData = new NewApplicationNodeData(data);
                newAppNodeData.applicationType = newAppNode.applicationType.ToString();
                newAppNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = newAppNodeData;
                break;
            case NodeType.NewFile:
                var newFileNode = node as NewFileNode;
                NewFileNodeData newFileNodeData = new NewFileNodeData(data);
                newFileNodeData.fileName = newFileNode.fileName;
                newFileNodeData.outputs.Add(CreateOutputData(connectedPorts, node, "Next"));
                data = newFileNodeData;
                break;
            default:
                break;
        }
        data.IsSentBase = node.isSent;
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
        _dataCache = Resources.Load<DialogueData>($"Dialogues/{fileName}");
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
        //_targetGraphView.ClearBlackboard();
        /*foreach (var property in _dataCache.properties)
        {
            if(property.Name == "Affinity")
            {
                continue;

            }
            _targetGraphView.AddPropertyToBlackboard(property);
        }*/
    }
    private void ClearGraph()
    {
        
       
        _nodes.Find(x => x.isEntryPoint).GUID = _dataCache.entryPointNodeGuid;
        foreach (var node in _nodes)
        {
            if (node.isEntryPoint) continue;
            _edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}
