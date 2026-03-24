using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueData : ScriptableObject
{
    [SerializeReference]
    public List<NodeData> nodes = new List<NodeData>();
    public List<ExposedProperty> properties = new List<ExposedProperty>();
    public string entryPointNodeGuid = "";
    public string characterID = "Sasha";
}
