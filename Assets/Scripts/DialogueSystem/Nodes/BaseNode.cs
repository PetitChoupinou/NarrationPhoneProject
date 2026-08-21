#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseNode : Node
{
    public static readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public string GUID;
    public NodeType nodeType;

    public bool isEntryPoint = false;
    public bool isSent = false;

    public Toggle isSentToggle;

    public Port GeneratePort(Direction direction, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }



}
#endif