using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;
using static UnityEngine.Audio.GeneratorInstance;

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
                    capabilities = Capabilities.Movable | Capabilities.Selectable,
                    nodeType = NodeType.Start
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
                    dialogueText = "New Dialogue",
                    nodeType = NodeType.Dialogue
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
                    nodeType = NodeType.Choice,
                    graphView = this
                };
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                var button = new Button(() => ((ChoiceNode)node).AddChoicePort())
                {
                    text = "Add Choice"
                };
                node.titleContainer.Add(button);

                ((ChoiceNode)node).AddChoicePort(false);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(Vector2.zero, BaseNode.defaultNodeSize));

                break;
        }
        AddElement(node);
        return node;


    }

    public BaseNode CreateFromData(DialogueData dataCache, NodeData nodeData)
    {
        BaseNode node = CreateNode(nodeData.nodeInfos.nodeType);
        node.SetPosition(new Rect(nodeData.position, BaseNode.defaultNodeSize));
        switch (nodeData.nodeType)
        {
            case NodeType.Start:
                node.isEntryPoint = true;
                node.capabilities = Capabilities.Movable | Capabilities.Selectable;
                break;

            case NodeType.Dialogue:
                var nodeDialogue = node as DialogueNode;
                nodeDialogue.dialogueText = ((DialogueNode)nodeData.nodeInfos).dialogueText;
                break;

            case NodeType.Choice:
                var nodeChoice = node as ChoiceNode;
               // var nodePorts = dataCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.nodeInfos.GUID).ToList();
                var nodePorts = nodeData.nodeInfos.outputContainer.Query("connector").ToList();

                //nodePorts.ForEach(x => nodeChoice.AddChoicePort(true, ));
                for (int i = 0; i < nodePorts.Count; i++)
                {
                    
                    var choiceInfos = ((ChoiceNode)nodeData.nodeInfos).choices[i];
                    if (i <= nodeChoice.choices.Count - 1 && nodeChoice.choices[i] != null)
                    {
                        nodeChoice.UpdatePortName(i, choiceInfos.choiceText);
                        continue;
                    }

                    if (choiceInfos != null)
                    {
                        nodeChoice.AddChoicePort(true, choiceInfos.choiceText);
                    }
                }
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
