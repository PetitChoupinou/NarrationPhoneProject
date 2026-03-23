using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public enum NodeType
{
    Start,
    Dialogue,
    Choice,
    Affinity
}

public enum Talker
{
    NPC,
    Player,
}

public class DialogueGraphView : GraphView
{
    private GraphSearchWindow _searchWindow;

    public DialogueGraphView(EditorWindow window)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        CreateNode(NodeType.Start, new Vector2(100, 200));
        AddSearchWindow(window);
    }

    private void AddSearchWindow(EditorWindow window)
    {
        _searchWindow = ScriptableObject.CreateInstance<GraphSearchWindow>();
        _searchWindow.Init(window, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public BaseNode CreateNode(NodeType type, Vector2 position)
    {
        BaseNode node = null;
        Port inputPort = null;
        Port outputPort = null;
        styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        switch (type)
        {
            case NodeType.Start:
                node = new BaseNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    isEntryPoint = true,
                    capabilities = Capabilities.Selectable,
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
                DialogueNode dialogueNode = node as DialogueNode;
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                var textFieldDialogue = new TextField
                {
                    value = "New Dialogue",
                    multiline = true
                };
                textFieldDialogue.RegisterValueChangedCallback(evt => dialogueNode.dialogueText = evt.newValue);
                node.mainContainer.Add(textFieldDialogue);
                dialogueNode.textField = textFieldDialogue;

                var talkerDialogue = new DropdownField
                {
                    choices = Enum.GetNames(typeof(Talker)).ToList(),
                };
                talkerDialogue.value = talkerDialogue.choices[0];
                talkerDialogue.RegisterValueChangedCallback(evt =>
                {
                    if (Enum.TryParse<Talker>(evt.newValue, out var talker))
                    {
                        dialogueNode.isNPC = talker == Talker.NPC;
                    }
                });
                node.titleContainer.Add(talkerDialogue);
                dialogueNode.talkerField = talkerDialogue;

                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));

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
                var textFieldChoice = new TextField
                {
                    value = "New Dialogue",
                    multiline = true
                };
                
                ChoiceNode choiceNode = node as ChoiceNode;
                textFieldChoice.RegisterValueChangedCallback(evt => choiceNode.dialogueText = evt.newValue);
                node.mainContainer.Add(textFieldChoice);
                choiceNode.textField = textFieldChoice;
                
                var button = new Button(() => ((ChoiceNode)node).AddChoicePort())
                {
                    text = "Add Choice"
                };
                node.titleContainer.Add(button);

                ((ChoiceNode)node).AddChoicePort(false);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));

                break;
            case NodeType.Affinity:
                node = new AffinityNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Affinity
                };
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);
                AffinityNode affinityNode = node as AffinityNode;
                var affinityField = new FloatField
                {
                    label = "Affinity Gain:",
                    value = 0,
                    
                };
                affinityField.RegisterValueChangedCallback(evt => affinityNode.affinityGain = evt.newValue);
                affinityNode.affinityField = affinityField;
                node.mainContainer.Add(affinityField);

                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));

                break;
        }

        AddElement(node);
        return node;


    }

    public BaseNode CreateFromData(DialogueData dataCache, NodeData nodeData)
    {
        BaseNode node = CreateNode(nodeData.nodeType, nodeData.position);
        node.GUID = nodeData.nodeGUID;
        switch (nodeData.nodeType)
        {
            case NodeType.Start:
                node.isEntryPoint = true;
                node.capabilities = Capabilities.Selectable;
                break;

            case NodeType.Dialogue:
                var nodeDialogue = node as DialogueNode;
                var nodeDialogueData = nodeData as DialogueNodeData;
                nodeDialogue.dialogueText = nodeDialogueData.dialogueText;
                nodeDialogue.UpdateTextFieldValue();
                nodeDialogue.isNPC = nodeDialogueData.isNPC;
                nodeDialogue.UpdateTalkerField();
                break;

            case NodeType.Choice:
                var nodeChoice = node as ChoiceNode;
                var nodeChoiceData = nodeData as ChoiceNodeData;
                nodeChoice.dialogueText = nodeChoiceData.dialogueText;
                nodeChoice.UpdateTextFieldValue();
                var nodeOutputs = nodeData.outputs;

                for (int i = 0; i < nodeOutputs.Count; i++)
                {
                    
                    if (i <= nodeChoice.choices.Count - 1 && nodeChoice.choices[i] != null)
                    {
                        nodeChoice.UpdatePort(i, nodeOutputs[i].portValue);
                        continue;
                    }

                    nodeChoice.AddChoicePort(true, nodeOutputs[i].portValue);   
                }
                break;
            case NodeType.Affinity:
                var nodeAffinity = node as AffinityNode;
                var nodeAffinityData = nodeData as AffinityNodeData;
                nodeAffinity.affinityGain = nodeAffinityData.affinityGain;
                nodeAffinity.UpdateAffinityField();
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

    public BaseNode GetStarterNode()
    {
        return nodes.FirstOrDefault(node => (node as BaseNode).isEntryPoint) as BaseNode;
    }
}
