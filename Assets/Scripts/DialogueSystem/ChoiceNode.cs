using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class ChoiceNode : BaseNode
{
    public string dialogueText;

    public void AddChoicePort()
    {
        var generatedPort = GeneratePort(Direction.Output);
        var choiceCount = outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {choiceCount}";
        outputContainer.Add(generatedPort);
        RefreshPorts();
        RefreshExpandedState();
    }
}
