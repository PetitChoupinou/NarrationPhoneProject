
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
    public bool isSent;

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
    public bool isNPC;
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
public class AffinityNodeData : NodeData
{
    public AffinityNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
    public float affinityGain;
}

[Serializable]
public class ConditionNodeData : NodeData
{
    public ConditionNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
    public List<Condition> conditions = new List<Condition>();
}
[Serializable]
public class SetPropertyNodeData : NodeData
{
    public SetPropertyNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
    [SerializeReference]
    public ExposedProperty property;
    public string valueString;
}
[Serializable]
public class OutputData
{
    public string portValue;
    public string targetNodeGuid;
}


