using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public enum NodeType
{
    Start,
    Dialogue,
    Choice
}

public class DialogueGraphView : GraphView
{

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        CreateNode(NodeType.Start);
    }

    public BaseNode CreateNode(NodeType type)
    {
        BaseNode node = null;
        Port inputPort = null;
        Port outputPort = null;
        switch (type)
        {
            case NodeType.Start:
                node = new BaseNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    isEntryPoint = true,
                    capabilities = Capabilities.Movable | Capabilities.Selectable
                };
                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(100, 200, 150, 200));
                break;
            case NodeType.Dialogue:
                node = new DialogueNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    dialogueText = "New Dialogue"
                    
                };
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(Vector2.zero, BaseNode.defaultNodeSize));

                break;
            case NodeType.Choice:
                node = new ChoiceNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    dialogueText = "Choice"
                };
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                var button = new Button(() => ((ChoiceNode)node).AddChoicePort())
                {
                    text = "Add Choice"
                };
                node.titleContainer.Add(button);

                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Choice 0";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(Vector2.zero, BaseNode.defaultNodeSize));

                break;
        }
        AddElement(node);
        return node;


    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        foreach (Port port in ports)
        {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
            {
                compatiblePorts.Add(port);
            }
        }

        return compatiblePorts;
    }

}
