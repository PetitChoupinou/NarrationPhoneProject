
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


[Serializable]
public class NodeData
{
    public string nodeGUID;
    public NodeType nodeType;
    public Vector2 position;
    public List<OutputData> outputs = new List<OutputData>();

}
[Serializable]
public class DialogueNodeData : NodeData
{
    public string dialogueText;
    public DialogueNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
}
[Serializable]
public class ChoiceNodeData : DialogueNodeData
{
    public ChoiceNodeData(NodeData data) : base(data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
}
[Serializable]
public class OutputData
{
    public string portValue;
    public string targetNodeGuid;
}
