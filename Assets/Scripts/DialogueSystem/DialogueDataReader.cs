using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DialogueDataReader : MonoBehaviour
{

    public DialogueData dialogueData;

    private NodeData _currentNodeData;

    private MessageApp _messageApp;
    private ContactApp _contactApp;

    private string _characterID;


    public string CharacterID { get => _characterID; set => _characterID = value; }

    private void OnEnable()
    {
        _messageApp = AppManager.Instance.GetApplication(ApplicationType.Messages) as MessageApp;
        
        //Get dialogueData from contact app
        /*var contactApp = AppManager.Instance.GetApplication(ApplicationType.Contacts) as ContactApp;
        dialogueData = contactApp.*/
    }


    public void StartConversation()
    {
        _contactApp = AppManager.Instance.GetApplication(ApplicationType.Contacts) as ContactApp;
        List<NodeData> nodes = dialogueData.nodes;
        var affinityProperty = dialogueData.properties.FirstOrDefault(x => x.Name == "Affinity");
        //StartCoroutine(DelayMessage(5, "hihi"));
        //Get affinity from character ID
        //affinityProperty.SetValue()
        ReadNodeData(GetNextNodeData(dialogueData.nodes.FirstOrDefault(node => node.nodeGUID == dialogueData.entryPointNodeGuid))).Invoke();
        
    }

    private NodeData GetNextNodeData(NodeData currentNodeData, int outputID = 0)
    {
        if(currentNodeData == null) { return null; }
        return dialogueData.nodes.FirstOrDefault(node => node.nodeGUID == currentNodeData.outputs[outputID].targetNodeGuid);
    }

    private void ReadNextNode(NodeData currentNodeData, int outputID = 0)
    {
        var nextData = GetNextNodeData(currentNodeData, outputID);
        if(nextData == null) { return; }
        ReadNodeData(nextData).Invoke();
    }

    public Action ReadNodeData(NodeData nodeData)
    {
        _currentNodeData = nodeData;
        
        if (nodeData == null)
            return () => { };
        switch (nodeData.nodeType)
        {

            case NodeType.Dialogue:
                DialogueNodeData dialogueNodeData = nodeData as DialogueNodeData;
                return () =>
                {
                    _messageApp.AddMessage(dialogueNodeData.dialogueText, dialogueNodeData.isNPC, _characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Choice:
                ChoiceNodeData choiceData = nodeData as ChoiceNodeData;
                return () =>
                {
                    if(!string.IsNullOrEmpty(choiceData.dialogueText))
                    {
                        _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                    }
                    _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                };
            case NodeType.Affinity:
                AffinityNodeData affinityNodeData = nodeData as AffinityNodeData;
                return () =>
                {
                    
                    _contactApp.GainRelation(affinityNodeData.affinityGain, _characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Condition:
                ConditionNodeData conditionNodeData = nodeData as ConditionNodeData;
                return () =>
                {
                    
                    if (GetFinalConditionValue(conditionNodeData.conditions))
                    {
                        ReadNextNode(nodeData, 0);
                    }
                    else
                    {
                        ReadNextNode(nodeData, 1);
                    }

                };

            case NodeType.Set:
                SetPropertyNodeData setNodeData = nodeData as SetPropertyNodeData;
                return () =>
                {
                    ExposedProperty property = dialogueData.properties.FirstOrDefault(prop => prop.Name == setNodeData.property.Name);
                    if(property != null)
                    {
                        property.SetValue(ExposedProperty.GetValueFromString(property.type, setNodeData.valueString));
                    }
                    ReadNextNode(nodeData, 0);
                };

            default:
                return () => { };
        }
        
    }


    List<string> GetChoicesTexts(List<OutputData> choices)
    {
        List<string> choicesTexts = new List<string>();
        for (int i = 0; i < choices.Count; i++)
        {
            choicesTexts.Add(choices[i].portValue);
        }

        return choicesTexts;
    }

    public OutputData GetChoiceFromText(string text)
    {
        OutputData choice = _currentNodeData.outputs.FirstOrDefault(output => output.portValue == text);
        return choice;
    }

    public void MakeChoice(string choiceText)
    {
        OutputData choice = GetChoiceFromText(choiceText);
        int choiceID = _currentNodeData.outputs.IndexOf(choice);
        ReadNextNode(_currentNodeData, choiceID);

    }

    public bool GetFinalConditionValue(List<Condition> conditions)
    {
        
        foreach (var condition in conditions)
        {
            if(!condition.Evaluate(dialogueData.properties)) return false;
        }
        return true;
    }


    IEnumerator Conversation()
    {
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator DelayMessage(float timer, string text)
    {
        print("Commenceeee :D");
        yield return new WaitForSeconds(timer);
        print("ayai :D");

    }
}
