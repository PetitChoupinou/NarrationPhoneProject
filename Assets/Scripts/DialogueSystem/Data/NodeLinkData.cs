using System;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class NodeLinkData
{
    public string baseNodeGuid;
    public string portName;
    public string targetNodeGuid;
    public Port originPort;
}
#endif