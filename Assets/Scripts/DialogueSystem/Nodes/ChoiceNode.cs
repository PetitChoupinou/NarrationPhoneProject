#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

public class ChoiceNode : DialogueNode
{
    public DialogueGraphView graphView;
    public List<ChoiceInfos> choices = new List<ChoiceInfos>();

    public void AddChoicePort(bool isDeletable = true, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(Direction.Output);
        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var choiceCount = outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {choiceCount+1}" : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName,
            multiline = true
        };
        textField.RegisterValueChangedCallback(evt => 
        {
            generatedPort.portName = evt.newValue;
            GetChoiceInfos(generatedPort).choiceText = evt.newValue;
        });
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        if (isDeletable)
        {
            var deleteButton = new Button(() => RemovePort(generatedPort))
            {
                text = "X",
                style =
                {
                    backgroundColor = new Color(0.5f, 0, 0)
                }
            };
            generatedPort.contentContainer.Add(deleteButton);
        }
        

        generatedPort.portName = choicePortName;
        
        outputContainer.Add(generatedPort);

        choices.Add(new ChoiceInfos
        {
            choiceText = choicePortName,
            port = generatedPort,
            textField = textField
        });
        RefreshPorts();
        RefreshExpandedState();
    }

    private void RemovePort(Port generatedPort)
    {
        if (graphView == null) return;
        var targetEdge = graphView.edges.Where(x => x.output.portName == generatedPort.portName && x.output.node == this);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            graphView.RemoveElement(targetEdge.First());
        }

        choices.Remove(GetChoiceInfos(generatedPort));
        outputContainer.Remove(generatedPort);
        
        RefreshPorts();
        RefreshExpandedState();

    }

    public ChoiceInfos GetChoiceInfos(Port port)
    {
        return choices.FirstOrDefault(x => x.port == port);
    }

    public void UpdatePort(int id, string text)
    {
        choices[id].textField.value = text;
        choices[id].choiceText = text;
    }
}
#endif