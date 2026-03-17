using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseNode : Node
{
    public static readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public string GUID;
    public NodeType nodeType;

    public bool isEntryPoint = false;

    public Port GeneratePort(Direction direction, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }



}
