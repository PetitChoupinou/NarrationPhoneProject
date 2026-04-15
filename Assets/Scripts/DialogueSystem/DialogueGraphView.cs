using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public enum NodeType
{
    Start,
    Dialogue,
    Choice,
    Affinity,
    Condition,
    Note,
    Set,
    Unlock,
    Thinking,
    Block
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
    public GlobalPropertiesData globalPropertiesData;
    private List<ExposedProperty> blackBoardProperty = new List<ExposedProperty>();
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

        globalPropertiesData = Resources.Load<GlobalPropertiesData>("GlobalPropertiesData");
        if (globalPropertiesData == null)
        {
            globalPropertiesData = ScriptableObject.CreateInstance<GlobalPropertiesData>();
            AssetDatabase.CreateAsset(globalPropertiesData, $"Assets/Resources/GlobalPropertiesData.asset");
        }
        
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
                node.SetPosition(new Rect(250, 200, 150, 200));
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
                node = new ConditionPropertyNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Condition
                };
                ConditionPropertyNode conditionNode = node as ConditionPropertyNode;
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
                    ExposedProperty selectedProperty = FindPropertyByName(evt.newValue);
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
            case NodeType.Unlock:
                node = new UnlockNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Unlock
                };
                UnlockNode unlockNode = node as UnlockNode;
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);
                CharacterSheet character = null;
                
                ObjectField characterUnlockField = new ObjectField
                {
                    label = "Character",
                    objectType = typeof(CharacterSheet)
                };
                unlockNode.characterField = characterUnlockField;
                DropdownField dialogueUnlockField = new DropdownField
                {
                    label = "Dialogue",
                    choices = new List<string> {},
                    value = ""
                };
                unlockNode.dialogueField = dialogueUnlockField;
                dialogueUnlockField.RegisterValueChangedCallback(evt =>
                {
                    if(character != null && evt.newValue != "")
                    {
                        unlockNode.IDDialogue = character.Dialogues.FirstOrDefault(d => d.name == evt.newValue).name;

                    }
                });
                characterUnlockField.RegisterValueChangedCallback(evt =>
                {
                    character = evt.newValue as CharacterSheet;
                    if (unlockNode.IDCharacter != character.Name && unlockNode.IDCharacter != "")
                    {
                        unlockNode.IDDialogue = "";
                    }
                    unlockNode.IDCharacter = character.Name;
                    
                    if (node.mainContainer.Contains(dialogueUnlockField)) node.mainContainer.Remove(dialogueUnlockField);
                    dialogueUnlockField.choices.Clear();
                    foreach (var dialogue in character.Dialogues)
                    {
                        if (dialogue == null) continue;
                        dialogueUnlockField.choices.Add(dialogue.name);
                    }
                    if (string.IsNullOrEmpty(unlockNode.IDDialogue) || unlockNode.IDDialogue == "Dialogue")
                    {
                        if (character.Dialogues.Count() > 0 && character.Dialogues[0] != null)
                        {
                            
                            unlockNode.IDDialogue = dialogueUnlockField.choices[0];
                            dialogueUnlockField.value = dialogueUnlockField.choices[0];
                        }
                        else
                        {
                            dialogueUnlockField.value = "Dialogue";
                        }
                        
                    }
                    else
                    {
                        var dialogue = character.Dialogues.FirstOrDefault(x => x.name == unlockNode.IDDialogue);
                        dialogueUnlockField.value = dialogue != null ? dialogue.name : "Dialogue";
                    }
                    node.mainContainer.Add(dialogueUnlockField);
                });
                node.mainContainer.Add(characterUnlockField);
                node.mainContainer.Add(dialogueUnlockField);


                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));

                break;
            case NodeType.Thinking:
                node = new ThinkingNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    text = "New Thought",
                    nodeType = NodeType.Thinking
                };
                ThinkingNode thinkingNode = node as ThinkingNode;

                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                var textFieldThink = new TextField
                {
                    value = "New Thought",
                    multiline = true
                };
                textFieldThink.RegisterValueChangedCallback(evt => thinkingNode.text = evt.newValue);
                node.mainContainer.Add(textFieldThink);
                thinkingNode.textField = textFieldThink;
                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));
                break;
            case NodeType.Note:
                node = new NoteNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Note
                };
                NoteNode noteNode = node as NoteNode;

                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

                var buttonNote = new Button(() => AddNoteUpdate(noteNode))
                {
                    text = "Create/Update Note"
                };
                node.mainContainer.Add(buttonNote);
                outputPort = node.GeneratePort(Direction.Output);
                outputPort.portName = "Next";
                node.outputContainer.Add(outputPort);

                node.RefreshExpandedState();
                node.RefreshPorts();
                node.SetPosition(new Rect(position, BaseNode.defaultNodeSize));
                break;
            case NodeType.Block:
                node = new BlockNode
                {
                    GUID = Guid.NewGuid().ToString(),
                    title = type.ToString(),
                    nodeType = NodeType.Block
                };
                inputPort = node.GeneratePort(Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);

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

    private void AddNoteUpdate(NoteNode node, string name = "Name", string text = "Text")
    {
        NoteData noteData = new NoteData(name, text);
        VisualElement container = new VisualElement()
        {
            style = {
                    flexDirection = FlexDirection.Row,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f),
                    marginBottom = 3,
                    marginLeft = 3,
                    marginRight = 3,
                    marginTop = 3,
                },
            
            
        };

        VisualElement valueContainer = new VisualElement()
        {
            style = {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                }
        };
        TextElement nameElement = new TextElement
        {
            text = "Name:",
            style = { fontSize = 10}
        };
        TextField nameField = new TextField
        {
            value = name
        };
        nameField.RegisterValueChangedCallback(evt =>
        {
            noteData.data.title = evt.newValue;
        });
        TextElement textElement = new TextElement
        {
            text = "Content:",
            style = { fontSize = 10}
        };
        TextField textField = new TextField
        {
            multiline = true,
            
            value = text
        };
        textField.RegisterValueChangedCallback(evt => {
            noteData.data.content = evt.newValue;
        });

        Button deleteButton = new Button(() =>
        {
            if (node.noteDatas.Contains(noteData)) node.noteDatas.Remove(noteData);
            node.mainContainer.Remove(container);
        })
        {
            text = "X",
            style = {
                backgroundColor = new Color(0.5f, 0, 0),
                maxHeight = 20,
                maxWidth = 20,
                alignSelf = Align.Center
            }
        };
        valueContainer.Add(nameElement);
        valueContainer.Add(nameField);
        valueContainer.Add(textElement);
        valueContainer.Add(textField);
        container.Add(valueContainer);
        container.Add(deleteButton);
        node.mainContainer.Add(container);
        node.noteDatas.Add(noteData);
    }

    private void AddCondition(Condition data, VisualElement mainContainer, ConditionPropertyNode conditionNode)
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
            
            value = newCondition.property != null ? newCondition.property.Name : "Property"
        };
        

        var valueField = new VisualElement();

        //If loading a data
        if (data.property != null)
        {
            if (globalPropertiesData.globalProperties.FirstOrDefault(p => p.Name == newCondition.property?.Name) == null)
            {
                propertyConditionField.style.backgroundColor = new Color(0.5f, 0, 0, 1);
                propertyConditionField.value = "Invalid Property";
            }
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
            propertyConditionField.style.backgroundColor = new Color(0.5f, 0, 0, 0);
            ExposedProperty selectedProperty = FindPropertyByName(evt.newValue);
            Type type = selectedProperty.type;
            if (row.Contains(conditionField)) row.Remove(conditionField);
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
        row.Add(conditionField);
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
                var nodeCondition = node as ConditionPropertyNode;
                var nodeConditionData = nodeData as ConditionPropertyNodeData;
                foreach (var conditionData in nodeConditionData.conditions)
                {
                    AddCondition(conditionData, nodeCondition.mainContainer, nodeCondition);
                }
                break;
            case NodeType.Set:
                var nodeSetProperty = node as SetPropertyNode;
                var nodeSetPropertyData = nodeData as SetPropertyNodeData;
                ExposedProperty property = FindPropertyByName(nodeSetPropertyData.property.Name);
                nodeSetProperty.valueString = nodeSetPropertyData.valueString;
                if (property != null)
                {
                    nodeSetProperty.property = property;
                    nodeSetProperty.GetValueFromString();
                    nodeSetProperty.propertyField.value = property.Name;
                   
                }
                break;
            case NodeType.Unlock:
                var nodeUnlock = node as UnlockNode;
                var nodeUnlockData = nodeData as UnlockNodeData;
                nodeUnlock.IDCharacter = nodeUnlockData.characterID;
                nodeUnlock.IDDialogue = nodeUnlockData.dialogueID;
                nodeUnlock.UpdateCharacterField();
                nodeUnlock.UpdateDialogueField();
                break;
            case NodeType.Thinking:
                var nodeThinking = node as ThinkingNode;
                var nodeThinkingData = nodeData as ThinkingNodeData;
                nodeThinking.text = nodeThinkingData.text;
                nodeThinking.UpdateTextFieldValue();
                break;
            case NodeType.Note:
                var nodeNote = node as NoteNode;
                var nodeNoteData = nodeData as NoteNodeData;
                foreach (var noteData in nodeNoteData.notesData)
                {
                    AddNoteUpdate(nodeNote, noteData.data.title, noteData.data.content);
                }
                break;
            case NodeType.Block:
                var nodeBlock = node as BlockNode;
                var blockNodeData = nodeData as BlockNodeData;
                break;
        }
        node.isSentToggle.value = nodeData.IsSentBase;
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
        blackboard.Clear();
        blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
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
        var propertyName = exposedProperty.Name;
        var propertyValue = exposedProperty.GetValue();
        string newName = propertyName;
        int index = 0;
        

        while (blackBoardProperty.Any(p => p.Name == newName))
        {
            index++;
            newName = $"{propertyName}_{index}";
        }
        propertyName = newName;
        ExposedProperty property = new ExposedProperty<T>(propertyName, (T)propertyValue);
        
        if (!globalPropertiesData.globalProperties.Contains(exposedProperty))
        {
            globalPropertiesData.globalProperties.Add(exposedProperty);
        }
        var container = new VisualElement();
        
        string typeName = typeof(T).Name;
        var blackboardField = new BlackboardField { text = propertyName, typeText = typeName, capabilities = Capabilities.Selectable | Capabilities.Renamable};
        
        blackboardField.Q(name: "node-border").style.backgroundImage = null;
        blackboardField.AddToClassList($"pill-{typeName.ToLower()}");

        container.Add(blackboardField);

        var deleteButton = new Button(() =>
        {
            var foundProperty = globalPropertiesData.globalProperties.FirstOrDefault(x => x.Name == exposedProperty.Name);
            if(foundProperty != null)
            {
                globalPropertiesData.globalProperties.Remove(foundProperty);

            }

            blackBoardProperty.Remove(property);
            blackboard.Remove(container);
        })
        {
            text = "X",
            style = {
                    backgroundColor = new Color(0.5f, 0, 0),
                }
        };
        blackboardField.Add(deleteButton);
        VisualElement containerValue = new VisualElement();
        var propertyField = CreateFieldForType(typeof(T), (T)propertyValue, value => {
            {
                var foundProperty = FindPropertyByName(exposedProperty.Name);
                foundProperty.SetValue(value);
            }

        });
        containerValue.Add(propertyField);

        var row = new BlackboardRow(blackboardField, containerValue);

        container.Add(row);
        blackboard.Add(container);
        blackBoardProperty.Add(property);

    }


    private VisualElement CreateFieldForType(Type type, object value, Action<object> onValueChanged, bool doesNeedLabel = true, object baseValue = null)
    {
        switch (type)
        {
            case Type t when t == typeof(bool):
                var toggle = new Toggle(){

                    value = baseValue != null ? (bool)baseValue : (bool)value
                };
                if(doesNeedLabel) toggle.label = "Value";
                toggle.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                onValueChanged(toggle.value);
                return toggle;

            case Type t when t == typeof(int):
                var intField = new IntegerField()
                {
                    value = baseValue != null ? (int)baseValue : (int)value
                };
                if (doesNeedLabel) intField.label = "Value";
                intField.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                onValueChanged(intField.value);
                return intField;

            case Type t when t == typeof(float):
                var floatField = new FloatField()
                {
                    value = baseValue != null ? (float)baseValue : (float)value
                };
                if (doesNeedLabel) floatField.label = "Value";
                floatField.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                onValueChanged(floatField.value);
                return floatField;

            case Type t when t == typeof(string):
                var textField = new TextField()
                {
                    value = baseValue != null ? (string)baseValue : (string)value
                };
                if (doesNeedLabel) textField.label = "Value";
                textField.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
                onValueChanged(textField.value);
                return textField;

            default:
                return new UnityEngine.UIElements.Label($"Type non supporté: {type.Name}");
        }
    }


    private List<string> GetProperties()
    {
        List<string> properties = new List<string>();
        foreach (var property in globalPropertiesData.globalProperties)
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

    public void UpdateGlobalVariables()
    {
        List<ExposedProperty> properties = new List<ExposedProperty>();
        properties.AddRange(globalPropertiesData.globalProperties);
        foreach (var property in properties)
        {
            AddPropertyToBlackboard(property);
        }
    }

    public ExposedProperty FindPropertyByName(string propertyName)
    {
        ExposedProperty property = null;
        property = globalPropertiesData.FindProperty(propertyName);
        return property;
    }
}


