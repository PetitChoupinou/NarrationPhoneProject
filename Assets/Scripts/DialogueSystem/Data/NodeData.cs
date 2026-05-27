
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
    private bool _isSentBase;
    public bool isSentCurrent;

    public bool IsSentBase
    {
        get => _isSentBase;
        set
        {
            _isSentBase = value;
            isSentCurrent = value;
        }
    }
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
    public float timerSending;
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

    public int chosenChoiceID = -1;
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
public class ConditionPropertyNodeData : NodeData
{
    public ConditionPropertyNodeData(NodeData data)
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
public class UnlockNodeData : NodeData
{
    public UnlockNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
    public string characterID;
    public string dialogueID;
}

[Serializable]
public class ThinkingNodeData: NodeData
{
    public string text;
    public ThinkingNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
}

[Serializable]
public class BlockNodeData : NodeData
{
    public BlockNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
}

[Serializable]
public class NoteNodeData : NodeData
{

    public List<NoteData> notesData = new List<NoteData>();
    public NoteNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
}

[Serializable]
public class NewApplicationNodeData : NodeData
{

    public string applicationType;
    public NewApplicationNodeData(NodeData data)
    {
        nodeGUID = data.nodeGUID;
        nodeType = data.nodeType;
        position = data.position;
        outputs = data.outputs;
    }
}
[Serializable]

public class NewFileNodeData : NodeData
{

    public string fileName;
    public NewFileNodeData(NodeData data)
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


