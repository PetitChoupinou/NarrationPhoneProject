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
    Affinity,
    Condition,
    Set,
}

public enum Talker
{
    NPC,
    Player,
}

public class DialogueGraphView : GraphView
{
    public Blackboard blackboard;
    private GraphSearchWindow _searchWindow;
    private PropertyTypeWindow _propertyTypeWindow;
    public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();

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
        AddBlackboardTypeWindow(window);
    }

    private void AddSearchWindow(EditorWindow window)
    {
        _searchWindow = ScriptableObject.CreateInstance<GraphSearchWindow>();
        _searchWindow.Init(window, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    private void AddBlackboardTypeWindow(EditorWindow window)
    {
        _propertyTypeWindow = ScriptableObject.CreateInstance<PropertyTypeWindow>();
        _propertyTypeWindow.Init(window, this);
    }

    public void OpenBlackboardTypeWindow(Vector2 pos)
    {

        SearchWindow.Open(new SearchWindowContext(pos), _propertyTypeWindow);
       
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

                FloatField timeField = new FloatField
                {
                    label = "Time before sending"
                };
                dialogueNode.TimeField = timeField;
                timeField.RegisterValueChangedCallback(evt =>
                {
                    dialogueNode.timerSending = evt.newValue;
                });

                node.mainContainer.Add(timeField);

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
                    
                    timeField.enabledSelf = dialogueNode.isNPC;
                    
                });

                node.titleContainer.Add(talkerDialogue);
                dialogueNode.talkerField = talkerDialogue;
                dialogueNode.isNPC = true;

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
            case NodeType.Condition:
                node = new ConditionNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Condition
                };
                ConditionNode conditionNode = node as ConditionNode;
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);
                var conditionsListContainer = new VisualElement();
                var buttonCondition = new Button(() => AddCondition(new Condition(), conditionsListContainer, conditionNode))
                {
                    text = "Add Condition"
                };
                node.mainContainer.Add(buttonCondition);
                node.mainContainer.Add(conditionsListContainer);


                var truePort = node.GeneratePort(Direction.Output);
                truePort.portName = "True";
                node.outputContainer.Add(truePort);

                var falsePort = node.GeneratePort(Direction.Output);
                falsePort.portName = "False";
                node.outputContainer.Add(falsePort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));

                break;
            case NodeType.Set:
                node = new SetPropertyNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Set
                };
                SetPropertyNode setPropertyNode = node as SetPropertyNode;
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                var setPropertyRow = new VisualElement()
                {
                    name = "Row",
                    style = {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginBottom = 5
                }
                };
                DropdownField propertyDropdown = new DropdownField
                {
                    choices = GetProperties(),
                    value = "Property"
                };
                var valuePropertyField = new VisualElement();
                TextElement labelValue = new TextElement { text = " = ", style = { fontSize = 30 } };
                propertyDropdown.RegisterCallback<MouseDownEvent>(_ =>
                {
                    propertyDropdown.choices = GetProperties();
                });
                
                propertyDropdown.RegisterValueChangedCallback(evt =>
                {
                    ExposedProperty selectedProperty = exposedProperties.FirstOrDefault(p => p.Name == evt.newValue);
                    if (selectedProperty != null)
                    {
                        setPropertyNode.property = selectedProperty;
                        if(setPropertyRow.Contains(valuePropertyField)) setPropertyRow.Remove(valuePropertyField);
                        object fieldValue = setPropertyNode.Value != null ? setPropertyNode.Value : GetDefaultValue(selectedProperty.type);
                        valuePropertyField = CreateFieldForType(selectedProperty.type, fieldValue, value =>
                        {
                            setPropertyNode.Value = value;
                            
                        }, false);
                        setPropertyRow.Add(labelValue);
                        setPropertyNode.valueField = valuePropertyField;
                        setPropertyRow.Add(valuePropertyField);


                    }
                });

                setPropertyRow.Add(propertyDropdown);
                setPropertyNode.propertyField = propertyDropdown;
                
                node.mainContainer.Add(setPropertyRow);

                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));

                break;
        }
        if(type != NodeType.Start)
        {
            Toggle isSentToggle = new Toggle
            {
                text = "Is Sent"
            };
            isSentToggle.RegisterValueChangedCallback(evt =>
            {
                node.isSent = evt.newValue;
                
            });
            node.isSentToggle = isSentToggle;
            node.titleContainer.Add(isSentToggle);
        }
        
        AddElement(node);
        return node;


    }

    private void AddCondition(Condition data, VisualElement mainContainer, ConditionNode conditionNode)
    {
        Condition newCondition = new Condition
        {
            property = data.property,
            condition = data.condition,
            valueString = data.valueString,

        };
        newCondition.GetValueFromString();
        var row = new VisualElement()
        {
            style = {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginBottom = 5
                }
        };
        var propertyConditionField = new DropdownField
        {
            choices = GetProperties(),
            
            value = data.property != null ? data.property.Name : "Property"
        };
        var valueField = new VisualElement();

        //If loading a data
        if (data.property != null)
        {
            ExposedProperty property = data.property;
            valueField = CreateFieldForType(property.type, property.GetValue(), value =>
            {
                newCondition.Value = value;
            }, true, newCondition.Value);

        }

        propertyConditionField.RegisterCallback<MouseDownEvent>(_ =>
        {
            propertyConditionField.choices = GetProperties();
        });

        var conditionField = new DropdownField
        {
            choices = GetChoiceConditionsFromType(data.typeCondition),
            value = data.property != null ? data.GetConditionText(data.condition) : "=",
            style = {
                fontSize = 30
            }
        };
        var dropdownFieldStyle = conditionField.Q(className: "unity-base-popup-field__input").style;
        conditionField.Q(className: "unity-base-popup-field__text").style.fontSize = 20;
        dropdownFieldStyle.backgroundColor = Color.clear;
        conditionField.RegisterValueChangedCallback(evt =>
        {
            newCondition.condition = newCondition.GetConditionType(evt.newValue);
        });
        
        propertyConditionField.RegisterValueChangedCallback(evt =>
        {
            ExposedProperty selectedProperty = exposedProperties.FirstOrDefault(p => p.Name == evt.newValue);
            Type type = selectedProperty.type;
            if(row.Contains(conditionField)) row.Remove(conditionField);
            conditionField.value = "=";
            conditionField.choices = GetChoiceConditionsFromType(selectedProperty.type);
            newCondition.property = selectedProperty;
            row.Add(conditionField);
            row.Remove(valueField);
            valueField = CreateFieldForType(type, GetDefaultValue(selectedProperty.type), value =>
            {
                newCondition.Value = value;
            });
            row.Add(valueField);
        });

        Button deleteButton = new Button(() =>
        {
            conditionNode.conditions.Remove(newCondition);
            mainContainer.Remove(row);
        })
        { 
            text = "X",
            style = {
                backgroundColor = new Color(0.5f, 0, 0),
            }
        };


        conditionNode.conditions.Add(newCondition);
        row.Add(deleteButton);
        row.Add(propertyConditionField);
        
        row.Add(valueField);
        
        mainContainer.Add(row);
    }

    public List<string> GetChoiceConditionsFromType(Type type)
    {
        List<string> conditions = new List<string>() { "=", "!=", ">", "<", ">=", "<=" };
        switch (type)
        {
            case Type t when t == typeof(bool):
                conditions.Clear();
                conditions = new List<string>() { "=" };
                break;
            case Type t when t == typeof(string):
                conditions.Clear();
                conditions = new List<string>() { "=", "!=" };
                break;

        }
        return conditions;
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
                nodeDialogue.timerSending = nodeDialogueData.timerSending;
                nodeDialogue.TimeField.value = nodeDialogue.timerSending;
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
            case NodeType.Condition:
                var nodeCondition = node as ConditionNode;
                var nodeConditionData = nodeData as ConditionNodeData;
                foreach (var conditionData in nodeConditionData.conditions)
                {
                    AddCondition(conditionData, nodeCondition.mainContainer, nodeCondition);
                }
                break;
            case NodeType.Set:
                var nodeSetProperty = node as SetPropertyNode;
                var nodeSetPropertyData = nodeData as SetPropertyNodeData;
                ExposedProperty property = exposedProperties.FirstOrDefault(p => p.Name == nodeSetPropertyData.property.Name);
                nodeSetProperty.valueString = nodeSetPropertyData.valueString;
                if (property != null)
                {
                    nodeSetProperty.property = property;
                    nodeSetProperty.GetValueFromString();
                    nodeSetProperty.propertyField.value = property.Name;
                   
                }
                break;
        }
        node.isSentToggle.value = nodeData.isSent;

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
    public void ClearBlackboard()
    {
        exposedProperties.Clear();
        blackboard.Clear();
        blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
        AddPropertyToBlackboard(typeof(float), "Affinity");
    }
    public void AddPropertyToBlackboard(Type type, string propertyName = "")
    {
        ExposedProperty property = null;
        if (propertyName == "") propertyName = $"New {type.Name}";
        switch (type)
        {
            case Type t when t == typeof(bool):
                property = new ExposedProperty<bool>(propertyName, false);
                AddPropertyToBlackboard<bool>(property);
                break;
            case Type t when t == typeof(int):
                property = new ExposedProperty<int>(propertyName, 0);
                AddPropertyToBlackboard<int>(property);
                break;
            case Type t when t == typeof(float):
                property = new ExposedProperty<float>(propertyName, 0);
                AddPropertyToBlackboard<float>(property);
                break;
            case Type t when t == typeof(string):
                property = new ExposedProperty<string>(propertyName, "");
                AddPropertyToBlackboard<string>(property);
                break;

            default:
                break;
        }
    }

    public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    {
        Type type = exposedProperty.type;
        switch (type)
        {
            case Type t when t == typeof(bool):
                AddPropertyToBlackboard<bool>(exposedProperty);
                break;
            case Type t when t == typeof(int):
                AddPropertyToBlackboard<int>(exposedProperty);
                break;
            case Type t when t == typeof(float):
                AddPropertyToBlackboard<float>(exposedProperty);
                break;
            case Type t when t == typeof(string):
                AddPropertyToBlackboard<string>(exposedProperty);
                break;

            default:
                break;
        }
    }


    public void AddPropertyToBlackboard<T>(ExposedProperty exposedProperty)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("Property"));
        if (exposedProperty == null) return;
        var localPropertyName = exposedProperty.Name;
        var localPropertyValue = exposedProperty.GetValue();
        string newName = localPropertyName;
        int index = 0;
        while (exposedProperties.Any(p => p.Name == newName)){
            index++;
            newName = $"{localPropertyName}_{index}";
        }
        localPropertyName = newName;
        ExposedProperty property = new ExposedProperty<T>(localPropertyName, (T)localPropertyValue);
        exposedProperties.Add(property);

        var container = new VisualElement();
        
        string typeName = typeof(T).Name;
        //var blackboardField = new BlackboardField { text = $"New {typeName}" , typeText = typeName };
        var blackboardField = new BlackboardField { text = localPropertyName, typeText = typeName, capabilities = Capabilities.Selectable | Capabilities.Renamable};
        /*Color fieldColor = typeof(T) switch
        {
            Type t when t == typeof(bool) => Color.red,
            Type t when t == typeof(int) => new Color(0.2f, 0.6f, 1f),
            Type t when t == typeof(float) => new Color(0.4f, 0.9f, 0.4f),
            Type t when t == typeof(string) => new Color(1f, 0.8f, 0.2f),
            _ => Color.white
        };
        */
        blackboardField.Q(name: "node-border").style.backgroundImage = null;
        blackboardField.AddToClassList($"pill-{typeName.ToLower()}");

        //blackboardField.Q(className: "node-border").style.backgroundColor = fieldColor;
        container.Add(blackboardField);
        if (property.Name == "Affinity")
        {
            container.enabledSelf = false;
        }
        else 
        { 
            var deleteButton = new Button(() =>
            {
                exposedProperties.Remove(property);
                blackboard.Remove(container);
            })
            { 
                text = "X",
                style = {
                    backgroundColor = new Color(0.5f, 0, 0),
                }
            };
            blackboardField.Add(deleteButton);
            var propertyField = CreateFieldForType(typeof(T), (T)localPropertyValue, value => {

                var propertyIndex = exposedProperties.FindIndex(p => p.Name == property.Name);
                exposedProperties[propertyIndex].SetValue(value);
            });
            var row = new BlackboardRow(blackboardField, propertyField);
            container.Add(row);
        }
        blackboard.Add(container);
        
    }


    private VisualElement CreateFieldForType(Type type, object value, Action<object> onValueChanged, bool doesNeedLabel = true, object baseValue = null)
    {
        switch (type)
        {
            case Type t when t == typeof(bool):
                var toggle = new Toggle(){
                    
                    value = baseValue != null ? (bool)baseValue : (bool)value,
                    
                };
                if(doesNeedLabel) toggle.label = "Value";
                toggle.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                return toggle;

            case Type t when t == typeof(int):
                var intField = new IntegerField()
                {
                    value = baseValue != null ? (int)baseValue : (int)value
                };
                if (doesNeedLabel) intField.label = "Value";
                intField.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                return intField;

            case Type t when t == typeof(float):
                var floatField = new FloatField()
                {
                    value = baseValue != null ? (float)baseValue : (float)value,
                   
                };
                if (doesNeedLabel) floatField.label = "Value";
                floatField.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                return floatField;

            case Type t when t == typeof(string):
                var textField = new TextField()
                {
                    value = baseValue != null ? (string)baseValue : (string)value
                };
                if (doesNeedLabel) textField.label = "Value";
                textField.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                return textField;

            default:
                return new UnityEngine.UIElements.Label($"Type non supporté: {type.Name}");
        }
    }


    private List<string> GetProperties()
    {
        List<string> properties = new List<string>();
        foreach(var property in exposedProperties)
        {
            properties.Add(property.Name);
        }
        return properties;
    }

    private object GetDefaultValue(Type type)
    {
        switch (type)
        {
            case Type t when t == typeof(bool):
                return false;
            case Type t when t == typeof(int):
                return 0;
            case Type t when t == typeof(float):
                return 0f;
            case Type t when t == typeof(string):
                return "";
            default:
                return null;
        }
    }
}


