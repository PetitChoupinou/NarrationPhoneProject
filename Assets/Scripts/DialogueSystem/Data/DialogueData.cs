using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueData : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
    public string entryPointNodeGuid = "";
}
